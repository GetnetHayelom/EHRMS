using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_OvertimeRecords_Departments_departmentID",
            //    table: "OvertimeRecords");

            migrationBuilder.AlterColumn<int>(
                name: "departmentID",
                table: "OvertimeRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Terminations",
                columns: table => new
                {
                    subAccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subAccountDescription = table.Column<int>(type: "int", nullable: false),
                    accountID = table.Column<DateTime>(type: "datetime2", nullable: false),
                    subAccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subAccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminations", x => x.subAccountID);
                    table.ForeignKey(
                        name: "FK_Terminations_Employments_subAccountDescription",
                        column: x => x.subAccountDescription,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Terminations_subAccountDescription",
                table: "Terminations",
                column: "subAccountDescription",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords");

            migrationBuilder.DropTable(
                name: "Terminations");

            migrationBuilder.AlterColumn<int>(
                name: "departmentID",
                table: "OvertimeRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRecords_Departments_departmentID",
                table: "OvertimeRecords",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
