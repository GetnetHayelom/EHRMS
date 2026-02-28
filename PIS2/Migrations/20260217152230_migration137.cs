using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration137 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LetterSequences",
                columns: table => new
                {
                    letterSequenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    letterTypePrefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    letterTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    letterGroup = table.Column<int>(type: "int", nullable: false),
                    lastNumber = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterSequences", x => x.letterSequenceId);
                });

            migrationBuilder.CreateTable(
                name: "LetterTypes",
                columns: table => new
                {
                    letterTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    letterTypeName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    letterTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    letterTypeCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    letterTypePrefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterTypes", x => x.letterTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Letters",
                columns: table => new
                {
                    letterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    letterNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    letterGroup = table.Column<int>(type: "int", nullable: false),
                    letterTypeID = table.Column<int>(type: "int", nullable: false),
                    letterSubject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    letterSender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    letterReceiver = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    letterParent = table.Column<int>(type: "int", nullable: true),
                    letterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    letterStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    letterModelletterID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Letters", x => x.letterID);
                    table.ForeignKey(
                        name: "FK_Letters_LetterTypes_letterTypeID",
                        column: x => x.letterTypeID,
                        principalTable: "LetterTypes",
                        principalColumn: "letterTypeID");
                    table.ForeignKey(
                        name: "FK_Letters_Letters_letterModelletterID",
                        column: x => x.letterModelletterID,
                        principalTable: "Letters",
                        principalColumn: "letterID");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Letters_letterTypeID",
                table: "Letters",
                column: "letterTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_LetterTypes_letterTypeCode",
                table: "LetterTypes",
                column: "letterTypeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LetterTypes_letterTypeName",
                table: "LetterTypes",
                column: "letterTypeName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Letters");

            migrationBuilder.DropTable(
                name: "LetterSequences");

            migrationBuilder.DropTable(
                name: "LetterTypes");
        }
    }
}
