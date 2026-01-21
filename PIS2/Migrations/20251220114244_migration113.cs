using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration113 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachementIDs",
                table: "PersonEducationLevels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachementIDs",
                table: "Experiences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attachements",
                columns: table => new
                {
                    attachmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    filePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fileSize = table.Column<long>(type: "bigint", nullable: false),
                    uploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachements", x => x.attachmentID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachements");

            migrationBuilder.DropColumn(
                name: "AttachementIDs",
                table: "PersonEducationLevels");

            migrationBuilder.DropColumn(
                name: "AttachementIDs",
                table: "Experiences");
        }
    }
}
