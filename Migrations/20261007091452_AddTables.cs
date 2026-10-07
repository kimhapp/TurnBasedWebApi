using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnBasedWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayer_Match_MatchId",
                table: "MatchPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Match",
                table: "Match");

            migrationBuilder.RenameTable(
                name: "Match",
                newName: "Matches");

            migrationBuilder.RenameIndex(
                name: "IX_Match_Code",
                table: "Matches",
                newName: "IX_Matches_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Matches",
                table: "Matches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayer_Matches_MatchId",
                table: "MatchPlayer",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayer_Matches_MatchId",
                table: "MatchPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Matches",
                table: "Matches");

            migrationBuilder.RenameTable(
                name: "Matches",
                newName: "Match");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_Code",
                table: "Match",
                newName: "IX_Match_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Match",
                table: "Match",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayer_Match_MatchId",
                table: "MatchPlayer",
                column: "MatchId",
                principalTable: "Match",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
