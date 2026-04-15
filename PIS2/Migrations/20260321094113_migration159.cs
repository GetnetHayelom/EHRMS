using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration159 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankInfos_PersonAccounts_personAccountModelpersonAccountID",
                table: "BankInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Accounts_AccountID",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_SubAccounts_SubAccountID",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_PersonAccounts_personAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_SubAccounts_subAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_PersonAccounts_personAccountID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_SubAccounts_subAccountID",
                table: "PayrollPays");

            migrationBuilder.DropTable(
                name: "PersonAccounts");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_personAccountID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_OtherPayments_personAccountID",
                table: "OtherPayments");

            migrationBuilder.DropIndex(
                name: "IX_BankInfos_personAccountModelpersonAccountID",
                table: "BankInfos");

            migrationBuilder.DropColumn(
                name: "ceiling",
                table: "TaxRates");

            migrationBuilder.DropColumn(
                name: "personAccountID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "personAccountID",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "personAccountModelpersonAccountID",
                table: "BankInfos");

            migrationBuilder.RenameColumn(
                name: "from",
                table: "TaxRates",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "subAccountID",
                table: "PayrollPays",
                newName: "accountModelaccountID");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollPays_subAccountID",
                table: "PayrollPays",
                newName: "IX_PayrollPays_accountModelaccountID");

            migrationBuilder.RenameColumn(
                name: "subAccountID",
                table: "OtherPayments",
                newName: "accountModelaccountID");

            migrationBuilder.RenameIndex(
                name: "IX_OtherPayments_subAccountID",
                table: "OtherPayments",
                newName: "IX_OtherPayments_accountModelaccountID");

            migrationBuilder.RenameColumn(
                name: "SubAccountID",
                table: "JournalEntryLines",
                newName: "subAccountID");

            migrationBuilder.RenameColumn(
                name: "AccountID",
                table: "JournalEntryLines",
                newName: "accountID");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntryLines_SubAccountID",
                table: "JournalEntryLines",
                newName: "IX_JournalEntryLines_subAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntryLines_AccountID",
                table: "JournalEntryLines",
                newName: "IX_JournalEntryLines_accountID");

            migrationBuilder.AddColumn<string>(
                name: "subAccountNumber",
                table: "SubAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "subAccountID",
                table: "Persons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreditAccountID",
                table: "PayrollPays",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DebitAccountID",
                table: "PayrollPays",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreditAccountID",
                table: "OtherPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DebitAccountID",
                table: "OtherPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "subAccountID",
                table: "JournalEntryLines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentAccountID",
                table: "Accounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_subAccountID",
                table: "Persons",
                column: "subAccountID",
                unique: true,
                filter: "[subAccountID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_CreditAccountID",
                table: "PayrollPays",
                column: "CreditAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollPays_DebitAccountID",
                table: "PayrollPays",
                column: "DebitAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_OtherPayments_CreditAccountID",
                table: "OtherPayments",
                column: "CreditAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_OtherPayments_DebitAccountID",
                table: "OtherPayments",
                column: "DebitAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountID",
                table: "Accounts",
                column: "ParentAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountID",
                table: "Accounts",
                column: "ParentAccountID",
                principalTable: "Accounts",
                principalColumn: "accountID");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Accounts_accountID",
                table: "JournalEntryLines",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_SubAccounts_subAccountID",
                table: "JournalEntryLines",
                column: "subAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_Accounts_accountModelaccountID",
                table: "OtherPayments",
                column: "accountModelaccountID",
                principalTable: "Accounts",
                principalColumn: "accountID");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_SubAccounts_CreditAccountID",
                table: "OtherPayments",
                column: "CreditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_SubAccounts_DebitAccountID",
                table: "OtherPayments",
                column: "DebitAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_Accounts_accountModelaccountID",
                table: "PayrollPays",
                column: "accountModelaccountID",
                principalTable: "Accounts",
                principalColumn: "accountID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_SubAccounts_CreditAccountID",
                table: "PayrollPays",
                column: "CreditAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayrollPays_SubAccounts_DebitAccountID",
                table: "PayrollPays",
                column: "DebitAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_SubAccounts_subAccountID",
                table: "Persons",
                column: "subAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_ParentAccountID",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_Accounts_accountID",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_SubAccounts_subAccountID",
                table: "JournalEntryLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_Accounts_accountModelaccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_SubAccounts_CreditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherPayments_SubAccounts_DebitAccountID",
                table: "OtherPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_Accounts_accountModelaccountID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_SubAccounts_CreditAccountID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollPays_SubAccounts_DebitAccountID",
                table: "PayrollPays");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_SubAccounts_subAccountID",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_subAccountID",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_CreditAccountID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_PayrollPays_DebitAccountID",
                table: "PayrollPays");

            migrationBuilder.DropIndex(
                name: "IX_OtherPayments_CreditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropIndex(
                name: "IX_OtherPayments_DebitAccountID",
                table: "OtherPayments");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ParentAccountID",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "subAccountNumber",
                table: "SubAccounts");

            migrationBuilder.DropColumn(
                name: "subAccountID",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CreditAccountID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "DebitAccountID",
                table: "PayrollPays");

            migrationBuilder.DropColumn(
                name: "CreditAccountID",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "DebitAccountID",
                table: "OtherPayments");

            migrationBuilder.DropColumn(
                name: "ParentAccountID",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "TaxRates",
                newName: "from");

            migrationBuilder.RenameColumn(
                name: "accountModelaccountID",
                table: "PayrollPays",
                newName: "subAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_PayrollPays_accountModelaccountID",
                table: "PayrollPays",
                newName: "IX_PayrollPays_subAccountID");

            migrationBuilder.RenameColumn(
                name: "accountModelaccountID",
                table: "OtherPayments",
                newName: "subAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_OtherPayments_accountModelaccountID",
                table: "OtherPayments",
                newName: "IX_OtherPayments_subAccountID");

            migrationBuilder.RenameColumn(
                name: "subAccountID",
                table: "JournalEntryLines",
                newName: "SubAccountID");

            migrationBuilder.RenameColumn(
                name: "accountID",
                table: "JournalEntryLines",
                newName: "AccountID");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntryLines_subAccountID",
                table: "JournalEntryLines",
                newName: "IX_JournalEntryLines_SubAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntryLines_accountID",
                table: "JournalEntryLines",
                newName: "IX_JournalEntryLines_AccountID");

            migrationBuilder.AddColumn<decimal>(
                name: "ceiling",
                table: "TaxRates",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "personAccountID",
                table: "PayrollPays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "personAccountID",
                table: "OtherPayments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubAccountID",
                table: "JournalEntryLines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
                    personID = table.Column<int>(type: "int", nullable: false),
                    personAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_PayrollPays_personAccountID",
                table: "PayrollPays",
                column: "personAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_OtherPayments_personAccountID",
                table: "OtherPayments",
                column: "personAccountID");

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
                name: "FK_JournalEntryLines_Accounts_AccountID",
                table: "JournalEntryLines",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "accountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_SubAccounts_SubAccountID",
                table: "JournalEntryLines",
                column: "SubAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_PersonAccounts_personAccountID",
                table: "OtherPayments",
                column: "personAccountID",
                principalTable: "PersonAccounts",
                principalColumn: "personAccountID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherPayments_SubAccounts_subAccountID",
                table: "OtherPayments",
                column: "subAccountID",
                principalTable: "SubAccounts",
                principalColumn: "subAccountID",
                onDelete: ReferentialAction.SetNull);

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
        }
    }
}
