using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.Migrations
{
    public partial class TokenFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Token_Admins_AdminId",
                table: "Token");

            migrationBuilder.DropForeignKey(
                name: "FK_Token_Users_UserId",
                table: "Token");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Token",
                table: "Token");

            migrationBuilder.RenameTable(
                name: "Token",
                newName: "Tokens");

            migrationBuilder.RenameIndex(
                name: "IX_Token_UserId",
                table: "Tokens",
                newName: "IX_Tokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Token_AdminId",
                table: "Tokens",
                newName: "IX_Tokens_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Admins_AdminId",
                table: "Tokens",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Users_UserId",
                table: "Tokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Admins_AdminId",
                table: "Tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Users_UserId",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens");

            migrationBuilder.RenameTable(
                name: "Tokens",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_UserId",
                table: "Token",
                newName: "IX_Token_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_AdminId",
                table: "Token",
                newName: "IX_Token_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Token",
                table: "Token",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Token_Admins_AdminId",
                table: "Token",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Token_Users_UserId",
                table: "Token",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
