using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "workSiteHistoryAction",
                table: "WorkSitesHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "personLastName",
                table: "Persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "employmentMaxAge",
                table: "EmploymentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "employmentMinAge",
                table: "EmploymentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leave_leaveEndDate",
                table: "Leaves",
                sql: "[leaveEndDate]>=[leaveStartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves",
                sql: "[leaveDays]>0 AND [leaveDays] <= DATEDIFF(DAY, [leaveStartDate], [leaveEndDate])+1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leave_leaveEndDate",
                table: "Leaves");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "employmentMaxAge",
                table: "EmploymentTypes");

            migrationBuilder.DropColumn(
                name: "employmentMinAge",
                table: "EmploymentTypes");

            migrationBuilder.AlterColumn<string>(
                name: "workSiteHistoryAction",
                table: "WorkSitesHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "personLastName",
                table: "Persons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
