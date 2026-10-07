using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnBasedWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReadyInMatchPlayerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReady",
                table: "MatchPlayers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReady",
                table: "MatchPlayers");
        }
    }
}
