using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration115 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "departmentID",
                table: "Penalties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_departmentID",
                table: "Penalties",
                column: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Penalties_Departments_departmentID",
                table: "Penalties",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Penalties_Departments_departmentID",
                table: "Penalties");

            migrationBuilder.DropIndex(
                name: "IX_Penalties_departmentID",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "departmentID",
                table: "Penalties");
        }
    }
}
