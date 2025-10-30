using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration82 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accesses",
                columns: table => new
                {
                    accessID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<int>(type: "int", nullable: false),
                    userGroups = table.Column<int>(type: "int", nullable: false),
                    companyID = table.Column<int>(type: "int", nullable: true),
                    accessStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesses", x => x.accessID);
                    table.ForeignKey(
                        name: "FK_Accesses_Companies_companyID",
                        column: x => x.companyID,
                        principalTable: "Companies",
                        principalColumn: "companyID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Accesses_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessHistories",
                columns: table => new
                {
                    accessHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accessID = table.Column<int>(type: "int", nullable: false),
                    accessStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessHistories", x => x.accessHistoryID);
                    table.ForeignKey(
                        name: "FK_AccessHistories_Accesses_accessID",
                        column: x => x.accessID,
                        principalTable: "Accesses",
                        principalColumn: "accessID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accesses_companyID",
                table: "Accesses",
                column: "companyID");

            migrationBuilder.CreateIndex(
                name: "IX_Accesses_userID",
                table: "Accesses",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_AccessHistories_accessID",
                table: "AccessHistories",
                column: "accessID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessHistories");

            migrationBuilder.DropTable(
                name: "Accesses");
        }
    }
}
