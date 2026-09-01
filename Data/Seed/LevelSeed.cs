using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Models;

namespace RumblingFishBackend.Data.Seed
{
    public static class LevelSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Level>().HasData(
                new Level { Id = 1, Name = "Level 1" },
                new Level { Id = 2, Name = "Level 2" },
                new Level { Id = 3, Name = "Level 3" },
                new Level { Id = 4, Name = "Level 4" },
                new Level { Id = 5, Name = "Level 5" },
                new Level { Id = 6, Name = "Level 6" },
                new Level { Id = 7, Name = "Level 7" },
                new Level { Id = 8, Name = "Level 8" },
                new Level { Id = 9, Name = "Level 9" }
            );
        }
    }
}
