using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration54 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expriences_Persons_personModelpersonID",
                table: "Expriences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Expriences",
                table: "Expriences");

            migrationBuilder.RenameTable(
                name: "Expriences",
                newName: "Experiences");

            migrationBuilder.RenameIndex(
                name: "IX_Expriences_personModelpersonID",
                table: "Experiences",
                newName: "IX_Experiences_personModelpersonID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences",
                column: "exprienceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Persons_personModelpersonID",
                table: "Experiences",
                column: "personModelpersonID",
                principalTable: "Persons",
                principalColumn: "personID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Persons_personModelpersonID",
                table: "Experiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences");

            migrationBuilder.RenameTable(
                name: "Experiences",
                newName: "Expriences");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_personModelpersonID",
                table: "Expriences",
                newName: "IX_Expriences_personModelpersonID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Expriences",
                table: "Expriences",
                column: "exprienceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Expriences_Persons_personModelpersonID",
                table: "Expriences",
                column: "personModelpersonID",
                principalTable: "Persons",
                principalColumn: "personID");
        }
    }
}
