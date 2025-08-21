using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration62 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prohibitionHitories");

            migrationBuilder.CreateTable(
                name: "prohibitionHistories",
                columns: table => new
                {
                    prohibitionHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prohibitionID = table.Column<int>(type: "int", nullable: false),
                    prohibitionStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionStatus = table.Column<int>(type: "int", nullable: false),
                    prohibitionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prohibitionType = table.Column<int>(type: "int", nullable: false),
                    prohibitionRemark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prohibitionHistories", x => x.prohibitionHistoryID);
                    table.ForeignKey(
                        name: "FK_prohibitionHistories_Prohibitions_prohibitionID",
                        column: x => x.prohibitionID,
                        principalTable: "Prohibitions",
                        principalColumn: "prohibitionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prohibitionHistories_prohibitionID",
                table: "prohibitionHistories",
                column: "prohibitionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prohibitionHistories");

            migrationBuilder.CreateTable(
                name: "prohibitionHitories",
                columns: table => new
                {
                    prohibitionHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prohibitionID = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prohibitionRemark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prohibitionStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionStatus = table.Column<int>(type: "int", nullable: false),
                    prohibitionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prohibitionHitories", x => x.prohibitionHistoryID);
                    table.ForeignKey(
                        name: "FK_prohibitionHitories_Prohibitions_prohibitionID",
                        column: x => x.prohibitionID,
                        principalTable: "Prohibitions",
                        principalColumn: "prohibitionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prohibitionHitories_prohibitionID",
                table: "prohibitionHitories",
                column: "prohibitionID");
        }
    }
}
