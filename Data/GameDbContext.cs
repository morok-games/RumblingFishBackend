using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Models;
using RumblingFishBackend.Data.Seed;

namespace RumblingFishBackend.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<PlayerStatistics> PlayerStatistics { get; set; }
        public DbSet<PlayerLevelStatistics> PlayerLevelStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.FirebaseUid)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Nickname)
                .IsUnique();

            modelBuilder.Entity<PlayerStatistics>()
                .HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<PlayerStatistics>(x => x.UserId)
                .IsRequired();

            modelBuilder.Entity<PlayerLevelStatistics>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            modelBuilder.Entity<PlayerLevelStatistics>()
                .HasOne<Level>()
                .WithMany()
                .HasForeignKey(x => x.LevelId)
                .IsRequired();

            modelBuilder.Entity<PlayerLevelStatistics>()
                .HasIndex(x => new { x.UserId, x.LevelId })
                .IsUnique();

            LevelSeed.Seed(modelBuilder);
        }
    }
}
