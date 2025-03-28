using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "oldBatchNbr",
                table: "OvertimeRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "oldBatchNbr",
                table: "Leaves",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "oldBatchNbr",
                table: "OvertimeRecords");

            migrationBuilder.DropColumn(
                name: "oldBatchNbr",
                table: "Leaves");
        }
    }
}
