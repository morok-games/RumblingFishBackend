using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Models;

namespace RumblingFishBackend.Data.Seed
{
    public class AdminAccountSeed
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();

            if (await db.AdminAccounts.AnyAsync())
            {
                return;
            }

            var username = app.Configuration["Admin:InitialUsername"];
            var password = app.Configuration["Admin:InitialPassword"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AdminAccount>>();

            var admin = new AdminAccount { Username = username, CreatedAt = DateTime.UtcNow };
            admin.PasswordHash = hasher.HashPassword(admin, password);

            db.AdminAccounts.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}
