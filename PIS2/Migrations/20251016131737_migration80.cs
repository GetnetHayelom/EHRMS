using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Leave_leaveEndDate",
                table: "Leaves");

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    disciplineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    disciplineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    disciplineDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.disciplineID);
                });

            migrationBuilder.CreateTable(
                name: "Notices",
                columns: table => new
                {
                    noticeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    noticeTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    noticeSubTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    noticeContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    noticePostedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    noticeApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    noticeFrom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    noticePriority = table.Column<int>(type: "int", nullable: false),
                    DatePosted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notices", x => x.noticeID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Notices");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Leave_leaveEndDate",
                table: "Leaves",
                sql: "[leaveEndDate]>=[leaveStartDate]");
        }
    }
}
