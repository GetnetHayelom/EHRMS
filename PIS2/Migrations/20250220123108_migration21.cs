using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "addressID",
                table: "WorkSitesHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "addressModeladdressID",
                table: "WorkSitesHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "workSiteName",
                table: "WorkSitesHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "workSiteUser",
                table: "WorkSitesHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "overtimeUser",
                table: "OvertimeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSitesHistories_addressModeladdressID",
                table: "WorkSitesHistories",
                column: "addressModeladdressID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSitesHistories_Addresses_addressModeladdressID",
                table: "WorkSitesHistories",
                column: "addressModeladdressID",
                principalTable: "Addresses",
                principalColumn: "addressID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSitesHistories_Addresses_addressModeladdressID",
                table: "WorkSitesHistories");

            migrationBuilder.DropIndex(
                name: "IX_WorkSitesHistories_addressModeladdressID",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "addressID",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "addressModeladdressID",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "workSiteName",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "workSiteUser",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "overtimeUser",
                table: "OvertimeHistories");
        }
    }
}
