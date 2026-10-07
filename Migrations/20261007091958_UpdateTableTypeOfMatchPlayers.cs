using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnBasedWebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableTypeOfMatchPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayer_Matches_MatchId",
                table: "MatchPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayer_Player_PlayerId",
                table: "MatchPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Player",
                table: "Player");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchPlayer",
                table: "MatchPlayer");

            migrationBuilder.RenameTable(
                name: "Player",
                newName: "Players");

            migrationBuilder.RenameTable(
                name: "MatchPlayer",
                newName: "MatchPlayers");

            migrationBuilder.RenameIndex(
                name: "IX_MatchPlayer_PlayerId",
                table: "MatchPlayers",
                newName: "IX_MatchPlayers_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_MatchPlayer_MatchId_Slot",
                table: "MatchPlayers",
                newName: "IX_MatchPlayers_MatchId_Slot");

            migrationBuilder.RenameIndex(
                name: "IX_MatchPlayer_MatchId_PlayerId",
                table: "MatchPlayers",
                newName: "IX_MatchPlayers_MatchId_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchPlayers",
                table: "MatchPlayers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayers_Matches_MatchId",
                table: "MatchPlayers",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayers_Players_PlayerId",
                table: "MatchPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayers_Matches_MatchId",
                table: "MatchPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayers_Players_PlayerId",
                table: "MatchPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchPlayers",
                table: "MatchPlayers");

            migrationBuilder.RenameTable(
                name: "Players",
                newName: "Player");

            migrationBuilder.RenameTable(
                name: "MatchPlayers",
                newName: "MatchPlayer");

            migrationBuilder.RenameIndex(
                name: "IX_MatchPlayers_PlayerId",
                table: "MatchPlayer",
                newName: "IX_MatchPlayer_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_MatchPlayers_MatchId_Slot",
                table: "MatchPlayer",
                newName: "IX_MatchPlayer_MatchId_Slot");

            migrationBuilder.RenameIndex(
                name: "IX_MatchPlayers_MatchId_PlayerId",
                table: "MatchPlayer",
                newName: "IX_MatchPlayer_MatchId_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Player",
                table: "Player",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchPlayer",
                table: "MatchPlayer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayer_Matches_MatchId",
                table: "MatchPlayer",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayer_Player_PlayerId",
                table: "MatchPlayer",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
