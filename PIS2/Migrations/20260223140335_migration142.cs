using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration142 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Letters_Letters_letterModelletterID",
                table: "Letters");

            migrationBuilder.DropIndex(
                name: "IX_Letters_letterModelletterID",
                table: "Letters");

            migrationBuilder.DropIndex(
                name: "IX_Letters_letterNumber",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "letterModelletterID",
                table: "Letters");

            migrationBuilder.AddColumn<string>(
                name: "jobPurpose",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Archives",
                columns: table => new
                {
                    archiveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    letterID = table.Column<int>(type: "int", nullable: false),
                    recievedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    archivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archives", x => x.archiveID);
                    table.ForeignKey(
                        name: "FK_Archives_Letters_letterID",
                        column: x => x.letterID,
                        principalTable: "Letters",
                        principalColumn: "letterID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Letters_letterNumber_letterGroup",
                table: "Letters",
                columns: new[] { "letterNumber", "letterGroup" },
                unique: true,
                filter: "[letterNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Letters_letterParent",
                table: "Letters",
                column: "letterParent");

            migrationBuilder.CreateIndex(
                name: "IX_Archives_letterID",
                table: "Archives",
                column: "letterID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Letters_Letters_letterParent",
                table: "Letters",
                column: "letterParent",
                principalTable: "Letters",
                principalColumn: "letterID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Letters_Letters_letterParent",
                table: "Letters");

            migrationBuilder.DropTable(
                name: "Archives");

            migrationBuilder.DropIndex(
                name: "IX_Letters_letterNumber_letterGroup",
                table: "Letters");

            migrationBuilder.DropIndex(
                name: "IX_Letters_letterParent",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "jobPurpose",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "letterModelletterID",
                table: "Letters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Letters_letterModelletterID",
                table: "Letters",
                column: "letterModelletterID");

            migrationBuilder.CreateIndex(
                name: "IX_Letters_letterNumber",
                table: "Letters",
                column: "letterNumber",
                unique: true,
                filter: "[letterNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Letters_Letters_letterModelletterID",
                table: "Letters",
                column: "letterModelletterID",
                principalTable: "Letters",
                principalColumn: "letterID");
        }
    }
}
