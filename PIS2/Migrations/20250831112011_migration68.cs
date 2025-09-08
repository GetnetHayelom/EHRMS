using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration68 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Structures",
                columns: table => new
                {
                    structureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departmentID = table.Column<int>(type: "int", nullable: false),
                    departmentModeldepartmentID = table.Column<int>(type: "int", nullable: true),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    jobModeljobID = table.Column<int>(type: "int", nullable: true),
                    requiredNumber = table.Column<int>(type: "int", nullable: false),
                    structureStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Structures", x => x.structureID);
                    table.ForeignKey(
                        name: "FK_Structures_Departments_departmentModeldepartmentID",
                        column: x => x.departmentModeldepartmentID,
                        principalTable: "Departments",
                        principalColumn: "departmentID");
                    table.ForeignKey(
                        name: "FK_Structures_Jobs_jobModeljobID",
                        column: x => x.jobModeljobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID");
                });

            migrationBuilder.CreateTable(
                name: "StructureHistories",
                columns: table => new
                {
                    structureHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    structureID = table.Column<int>(type: "int", nullable: false),
                    requiredNumber = table.Column<int>(type: "int", nullable: false),
                    structureStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureHistories", x => x.structureHistoryID);
                    table.ForeignKey(
                        name: "FK_StructureHistories_Structures_structureID",
                        column: x => x.structureID,
                        principalTable: "Structures",
                        principalColumn: "structureID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StructureHistories_structureID",
                table: "StructureHistories",
                column: "structureID");

            migrationBuilder.CreateIndex(
                name: "IX_Structures_departmentModeldepartmentID",
                table: "Structures",
                column: "departmentModeldepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Structures_jobModeljobID",
                table: "Structures",
                column: "jobModeljobID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StructureHistories");

            migrationBuilder.DropTable(
                name: "Structures");
        }
    }
}
