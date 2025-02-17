using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expriences_JobPlacements_jobPlacementModeljobPlacementID",
                table: "Expriences");

            migrationBuilder.DropForeignKey(
                name: "FK_Expriences_Persons_personModelpersonID",
                table: "Expriences");

            migrationBuilder.AlterColumn<int>(
                name: "personModelpersonID",
                table: "Expriences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "jobPlacementModeljobPlacementID",
                table: "Expriences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "departmentShort",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Expriences_JobPlacements_jobPlacementModeljobPlacementID",
                table: "Expriences",
                column: "jobPlacementModeljobPlacementID",
                principalTable: "JobPlacements",
                principalColumn: "jobPlacementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Expriences_Persons_personModelpersonID",
                table: "Expriences",
                column: "personModelpersonID",
                principalTable: "Persons",
                principalColumn: "personID");
            //migrationBuilder.Sql(@"
            //    CREATE TRIGGER trg_LeaveHistory
            //    ON Leaves
            //    AFTER INSERT, UPDATE
            //    AS
            //    BEGIN
            //    INSERT INTO LeaveHistories(leaveID, leaveHistoryAction)
            //    SELECT leaveID, leaveStatus
            //    FROM Leaves
            //    END;
            //    CREATE TRIGGER trg_WorkSiteHistory
            //    ON workSiteModel
            //    AFTER INSERT, UPDATE
            //    AS
            //    BEGIN
            //    INSERT INTO WorkSiteHistories(workSiteID, workSiteHistoryAction)
            //    SELECT workSiteID, workSiteStatus
            //    FROM workSiteModel
            //    END;
            //    CREATE TRIGGER trg_EmploymentHistory
            //    ON Employments
            //    AFTER INSERT, UPDATE
            //    AS
            //    BEGIN
            //    INSERT INTO EmploymentHistories(employmentID, employmentTypeID)
            //    SELECT employmentID, employmentTypeID
            //    FROM Employments
            //    END; 
            //    "
            //    );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expriences_JobPlacements_jobPlacementModeljobPlacementID",
                table: "Expriences");

            migrationBuilder.DropForeignKey(
                name: "FK_Expriences_Persons_personModelpersonID",
                table: "Expriences");

            migrationBuilder.AlterColumn<int>(
                name: "personModelpersonID",
                table: "Expriences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "jobPlacementModeljobPlacementID",
                table: "Expriences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "departmentShort",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expriences_JobPlacements_jobPlacementModeljobPlacementID",
                table: "Expriences",
                column: "jobPlacementModeljobPlacementID",
                principalTable: "JobPlacements",
                principalColumn: "jobPlacementID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expriences_Persons_personModelpersonID",
                table: "Expriences",
                column: "personModelpersonID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
