using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Data;
using System.Linq.Expressions;

namespace RumblingFishBackend.Services
{
    public class LeaderboardService
    {
        private readonly GameDbContext _db;

        // Kept as an Expression (not a compiled delegate) so it can be composed into
        // a query's Select(...) and still translate to SQL — this must always be the
        // LAST operator in a query chain, after every Where/OrderBy/Count on the entity itself.
        private static readonly Expression<Func<Models.PlayerStatistics, LeaderboardRow>> ToRow =
            x => new LeaderboardRow(x.PlayerId, x.Player.Nickname, x.Player.CountryCode, x.Score);

        public LeaderboardService(GameDbContext db)
        {
            _db = db;
        }

        public async Task<LeaderboardResponse?> GetLeaderboardByPlayerAsync(
            Models.Player player, int amountOfFirstPlayers, int amountBeforePlayer, int amountAfterPlayer)
        {
            var myStatistics = await _db.PlayerStatistics.FirstOrDefaultAsync(x => x.PlayerId == player.Id);

            if (myStatistics == null)
            {
                return null;
            }

            var myScore = myStatistics.Score;
            var myPlayerId = player.Id;

            var top = await _db.PlayerStatistics
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.PlayerId)
                .Take(amountOfFirstPlayers)
                .Select(ToRow)
                .ToListAsync();

            // Last entry in Top is the cutoff — anyone ranked at or better than this is already in Top.
            // If there are fewer players than requested, there's no cutoff: everyone is already in Top.
            var cutoff = top.Count == amountOfFirstPlayers ? top[^1] : null;

            var before = new List<LeaderboardRow>();
            var after = new List<LeaderboardRow>();

            if (cutoff != null)
            {
                before = await _db.PlayerStatistics
                    .Where(x =>
                        (x.Score > myScore || (x.Score == myScore && x.PlayerId < myPlayerId)) &&
                        (x.Score < cutoff.Score || (x.Score == cutoff.Score && x.PlayerId > cutoff.PlayerId)))
                    .OrderBy(x => x.Score)
                    .ThenByDescending(x => x.PlayerId)
                    .Take(amountBeforePlayer)
                    .Select(ToRow)
                    .ToListAsync();

                before.Reverse();

                after = await _db.PlayerStatistics
                    .Where(x =>
                        (x.Score < myScore || (x.Score == myScore && x.PlayerId > myPlayerId)) &&
                        (x.Score < cutoff.Score || (x.Score == cutoff.Score && x.PlayerId > cutoff.PlayerId)))
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.PlayerId)
                    .Take(amountAfterPlayer)
                    .Select(ToRow)
                    .ToListAsync();
            }

            // How many players are strictly better than me, plus one — same "better than me"
            // comparison used to filter Before, just counted instead of listed.
            var myRank = await _db.PlayerStatistics
                .CountAsync(x => x.Score > myScore || (x.Score == myScore && x.PlayerId < myPlayerId)) + 1;

            var me = new LeaderboardEntry(myRank, player.Nickname ?? string.Empty, player.CountryCode, myScore);

            // Top is ranks 1..top.Count.
            var topEntries = top
                .Select((row, index) => ToEntry(row, index + 1))
                .ToList();

            // Before ends right before myRank, so its entries occupy myRank-before.Count .. myRank-1.
            var beforeEntries = before
                .Select((row, index) => ToEntry(row, myRank - before.Count + index))
                .ToList();

            // After starts right after myRank, so its entries occupy myRank+1 .. myRank+after.Count.
            var afterEntries = after
                .Select((row, index) => ToEntry(row, myRank + 1 + index))
                .ToList();

            return new LeaderboardResponse(topEntries, beforeEntries, me, afterEntries);
        }

        private static LeaderboardEntry ToEntry(LeaderboardRow row, int rank)
        {
            return new LeaderboardEntry(rank, row.Nickname ?? string.Empty, row.CountryCode, row.Score);
        }

        private record LeaderboardRow(int PlayerId, string? Nickname, string? CountryCode, int Score);
        public record LeaderboardEntry(int Rank, string Nickname, string? CountryCode, int Score);

        public record LeaderboardResponse(
            List<LeaderboardEntry> Top,
            List<LeaderboardEntry> Before,
            LeaderboardEntry Me,
            List<LeaderboardEntry> After);
    }
}
