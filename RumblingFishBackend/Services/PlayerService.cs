using Microsoft.EntityFrameworkCore;
using Npgsql;
using RumblingFishBackend.Data;
using RumblingFishBackend.Models;
using RumblingFishBackend.Models.DTO.Level;

namespace RumblingFishBackend.Services
{
    public class PlayerService
    {
        private readonly GameDbContext _db;
        private readonly ILogger<PlayerService> _logger;    

        public PlayerService(GameDbContext db, ILogger<PlayerService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Models.Player?> GetPlayerAsync(string firebaseUid)
        {
            var player = await _db.Players.FirstOrDefaultAsync(p => p.User.FirebaseUid == firebaseUid);

            return player;
        }

        public async Task<Player> CreatePlayerAsync(
            User user,string? countryCode,int initialExperience,int initialCoins,IEnumerable<int>? completedLevelIds)
        {
            //Create Player
            var player = new Player
            {
                User = user,
                Nickname = $"Player{user.Id}",
                CountryCode = countryCode
            };

            // Create PlayerStatistics
            var playerStatistics = new PlayerStatistics
            {
                Player = player,
                Experience = initialExperience,
                CoinsCollected = initialCoins
            };

            _db.PlayerStatistics.Add(playerStatistics);

            // Create PlayerLevelStatistics

            var levelIds = await _db.Levels.Select(x => x.Id).ToHashSetAsync();

            if (completedLevelIds != null)
            {
                foreach (var levelId in completedLevelIds.Distinct())
                {
                    if (!levelIds.Contains(levelId))
                    {
                        continue;
                    }

                    _db.PlayerLevelStatistics.Add(new PlayerLevelStatistics
                    {
                        Player = player,
                        LevelId = levelId,
                        Deaths = 0,
                        Attempts = 1,
                        PlayTime = 0,
                        Rating = Level.MaxLevelRating,
                        Completed = true
                    });
                }
            }

            await _db.SaveChangesAsync();
            await UpdateLevelsScoreAsync(player);

            return player;
        }

        public async Task<SetNicknameResult> TrySetNicknameAsync(string firebaseUid, string nickname)
        {
            var player = await _db.Players.FirstOrDefaultAsync(p => p.User.FirebaseUid == firebaseUid);

            if(player == null)
            {
                return SetNicknameResult.PlayerNotFound;
            }

            player.Nickname = nickname;

            try
            {
                await _db.SaveChangesAsync();
                return SetNicknameResult.Success;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return SetNicknameResult.NicknameTaken;
            }
        }

        public async Task<SetPlayerStatisticResult> TryAddPlayerStatistic(string firebaseUid, int levelId, LevelResultRequest request)
        {
            var player = await _db.Players.FirstOrDefaultAsync(p => p.User.FirebaseUid == firebaseUid);

            if (player == null)
            {
                return SetPlayerStatisticResult.PlayerNotFound;
            }

            var level = await _db.Levels.FirstOrDefaultAsync(l=>l.Id == levelId);

            if(level == null)
            {
                return SetPlayerStatisticResult.InvalidLevel;
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {

                var playerStatistics = await _db.PlayerStatistics.FirstOrDefaultAsync(p => p.Player == player);

                if (playerStatistics == null)
                {
                    playerStatistics = new Models.PlayerStatistics
                    {
                        Player = player,
                        Experience = request.Experience,
                        CoinsCollected = request.CoinsCollected
                    };

                    _db.PlayerStatistics.Add(playerStatistics);
                }
                else
                {
                    playerStatistics.Experience += request.Experience;
                    playerStatistics.CoinsCollected += request.CoinsCollected;
                }

                var playerLevelStatistics = await _db.PlayerLevelStatistics.FirstOrDefaultAsync(p => p.Player == player && p.Level == level);

                if (playerLevelStatistics == null)
                {
                    playerLevelStatistics = new Models.PlayerLevelStatistics
                    {
                        Player = player,
                        Level = level,
                        Deaths = request.Deaths,
                        Attempts = 1,
                        PlayTime = request.PlayTime,
                        Rating = request.Rating,
                        Completed = request.Completed
                    };

                    _db.PlayerLevelStatistics.Add(playerLevelStatistics);
                }
                else
                {
                    playerLevelStatistics.Deaths += request.Deaths;
                    playerLevelStatistics.Attempts++;
                    playerLevelStatistics.PlayTime += request.PlayTime;
                    playerLevelStatistics.Rating = Math.Max(playerLevelStatistics.Rating,request.Rating);
                    playerLevelStatistics.Completed = playerLevelStatistics.Completed ? true : request.Completed;
                }

                await _db.SaveChangesAsync();
                await UpdateLevelsScoreAsync(player);

                await transaction.CommitAsync();

                return SetPlayerStatisticResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to save level statistic for player");
                return SetPlayerStatisticResult.Error;
            }
        }
        
        private async Task UpdateLevelsScoreAsync(Models.Player player)
        {
            var levelsScore = await _db.PlayerLevelStatistics
                .Where(x => x.Player == player && x.Completed)
                .SumAsync(x => x.Level.Reward * x.Rating);

            var playerStatistics = await _db.PlayerStatistics
                .FirstOrDefaultAsync(x => x.Player == player);

            if (playerStatistics != null)
            {
                playerStatistics.LevelsScore = levelsScore;
                playerStatistics.Score = playerStatistics.Experience + levelsScore;
                await _db.SaveChangesAsync();
            }
        }
    }

    public enum SetNicknameResult { Success, NicknameTaken, PlayerNotFound }
    public enum SetPlayerStatisticResult { Success, Error, InvalidLevel, PlayerNotFound }
}
