using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RumblingFishBackend.Migrations
{
    /// <inheritdoc />
    public partial class playerLevelStatisticsRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "PlayerLevelStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "PlayerLevelStatistics");
        }
    }
}
