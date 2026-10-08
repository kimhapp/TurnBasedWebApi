using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnBasedWebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchPlayers_MatchId_PlayerId",
                table: "MatchPlayers");

            migrationBuilder.DropIndex(
                name: "IX_Matches_Code",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "CurrentTurnMatchPlayerId",
                table: "Matches",
                newName: "CurrentTurnPlayerId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Matches",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Matches",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Code",
                table: "Matches",
                column: "Code",
                unique: true,
                filter: "\"StartedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Matches_Code",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "CurrentTurnPlayerId",
                table: "Matches",
                newName: "CurrentTurnMatchPlayerId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Matches",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.CreateIndex(
                name: "IX_MatchPlayers_MatchId_PlayerId",
                table: "MatchPlayers",
                columns: new[] { "MatchId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Code",
                table: "Matches",
                column: "Code",
                unique: true);
        }
    }
}
