using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Persons_personModelpersonID",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_personModelpersonID",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "personModelpersonID",
                table: "Experiences");

            migrationBuilder.RenameColumn(
                name: "modfiedDate",
                table: "PayrollHistories",
                newName: "modifiedDate");

            migrationBuilder.AlterColumn<string>(
                name: "workSiteCode",
                table: "workSiteModel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Accounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_personID",
                table: "Experiences",
                column: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Persons_personID",
                table: "Experiences",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Persons_personID",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_personID",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "PayrollHistories",
                newName: "modfiedDate");

            migrationBuilder.AlterColumn<string>(
                name: "workSiteCode",
                table: "workSiteModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "personModelpersonID",
                table: "Experiences",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_personModelpersonID",
                table: "Experiences",
                column: "personModelpersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Persons_personModelpersonID",
                table: "Experiences",
                column: "personModelpersonID",
                principalTable: "Persons",
                principalColumn: "personID");
        }
    }
}
