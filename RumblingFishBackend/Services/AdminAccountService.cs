using Microsoft.AspNetCore.Identity;
using RumblingFishBackend.Data;
using RumblingFishBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace RumblingFishBackend.Services
{
    public class AdminAccountService
    {
        private readonly GameDbContext _db;
        private readonly IPasswordHasher<AdminAccount> _passwordHasher;

        public AdminAccountService(GameDbContext db, IPasswordHasher<AdminAccount> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task<AdminAccount?> ValidateCredentialsAsync(string username, string password)
        {
            var account = await _db.AdminAccounts.FirstOrDefaultAsync(x => x.Username == username);

            if (account == null || password == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, password);

            return result == PasswordVerificationResult.Failed ? null : account;
        }
    }
}
