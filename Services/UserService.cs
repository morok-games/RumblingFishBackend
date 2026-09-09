using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Data;
using RumblingFishBackend.Models;

namespace RumblingFishBackend.Services
{
    public class UserService
    {
        private readonly GameDbContext _db;
        private readonly FirebaseSaveService _firebaseSaveService;
        private readonly GeoIpService _geoIpService;

        public UserService(GameDbContext db, FirebaseSaveService firebaseSaveService, GeoIpService geoIpService)
        {
            _db = db;
            _firebaseSaveService = firebaseSaveService;
            _geoIpService = geoIpService;
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
                    CountryCode = countryCode,
                    CreatedAt = now,
                    LastLoginAt = now
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                user.Nickname = $"Player{user.Id}";

                // Create PlayerStatistics
                var playerStatistics = new PlayerStatistics
                {
                    User = user,
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
                            User = user,
                            LevelId = levelId,
                            Deaths = 0,
                            Attempts = 1,
                            PlayTime = 0,
                            Completed = true
                        });
                    }
                }

                await _db.SaveChangesAsync();

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
