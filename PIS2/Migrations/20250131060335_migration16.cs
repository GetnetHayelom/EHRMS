using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employments_personID",
                table: "Employments");

            migrationBuilder.AlterColumn<string>(
                name: "givenID",
                table: "Employments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_personID_givenID",
                table: "Employments",
                columns: new[] { "personID", "givenID" },
                unique: true,
                filter: "[employmentStatus] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employments_personID_givenID",
                table: "Employments");

            migrationBuilder.AlterColumn<string>(
                name: "givenID",
                table: "Employments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_personID",
                table: "Employments",
                column: "personID",
                unique: true,
                filter: "[employmentStatus] = 1");
        }
    }
}
