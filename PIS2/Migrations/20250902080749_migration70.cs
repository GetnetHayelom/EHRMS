using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration70 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "reportsTo",
                table: "Structures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reportsTo",
                table: "StructureHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Structures_reportsTo",
                table: "Structures",
                column: "reportsTo");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs",
                column: "jobCode",
                unique: true,
                filter: "[jobCode] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_Structures_reportsTo",
                table: "Structures",
                column: "reportsTo",
                principalTable: "Structures",
                principalColumn: "structureID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Structures_Structures_reportsTo",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Structures_reportsTo",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "reportsTo",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "reportsTo",
                table: "StructureHistories");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_jobCode",
                table: "Jobs",
                column: "jobCode",
                unique: true,
                filter: "[jobCode] IS NOT NULL OR ''");
        }
    }
}
