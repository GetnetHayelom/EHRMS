using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration124 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trainerName",
                table: "TrainingSessions");

            migrationBuilder.AddColumn<int>(
                name: "personID",
                table: "TrainingSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_personID",
                table: "TrainingSessions",
                column: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSessions_Persons_personID",
                table: "TrainingSessions",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSessions_Persons_personID",
                table: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_personID",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "personID",
                table: "TrainingSessions");

            migrationBuilder.AddColumn<string>(
                name: "trainerName",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
