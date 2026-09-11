using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Data;
using RumblingFishBackend.Models;

namespace RumblingFishBackend.Services
{
    public class UserService
    {
        private const int maxLevelRating = 3;
        private readonly GameDbContext _db;
        private readonly FirebaseSaveService _firebaseSaveService;
        private readonly GeoIpService _geoIpService;
        private readonly PlayerService _playerService;

        public UserService(GameDbContext db, FirebaseSaveService firebaseSaveService, GeoIpService geoIpService, PlayerService playerService)
        {
            _db = db;
            _firebaseSaveService = firebaseSaveService;
            _geoIpService = geoIpService;
            _playerService = playerService;
        }

        public async Task<User> GetOrCreateUserAsync(string firebaseUid, string? ipAddress)
        {
            // Check if the user exists
            var user = await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUid == firebaseUid);

            var now = DateTime.UtcNow;

            // User Exists
            if (user != null)
            {
                user.LastIpAddress = ipAddress;
                user.LastLoginAt = now;

                await _db.SaveChangesAsync();

                return user;
            }

            //Read the old saved state BEFORE starting the database transaction
            var saveData = await _firebaseSaveService.GetSaveDataAsync(firebaseUid);
            var countryCode = await _geoIpService.GetCountryCodeAsync(ipAddress);

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Check if the user exists again
                user = await _db.Users.FirstOrDefaultAsync(x => x.FirebaseUid == firebaseUid);

                if (user != null)
                {
                    user.LastIpAddress = ipAddress;
                    user.LastLoginAt = now;

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return user;
                }

                // Create new User
                user = new User
                {
                    FirebaseUid = firebaseUid,
                    LastIpAddress = ipAddress,
                    CreatedAt = now,
                    LastLoginAt = now
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

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
                    Experience = saveData?.gameStatistics?.Experience ?? 0,
                    CoinsCollected = saveData?.gameStatistics?.CoinsCollected ?? 0
                };

                _db.PlayerStatistics.Add(playerStatistics);

                // Create PlayerLevelStatistics

                var levelIds = await _db.Levels.Select(x => x.Id).ToHashSetAsync();

                if (saveData?.completedLevels != null)
                {
                    foreach (var levelId in saveData.completedLevels.Distinct())
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
                            Rating = maxLevelRating,
                            Completed = true
                        });
                    }
                }

                await _db.SaveChangesAsync();
                await _playerService.UpdateLevelsScoreAsync(player);

                await transaction.CommitAsync();

                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
