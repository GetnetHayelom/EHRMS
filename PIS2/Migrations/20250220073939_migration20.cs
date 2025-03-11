using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "loyaltyUser",
                table: "LoyaltyHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "leaveUser",
                table: "LeaveHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "allowanceUser",
                table: "AllowanceAssignmentsHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loyaltyUser",
                table: "LoyaltyHistories");

            migrationBuilder.DropColumn(
                name: "leaveUser",
                table: "LeaveHistories");

            migrationBuilder.DropColumn(
                name: "allowanceUser",
                table: "AllowanceAssignmentsHistories");
        }
    }
}
