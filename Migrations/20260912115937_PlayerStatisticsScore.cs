using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RumblingFishBackend.Migrations
{
    /// <inheritdoc />
    public partial class PlayerStatisticsScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "PlayerStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_Score_PlayerId",
                table: "PlayerStatistics",
                columns: new[] { "Score", "PlayerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerStatistics_Score_PlayerId",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "PlayerStatistics");
        }
    }
}
