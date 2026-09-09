using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RumblingFishBackend.Migrations
{
    /// <inheritdoc />
    public partial class RenameDisplayNameToNickname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Users",
                newName: "Nickname");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Nickname",
                table: "Users",
                column: "Nickname",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Nickname",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Nickname",
                table: "Users",
                newName: "DisplayName");
        }
    }
}
