using Microsoft.EntityFrameworkCore;
using Npgsql;
using RumblingFishBackend.Data;

namespace RumblingFishBackend.Services
{
    public class PlayerService
    {
        private readonly GameDbContext _db;

        public PlayerService(GameDbContext db)
        {
            _db = db;
        }

        public async Task<string?> GetNicknameAsync(string firebaseUid)
        {
            var player = await _db.Players.FirstOrDefaultAsync(p => p.User.FirebaseUid == firebaseUid);

            return player?.Nickname;
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
    }

    public enum SetNicknameResult { Success, NicknameTaken, PlayerNotFound }
}
