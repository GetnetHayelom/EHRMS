using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration157 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_BankInfos_creditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_SubAccounts_creditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_BankInfos_creditAccountID",
                table: "PayrollPays");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_PayrollPays_Employments_employmentID",
            //    table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_SubAccounts_creditAccountID",
                table: "PayrollPays");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_WorkSitesHistories_Employments_employmentID",
            //    table: "WorkSitesHistories");

            migrationBuilder.DropIndex(
                name: "IX_WorkSitesHistories_employmentID",
                table: "WorkSitesHistories");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_employmentID",
                table: "PayrollPays");

            migrationBuilder.RenameColumn(
                name: "debitAccountID",
                table: "PayrollPays",
                newName: "subAccountID");

            migrationBuilder.RenameColumn(
                name: "creditAccountID",
                table: "PayrollPays",
                newName: "personAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollPays_creditAccountID",
                table: "PayrollPays",
                newName: "IX_PayrollPays_personAccountID");

            migrationBuilder.RenameColumn(
                name: "debitAccountID",
                table: "OtherPayments",
                newName: "subAccountID");

            migrationBuilder.RenameColumn(
                name: "creditAccountID",
                table: "OtherPayments",
                newName: "personAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_OtherPayments_creditAccountID",
                table: "OtherPayments",
                newName: "IX_OtherPayments_personAccountID");

            migrationBuilder.RenameColumn(
                name: "dedcutionPriority",
                table: "DeductionTypes",
                newName: "deductionPriority");

            migrationBuilder.AddColumn<int>(
                name: "employmentModelemploymentID",
                table: "WorkSitesHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentModelemploymentID",
                table: "PayrollPays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "bankInfoModelbankInfoID",
                table: "PayrollPays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "bankInfoModelbankInfoID",
                table: "OtherPayments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "givenID",
                table: "Employments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "personAccountModelpersonAccountID",
                table: "BankInfos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonAccounts",
                columns: table => new
                {
                    personAccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    personID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonAccounts", x => x.personAccountID);
                    table.ForeignKey(
                        name: "FK_PersonAccounts_Persons_personID",
                        column: x => x.personID,
                        principalTable: "Persons",
                        principalColumn: "personID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkSitesHistories_employmentModelemploymentID",
                table: "WorkSitesHistories",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_bankInfoModelbankInfoID",
                table: "PayrollPays",
                column: "bankInfoModelbankInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_EmploymentModelemploymentID",
                table: "PayrollPays",
                column: "EmploymentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_subAccountID",
                table: "PayrollPays",
                column: "subAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_OtherPayments_bankInfoModelbankInfoID",
                table: "OtherPayments",
                column: "bankInfoModelbankInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_OtherPayments_subAccountID",
                table: "OtherPayments",
                column: "subAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_givenID",
                table: "Employments",
                column: "givenID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankInfos_personAccountModelpersonAccountID",
                table: "BankInfos",
                column: "personAccountModelpersonAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAccounts_personID",
                table: "PersonAccounts",
                column: "personID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BankInfos_PersonAccounts_personAccountModelpersonAccountID",
                table: "BankInfos",
                column: "personAccountModelpersonAccountID",
                principalTable: "PersonAccounts",
                principalColumn: "personAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_BankInfos_bankInfoModelbankInfoID",
                table: "OtherPayments",
                column: "bankInfoModelbankInfoID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_PersonAccounts_personAccountID",
                table: "OtherPayments",
                column: "personAccountID",
                principalTable: "PersonAccounts",
                principalColumn: "personAccountID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_SubAccounts_subAccountID",
                table: "OtherPayments",
                column: "subAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_BankInfos_bankInfoModelbankInfoID",
                table: "PayrollPays",
                column: "bankInfoModelbankInfoID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Employments_EmploymentModelemploymentID",
                table: "PayrollPays",
                column: "EmploymentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_PersonAccounts_personAccountID",
                table: "PayrollPays",
                column: "personAccountID",
                principalTable: "PersonAccounts",
                principalColumn: "personAccountID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_SubAccounts_subAccountID",
                table: "PayrollPays",
                column: "subAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSitesHistories_Employments_employmentModelemploymentID",
                table: "WorkSitesHistories",
                column: "employmentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankInfos_PersonAccounts_personAccountModelpersonAccountID",
                table: "BankInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_BankInfos_bankInfoModelbankInfoID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_PersonAccounts_personAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_SubAccounts_subAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_BankInfos_bankInfoModelbankInfoID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Employments_EmploymentModelemploymentID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_PersonAccounts_personAccountID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_SubAccounts_subAccountID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSitesHistories_Employments_employmentModelemploymentID",
                table: "WorkSitesHistories");

            migrationBuilder.DropTable(
                name: "PersonAccounts");

            migrationBuilder.DropIndex(
                name: "IX_WorkSitesHistories_employmentModelemploymentID",
                table: "WorkSitesHistories");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_bankInfoModelbankInfoID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_EmploymentModelemploymentID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_subAccountID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_OtherPayments_bankInfoModelbankInfoID",
                table: "OtherPayments");

            migrationBuilder.DropIndex(
                name: "IX_OtherPayments_subAccountID",
                table: "OtherPayments");

            migrationBuilder.DropIndex(
                name: "IX_Employments_givenID",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_BankInfos_personAccountModelpersonAccountID",
                table: "BankInfos");

            migrationBuilder.DropColumn(
                name: "employmentModelemploymentID",
                table: "WorkSitesHistories");

            migrationBuilder.DropColumn(
                name: "EmploymentModelemploymentID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "bankInfoModelbankInfoID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "bankInfoModelbankInfoID",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "personAccountModelpersonAccountID",
                table: "BankInfos");

            migrationBuilder.RenameColumn(
                name: "subAccountID",
                table: "PayrollPays",
                newName: "debitAccountID");

            migrationBuilder.RenameColumn(
                name: "personAccountID",
                table: "PayrollPays",
                newName: "creditAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollPays_personAccountID",
                table: "PayrollPays",
                newName: "IX_PayrollPays_creditAccountID");

            migrationBuilder.RenameColumn(
                name: "subAccountID",
                table: "OtherPayments",
                newName: "debitAccountID");

            migrationBuilder.RenameColumn(
                name: "personAccountID",
                table: "OtherPayments",
                newName: "creditAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_OtherPayments_personAccountID",
                table: "OtherPayments",
                newName: "IX_OtherPayments_creditAccountID");

            migrationBuilder.RenameColumn(
                name: "deductionPriority",
                table: "DeductionTypes",
                newName: "dedcutionPriority");

            migrationBuilder.AlterColumn<string>(
                name: "givenID",
                table: "Employments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSitesHistories_employmentID",
                table: "WorkSitesHistories",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_employmentID",
                table: "PayrollPays",
                column: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_BankInfos_creditAccountID",
                table: "OtherPayments",
                column: "creditAccountID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_SubAccounts_creditAccountID",
                table: "OtherPayments",
                column: "creditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_BankInfos_creditAccountID",
                table: "PayrollPays",
                column: "creditAccountID",
                principalTable: "BankInfos",
                principalColumn: "bankInfoID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Employments_employmentID",
                table: "PayrollPays",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_SubAccounts_creditAccountID",
                table: "PayrollPays",
                column: "creditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSitesHistories_Employments_employmentID",
                table: "WorkSitesHistories",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");
        }
    }
}
