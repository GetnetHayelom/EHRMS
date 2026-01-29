using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration127 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "trainingSessionTitle",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedDate",
                table: "OtherPayments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "paymentStatus",
                table: "OtherPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "OtherPayments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSitesHistories_employmentID",
                table: "WorkSitesHistories",
                column: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSitesHistories_Employments_employmentID",
                table: "WorkSitesHistories",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSitesHistories_Employments_employmentID",
                table: "WorkSitesHistories");

            migrationBuilder.DropIndex(
                name: "IX_WorkSitesHistories_employmentID",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "trainingSessionTitle",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "paymentStatus",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "OtherPayments");
        }
    }
}
