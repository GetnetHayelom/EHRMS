using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration134 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "noticeApprovedBy",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "noticePostedBy",
                table: "Notices");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Notices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Notices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "noticeStatus",
                table: "Notices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "noticeStatus",
                table: "Notices");

            migrationBuilder.AddColumn<string>(
                name: "noticeApprovedBy",
                table: "Notices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "noticePostedBy",
                table: "Notices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
