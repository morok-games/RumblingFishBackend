using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RumblingFishBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerLevelStatistics_Users_UserId",
                table: "PlayerLevelStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStatistics_Users_UserId",
                table: "PlayerStatistics");

            migrationBuilder.DropIndex(
                name: "IX_Users_Nickname",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PlayerStatistics",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerStatistics_UserId",
                table: "PlayerStatistics",
                newName: "IX_PlayerStatistics_PlayerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PlayerLevelStatistics",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerLevelStatistics_UserId_LevelId",
                table: "PlayerLevelStatistics",
                newName: "IX_PlayerLevelStatistics_PlayerId_LevelId");

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: true),
                    CountryCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_Nickname",
                table: "Players",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerLevelStatistics_Players_PlayerId",
                table: "PlayerLevelStatistics",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStatistics_Players_PlayerId",
                table: "PlayerStatistics",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerLevelStatistics_Players_PlayerId",
                table: "PlayerLevelStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStatistics_Players_PlayerId",
                table: "PlayerStatistics");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "PlayerStatistics",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerStatistics_PlayerId",
                table: "PlayerStatistics",
                newName: "IX_PlayerStatistics_UserId");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "PlayerLevelStatistics",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerLevelStatistics_PlayerId_LevelId",
                table: "PlayerLevelStatistics",
                newName: "IX_PlayerLevelStatistics_UserId_LevelId");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Nickname",
                table: "Users",
                column: "Nickname",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerLevelStatistics_Users_UserId",
                table: "PlayerLevelStatistics",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStatistics_Users_UserId",
                table: "PlayerStatistics",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
