using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration119 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "modifedDate",
                table: "EvaluationTasks",
                newName: "modifiedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "modifiedDate",
                table: "EvaluationTasks",
                newName: "modifedDate");
        }
    }
}
