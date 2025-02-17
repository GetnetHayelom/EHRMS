using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Employments_EmploymentTypes_employmentTypeModelemploymentTypeID",
            //    table: "Employments");

            //migrationBuilder.DropIndex(
            //    name: "IX_Employments_employmentTypeModelemploymentTypeID",
            //    table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_Employments_employmentTypeModelemploymentTypeID1",
                table: "Employments");

            //migrationBuilder.DropColumn(
            //    name: "employmentTypeModelemploymentTypeID",
            //    table: "Employments");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_employmentTypeID",
                table: "Employments",
                column: "employmentTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_EmploymentTypes_employmentTypeID",
                table: "Employments",
                column: "employmentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_EmploymentTypes_employmentTypeID",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_Employments_employmentTypeID",
                table: "Employments");

            migrationBuilder.AddColumn<int>(
                name: "employmentTypeModelemploymentTypeID",
                table: "Employments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_employmentTypeModelemploymentTypeID",
                table: "Employments",
                column: "employmentTypeModelemploymentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_employmentTypeModelemploymentTypeID1",
                table: "Employments",
                column: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_EmploymentTypes_employmentTypeModelemploymentTypeID",
                table: "Employments",
                column: "employmentTypeModelemploymentTypeID",
                principalTable: "EmploymentTypes",
                principalColumn: "employmentTypeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
