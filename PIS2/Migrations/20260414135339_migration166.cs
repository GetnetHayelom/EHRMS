using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration166 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "overtimeAmount",
                table: "OvertimeRecords",
                type: "decimal(18,2)",
                nullable: true,
                computedColumnSql: "(DATEDIFF(MINUTE, overtimeRecordStartTime, overtimeRecordEndTime)/60) * overtimeRate * overtimeRecordEmploymentRate",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "overtimeAmount",
                table: "OvertimeRecords");
        }
    }
}
