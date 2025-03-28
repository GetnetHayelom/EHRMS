using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terminations_Employments_subAccountDescription",
                table: "Terminations");

            migrationBuilder.RenameColumn(
                name: "subAccountStatus",
                table: "Terminations",
                newName: "terminationRemark");

            migrationBuilder.RenameColumn(
                name: "subAccountName",
                table: "Terminations",
                newName: "terminationReason");

            migrationBuilder.RenameColumn(
                name: "subAccountDescription",
                table: "Terminations",
                newName: "employmentID");

            migrationBuilder.RenameColumn(
                name: "accountID",
                table: "Terminations",
                newName: "terminationDate");

            migrationBuilder.RenameColumn(
                name: "subAccountID",
                table: "Terminations",
                newName: "terminationID");

            migrationBuilder.RenameIndex(
                name: "IX_Terminations_subAccountDescription",
                table: "Terminations",
                newName: "IX_Terminations_employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Terminations_Employments_employmentID",
                table: "Terminations",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terminations_Employments_employmentID",
                table: "Terminations");

            migrationBuilder.RenameColumn(
                name: "terminationRemark",
                table: "Terminations",
                newName: "subAccountStatus");

            migrationBuilder.RenameColumn(
                name: "terminationReason",
                table: "Terminations",
                newName: "subAccountName");

            migrationBuilder.RenameColumn(
                name: "terminationDate",
                table: "Terminations",
                newName: "accountID");

            migrationBuilder.RenameColumn(
                name: "employmentID",
                table: "Terminations",
                newName: "subAccountDescription");

            migrationBuilder.RenameColumn(
                name: "terminationID",
                table: "Terminations",
                newName: "subAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_Terminations_employmentID",
                table: "Terminations",
                newName: "IX_Terminations_subAccountDescription");

            migrationBuilder.AddForeignKey(
                name: "FK_Terminations_Employments_subAccountDescription",
                table: "Terminations",
                column: "subAccountDescription",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
