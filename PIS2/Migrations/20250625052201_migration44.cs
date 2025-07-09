using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "serviceRequestID",
                table: "Guaranties",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "serviceRequestModelserviceRequestID",
                table: "Guaranties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guaranties_serviceRequestModelserviceRequestID",
                table: "Guaranties",
                column: "serviceRequestModelserviceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Guaranties_ServiceRequests_serviceRequestModelserviceRequestID",
                table: "Guaranties",
                column: "serviceRequestModelserviceRequestID",
                principalTable: "ServiceRequests",
                principalColumn: "serviceRequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guaranties_ServiceRequests_serviceRequestModelserviceRequestID",
                table: "Guaranties");

            migrationBuilder.DropIndex(
                name: "IX_Guaranties_serviceRequestModelserviceRequestID",
                table: "Guaranties");

            migrationBuilder.DropColumn(
                name: "serviceRequestID",
                table: "Guaranties");

            migrationBuilder.DropColumn(
                name: "serviceRequestModelserviceRequestID",
                table: "Guaranties");
        }
    }
}
