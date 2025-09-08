using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           // migrationBuilder.DropTable("employmentMethodHistories");

            migrationBuilder.CreateTable(
                name: "employmentMethodHistories",
                columns: table => new
                {
                    employmentMethodHistoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // ✅ identity
                    employmentMethodID = table.Column<int>(nullable: false),
                    employmentMethodName = table.Column<string>(nullable: true),
                    employmentMethodDescription = table.Column<string>(nullable: true),
                    employmentMethodStatus = table.Column<int>(nullable: false),
                    modifiedDate = table.Column<DateTime>(nullable: false),
                    modifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employmentMethodHistories", x => x.employmentMethodHistoryID);
                });

            //migrationBuilder.DropForeignKey(
            //    name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodHistoryID",
            //    table: "EmploymentMethodHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacementHistories_Departments_departmentID",
                table: "JobPlacementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_Shifts_shiftID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacements_workSiteModel_workSiteID",
                table: "JobPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Employments_EmploymentModelemploymentID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftModelshiftID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_Employments_employmentModelemploymentID",
                table: "SiteAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteModelworkSiteID",
                table: "SiteAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SiteAssignments_employmentModelemploymentID",
                table: "SiteAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SiteAssignments_workSiteModelworkSiteID",
                table: "SiteAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_EmploymentModelemploymentID",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_shiftModelshiftID",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_shiftID",
                table: "JobPlacements");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacements_workSiteID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "employmentModelemploymentID",
                table: "SiteAssignments");

            migrationBuilder.DropColumn(
                name: "workSiteModelworkSiteID",
                table: "SiteAssignments");

            migrationBuilder.DropColumn(
                name: "EmploymentModelemploymentID",
                table: "ShiftAssignments");

            migrationBuilder.DropColumn(
                name: "shiftModelshiftID",
                table: "ShiftAssignments");

            migrationBuilder.DropColumn(
                name: "shiftID",
                table: "JobPlacements");

            migrationBuilder.DropColumn(
                name: "workSiteID",
                table: "JobPlacements");

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "modifiedDate",
            //    table: "EmploymentMethods",
            //    type: "datetime2",
            //    nullable: true,
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime2");

            //migrationBuilder.AlterColumn<int>(
            //    name: "employmentMethodHistoryID",
            //    table: "EmploymentMethodHistories",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modifiedDate",
                table: "Delegations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_SiteAssignments_employmentID",
                table: "SiteAssignments",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteAssignments_workSiteID",
                table: "SiteAssignments",
                column: "workSiteID");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_employmentID",
                table: "ShiftAssignments",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_shiftID",
                table: "ShiftAssignments",
                column: "shiftID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentMethodHistories_employmentMethodID",
                table: "EmploymentMethodHistories",
                column: "employmentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodID",
                table: "EmploymentMethodHistories",
                column: "employmentMethodID",
                principalTable: "EmploymentMethods",
                principalColumn: "employmentMethodID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacementHistories_Departments_departmentID",
                table: "JobPlacementHistories",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Employments_employmentID",
                table: "ShiftAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftID",
                table: "ShiftAssignments",
                column: "shiftID",
                principalTable: "Shifts",
                principalColumn: "shiftID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_Employments_employmentID",
                table: "SiteAssignments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteID",
                table: "SiteAssignments",
                column: "workSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodID",
                table: "EmploymentMethodHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacementHistories_Departments_departmentID",
                table: "JobPlacementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Employments_employmentID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftID",
                table: "ShiftAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_Employments_employmentID",
                table: "SiteAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteID",
                table: "SiteAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SiteAssignments_employmentID",
                table: "SiteAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SiteAssignments_workSiteID",
                table: "SiteAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_employmentID",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ShiftAssignments_shiftID",
                table: "ShiftAssignments");

            migrationBuilder.DropIndex(
                name: "IX_EmploymentMethodHistories_employmentMethodID",
                table: "EmploymentMethodHistories");

            migrationBuilder.AddColumn<int>(
                name: "employmentModelemploymentID",
                table: "SiteAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "workSiteModelworkSiteID",
                table: "SiteAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmploymentModelemploymentID",
                table: "ShiftAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "shiftModelshiftID",
                table: "ShiftAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "shiftID",
                table: "JobPlacements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "workSiteID",
                table: "JobPlacements",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modifiedDate",
                table: "EmploymentMethods",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "employmentMethodHistoryID",
                table: "EmploymentMethodHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modifiedDate",
                table: "Delegations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteAssignments_employmentModelemploymentID",
                table: "SiteAssignments",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteAssignments_workSiteModelworkSiteID",
                table: "SiteAssignments",
                column: "workSiteModelworkSiteID");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmploymentModelemploymentID",
                table: "ShiftAssignments",
                column: "EmploymentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_shiftModelshiftID",
                table: "ShiftAssignments",
                column: "shiftModelshiftID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_shiftID",
                table: "JobPlacements",
                column: "shiftID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacements_workSiteID",
                table: "JobPlacements",
                column: "workSiteID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodHistoryID",
                table: "EmploymentMethodHistories",
                column: "employmentMethodHistoryID",
                principalTable: "EmploymentMethods",
                principalColumn: "employmentMethodID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacementHistories_Departments_departmentID",
                table: "JobPlacementHistories",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_Shifts_shiftID",
                table: "JobPlacements",
                column: "shiftID",
                principalTable: "Shifts",
                principalColumn: "shiftID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacements_workSiteModel_workSiteID",
                table: "JobPlacements",
                column: "workSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Employments_EmploymentModelemploymentID",
                table: "ShiftAssignments",
                column: "EmploymentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Shifts_shiftModelshiftID",
                table: "ShiftAssignments",
                column: "shiftModelshiftID",
                principalTable: "Shifts",
                principalColumn: "shiftID");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_Employments_employmentModelemploymentID",
                table: "SiteAssignments",
                column: "employmentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteAssignments_workSiteModel_workSiteModelworkSiteID",
                table: "SiteAssignments",
                column: "workSiteModelworkSiteID",
                principalTable: "workSiteModel",
                principalColumn: "workSiteID");
        }
    }
}
