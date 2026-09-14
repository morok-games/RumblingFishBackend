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
        public DbSet<Player> Players { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<PlayerStatistics> PlayerStatistics { get; set; }
        public DbSet<PlayerLevelStatistics> PlayerLevelStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.FirebaseUid)
                .IsUnique();

            modelBuilder.Entity<Player>()
                .HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<Player>(x => x.UserId)
                .IsRequired();

            modelBuilder.Entity<Player>()
                .HasIndex(x => x.Nickname)
                .IsUnique();

            modelBuilder.Entity<PlayerStatistics>()
                .HasOne(x => x.Player)
                .WithOne()
                .HasForeignKey<PlayerStatistics>(x => x.PlayerId)
                .IsRequired();

            modelBuilder.Entity<PlayerStatistics>()
                .HasIndex(x => new { x.Score, x.PlayerId });

            modelBuilder.Entity<PlayerLevelStatistics>()
                .HasOne(x => x.Player)
                .WithMany()
                .HasForeignKey(x => x.PlayerId)
                .IsRequired();

            modelBuilder.Entity<PlayerLevelStatistics>()
                .HasOne(x => x.Level)
                .WithMany()
                .HasForeignKey(x => x.LevelId)
                .IsRequired();

            modelBuilder.Entity<PlayerLevelStatistics>()
                .HasIndex(x => new { x.PlayerId, x.LevelId })
                .IsUnique();

            LevelSeed.Seed(modelBuilder);
        }
    }
}
