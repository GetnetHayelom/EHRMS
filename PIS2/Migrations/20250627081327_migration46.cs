using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "ServiceRequestHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "GuarantyHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "allowanceDuration",
                table: "Allowances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "allowanceAssignmentEndDate",
                table: "AllowanceAssignmentsHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "allowanceAssignmentEndDate",
                table: "AllowanceAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Prohibitions",
                columns: table => new
                {
                    prohibitionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    prohibitionStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionStatus = table.Column<int>(type: "int", nullable: false),
                    prohibitionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prohibitionType = table.Column<int>(type: "int", nullable: false),
                    prohibitionRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prohibitions", x => x.prohibitionID);
                    table.ForeignKey(
                        name: "FK_Prohibitions_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prohibitionHitoryModels",
                columns: table => new
                {
                    prohibitionHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prohibitionID = table.Column<int>(type: "int", nullable: false),
                    prohibitionStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionEnD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    prohibitionStatus = table.Column<int>(type: "int", nullable: false),
                    prohibitionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prohibitionType = table.Column<int>(type: "int", nullable: false),
                    prohibitionRemark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prohibitionHitoryModels", x => x.prohibitionHistoryID);
                    table.ForeignKey(
                        name: "FK_prohibitionHitoryModels_Prohibitions_prohibitionID",
                        column: x => x.prohibitionID,
                        principalTable: "Prohibitions",
                        principalColumn: "prohibitionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prohibitionHitoryModels_prohibitionID",
                table: "prohibitionHitoryModels",
                column: "prohibitionID");

            migrationBuilder.CreateIndex(
                name: "IX_Prohibitions_employmentID",
                table: "Prohibitions",
                column: "employmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prohibitionHitoryModels");

            migrationBuilder.DropTable(
                name: "Prohibitions");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "ServiceRequestHistories");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "GuarantyHistories");

            migrationBuilder.DropColumn(
                name: "allowanceDuration",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "allowanceAssignmentEndDate",
                table: "AllowanceAssignmentsHistories");

            migrationBuilder.DropColumn(
                name: "allowanceAssignmentEndDate",
                table: "AllowanceAssignments");
        }
    }
}
