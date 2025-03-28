using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "workSiteUser",
                table: "WorkSitesHistories",
                newName: "mapLink");

            migrationBuilder.AlterColumn<int>(
                name: "addressID",
                table: "WorkSitesHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "employmentID",
                table: "WorkSitesHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employmentID",
                table: "workSiteModel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mapLink",
                table: "workSiteModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "subAccountID",
                table: "workSiteModel",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_workSiteModel_employmentID",
                table: "workSiteModel",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_workSiteModel_subAccountID",
                table: "workSiteModel",
                column: "subAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_workSiteModel_Employments_employmentID",
                table: "workSiteModel",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_workSiteModel_SubAccounts_subAccountID",
                table: "workSiteModel",
                column: "subAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_workSiteModel_Employments_employmentID",
                table: "workSiteModel");

            migrationBuilder.DropForeignKey(
                name: "FK_workSiteModel_SubAccounts_subAccountID",
                table: "workSiteModel");

            migrationBuilder.DropIndex(
                name: "IX_workSiteModel_employmentID",
                table: "workSiteModel");

            migrationBuilder.DropIndex(
                name: "IX_workSiteModel_subAccountID",
                table: "workSiteModel");

            migrationBuilder.DropColumn(
                name: "employmentID",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "employmentID",
                table: "workSiteModel");

            migrationBuilder.DropColumn(
                name: "mapLink",
                table: "workSiteModel");

            migrationBuilder.DropColumn(
                name: "subAccountID",
                table: "workSiteModel");

            migrationBuilder.RenameColumn(
                name: "mapLink",
                table: "WorkSitesHistories",
                newName: "workSiteUser");

            migrationBuilder.AlterColumn<int>(
                name: "addressID",
                table: "WorkSitesHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
