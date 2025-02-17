using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves",
                sql: "[leaveDays]>0 AND [leaveDays] <= DATEDIFF(DAY, [leaveStartDate], [leaveEndDate])+1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leave_NumberOfDays",
                table: "Leaves",
                sql: "[leaveDays]>0 AND [leaveDays]<DATEDIFF(DAY, [leaveStartDate], [leaveEndDate]");
        }
    }
}
