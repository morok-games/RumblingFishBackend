using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Data;
using RumblingFishBackend.Models.DTO.Admin;

namespace RumblingFishBackend.Services
{
    public class AdminPlayerService
    {
        private readonly GameDbContext _db;

        public AdminPlayerService(GameDbContext db)
        {
            _db = db;
        }
            
        public async Task<PlayerListViewModel> GetPlayersAsync(int page, int pageSize, PlayerListFilter filter)
        {
            var query = _db.PlayerStatistics.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.CountryCode))
                query = query.Where(x => x.Player.CountryCode == filter.CountryCode);

            if (!string.IsNullOrWhiteSpace(filter.Nickname))
                query = query.Where(x => EF.Functions.ILike(x.Player.Nickname!, $"%{filter.Nickname}%"));

            if (filter.DateFrom.HasValue)
                query = query.Where(x => x.Player.User.CreatedAt >= filter.DateFrom.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));

            if (filter.DateTo.HasValue)
                query = query.Where(x => x.Player.User.CreatedAt < filter.DateTo.Value.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));

            var totalPlayers = await query.CountAsync();
            var totalLevels = await _db.Levels.CountAsync();

            var rows = await query
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.PlayerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Player.Nickname,
                    x.Player.CountryCode,
                    x.Player.User.CreatedAt,
                    x.Score,
                    CompletedLevels = _db.PlayerLevelStatistics.Count(pls => pls.PlayerId == x.PlayerId && pls.Completed),
                    Rank = 1 + _db.PlayerStatistics.Count(p =>p.Score > x.Score || (p.Score == x.Score && p.PlayerId < x.PlayerId))
                })
                .ToListAsync();

            var players = rows
                .Select(row => new PlayerListRow(
                    Rank: row.Rank,
                    CountryCode: row.CountryCode,
                    Nickname: row.Nickname ?? string.Empty,
                    CompletedLevels: row.CompletedLevels,
                    TotalLevels: totalLevels,
                    CreatedAt: row.CreatedAt,
                    Score: row.Score))
                .ToList();

            var totalPages = (int)Math.Ceiling(totalPlayers / (double)pageSize);

            return new PlayerListViewModel(players, page, totalPages, pageSize, filter);
        }   
    }
}
