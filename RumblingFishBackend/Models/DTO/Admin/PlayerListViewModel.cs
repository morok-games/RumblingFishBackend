namespace RumblingFishBackend.Models.DTO.Admin
{
    public record PlayerListViewModel(List<PlayerListRow> Players, int CurrentPage, int TotalPages, int PageSize, PlayerListFilter Filter);

    public record PlayerListRow(int Rank, string? CountryCode, string Nickname, int CompletedLevels, int TotalLevels, DateTime CreatedAt, int Score);

    public record PlayerListFilter(string? CountryCode, string? Nickname, DateOnly? DateFrom, DateOnly? DateTo);
}
