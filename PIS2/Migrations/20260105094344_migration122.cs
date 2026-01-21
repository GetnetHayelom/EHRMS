using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration122 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "modifedDate",
                table: "EvaluationValuations",
                newName: "modifiedDate");

            migrationBuilder.RenameColumn(
                name: "modifedDate",
                table: "EvaluationTypes",
                newName: "modifiedDate");

            migrationBuilder.RenameColumn(
                name: "modifedDate",
                table: "EvaluationSubTasks",
                newName: "modifiedDate");

            migrationBuilder.AlterColumn<string>(
                name: "evaluationSubTaskDescription",
                table: "EvaluationSubTasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "EvaluationValuations",
                newName: "modifedDate");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "EvaluationTypes",
                newName: "modifedDate");

            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "EvaluationSubTasks",
                newName: "modifedDate");

            migrationBuilder.AlterColumn<string>(
                name: "evaluationSubTaskDescription",
                table: "EvaluationSubTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
