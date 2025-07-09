using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration51 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prohibitionHitoryModels_Prohibitions_prohibitionID",
                table: "prohibitionHitoryModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_prohibitionHitoryModels",
                table: "prohibitionHitoryModels");

            migrationBuilder.RenameTable(
                name: "prohibitionHitoryModels",
                newName: "prohibitionHitories");

            migrationBuilder.RenameColumn(
                name: "prohibitionEnD",
                table: "prohibitionHitories",
                newName: "prohibitionEnd");

            migrationBuilder.RenameIndex(
                name: "IX_prohibitionHitoryModels_prohibitionID",
                table: "prohibitionHitories",
                newName: "IX_prohibitionHitories_prohibitionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_prohibitionHitories",
                table: "prohibitionHitories",
                column: "prohibitionHistoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_prohibitionHitories_Prohibitions_prohibitionID",
                table: "prohibitionHitories",
                column: "prohibitionID",
                principalTable: "Prohibitions",
                principalColumn: "prohibitionID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_prohibitionHitories_Prohibitions_prohibitionID",
                table: "prohibitionHitories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_prohibitionHitories",
                table: "prohibitionHitories");

            migrationBuilder.RenameTable(
                name: "prohibitionHitories",
                newName: "prohibitionHitoryModels");

            migrationBuilder.RenameColumn(
                name: "prohibitionEnd",
                table: "prohibitionHitoryModels",
                newName: "prohibitionEnD");

            migrationBuilder.RenameIndex(
                name: "IX_prohibitionHitories_prohibitionID",
                table: "prohibitionHitoryModels",
                newName: "IX_prohibitionHitoryModels_prohibitionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_prohibitionHitoryModels",
                table: "prohibitionHitoryModels",
                column: "prohibitionHistoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_prohibitionHitoryModels_Prohibitions_prohibitionID",
                table: "prohibitionHitoryModels",
                column: "prohibitionID",
                principalTable: "Prohibitions",
                principalColumn: "prohibitionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
