using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration130 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobClassName",
                table: "JobClasses",
                newName: "jobClassName");

            migrationBuilder.RenameColumn(
                name: "JobClassDescription",
                table: "JobClasses",
                newName: "jobClassDescription");

            migrationBuilder.RenameColumn(
                name: "JobClassId",
                table: "JobClasses",
                newName: "jobClassId");

            migrationBuilder.RenameColumn(
                name: "JobClasStatus",
                table: "JobClasses",
                newName: "jobClassStatus");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "EvaluationTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "EvaluationTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "jobClassID",
                table: "EvaluationTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationTypes_jobClassID",
                table: "EvaluationTypes",
                column: "jobClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationTypes_JobClasses_jobClassID",
                table: "EvaluationTypes",
                column: "jobClassID",
                principalTable: "JobClasses",
                principalColumn: "jobClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationTypes_JobClasses_jobClassID",
                table: "EvaluationTypes");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationTypes_jobClassID",
                table: "EvaluationTypes");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "EvaluationTypes");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "EvaluationTypes");

            migrationBuilder.DropColumn(
                name: "jobClassID",
                table: "EvaluationTypes");

            migrationBuilder.RenameColumn(
                name: "jobClassName",
                table: "JobClasses",
                newName: "JobClassName");

            migrationBuilder.RenameColumn(
                name: "jobClassDescription",
                table: "JobClasses",
                newName: "JobClassDescription");

            migrationBuilder.RenameColumn(
                name: "jobClassId",
                table: "JobClasses",
                newName: "JobClassId");

            migrationBuilder.RenameColumn(
                name: "jobClassStatus",
                table: "JobClasses",
                newName: "JobClasStatus");
        }
    }
}
