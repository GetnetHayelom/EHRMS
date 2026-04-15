using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration158 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeEffDate",
                table: "JobPlacements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeEffDate",
                table: "JobPlacementHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "accountID",
                table: "EarningTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "accountID",
                table: "DeductionTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "accountType",
                table: "Accounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    JournalEntryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.JournalEntryID);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLines",
                columns: table => new
                {
                    JournalEntryLineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryID = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    SubAccountID = table.Column<int>(type: "int", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLines", x => x.JournalEntryLineID);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_JournalEntries_JournalEntryID",
                        column: x => x.JournalEntryID,
                        principalTable: "JournalEntries",
                        principalColumn: "JournalEntryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_SubAccounts_SubAccountID",
                        column: x => x.SubAccountID,
                        principalTable: "SubAccounts",
                        principalColumn: "subAccountID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EarningTypes_accountID",
                table: "EarningTypes",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_DeductionTypes_accountID",
                table: "DeductionTypes",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountID",
                table: "JournalEntryLines",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_JournalEntryID",
                table: "JournalEntryLines",
                column: "JournalEntryID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_SubAccountID",
                table: "JournalEntryLines",
                column: "SubAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionTypes_Accounts_accountID",
                table: "DeductionTypes",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID");

            migrationBuilder.AddForeignKey(
                name: "FK_EarningTypes_Accounts_accountID",
                table: "EarningTypes",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeductionTypes_Accounts_accountID",
                table: "DeductionTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_EarningTypes_Accounts_accountID",
                table: "EarningTypes");

            migrationBuilder.DropTable(
                name: "JournalEntryLines");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_EarningTypes_accountID",
                table: "EarningTypes");

            migrationBuilder.DropIndex(
                name: "IX_DeductionTypes_accountID",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "ChangeEffDate",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "ChangeEffDate",
                table: "JobPlacementHistories");

            migrationBuilder.DropColumn(
                name: "accountID",
                table: "EarningTypes");

            migrationBuilder.DropColumn(
                name: "accountID",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(
                name: "accountType",
                table: "Accounts");
        }
    }
}
