using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration146 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttachmentPath",
                table: "Notices",
                newName: "noticeSignedBy");

            migrationBuilder.AddColumn<string>(
                name: "noticeTo",
                table: "Notices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "letterSignedBy",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "letterTitle",
                table: "Letters",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "noticeTo",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "letterSignedBy",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "letterTitle",
                table: "Letters");

            migrationBuilder.RenameColumn(
                name: "noticeSignedBy",
                table: "Notices",
                newName: "AttachmentPath");
        }
    }
}
