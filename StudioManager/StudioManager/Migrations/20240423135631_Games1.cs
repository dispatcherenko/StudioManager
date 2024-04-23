using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioManager.Migrations
{
    /// <inheritdoc />
    public partial class Games1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "gamepicture",
                table: "games",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gamepicture",
                table: "games");
        }
    }
}
