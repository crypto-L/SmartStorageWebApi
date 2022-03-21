using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.Migrations
{
    public partial class cascadeDeleteEnabledTry1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Storages_ParentStorageId",
                table: "Storages");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Storages_ParentStorageId",
                table: "Storages",
                column: "ParentStorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Storages_ParentStorageId",
                table: "Storages");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Storages_ParentStorageId",
                table: "Storages",
                column: "ParentStorageId",
                principalTable: "Storages",
                principalColumn: "Id");
        }
    }
}
