using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration120 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "evaluationTaskWeight",
                table: "EvaluationTasks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "evaluationSubTaskWeight",
                table: "EvaluationSubTasks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "isPayroll",
                table: "EarningTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "evaluationTaskWeight",
                table: "EvaluationTasks");

            migrationBuilder.DropColumn(
                name: "evaluationSubTaskWeight",
                table: "EvaluationSubTasks");

            migrationBuilder.DropColumn(
                name: "isPayroll",
                table: "EarningTypes");
        }
    }
}
