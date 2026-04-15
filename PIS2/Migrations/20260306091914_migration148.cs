using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration148 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_employmentID_requestedService",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "requestedService",
                table: "ServiceRequests",
                newName: "serviceRequestTypeID");

            migrationBuilder.CreateTable(
                name: "ServiceRequestTypes",
                columns: table => new
                {
                    serviceRequestTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    serviceRequestTypeName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    serviceRequestTypeStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestTypes", x => x.serviceRequestTypeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_employmentID_serviceRequestID",
                table: "ServiceRequests",
                columns: new[] { "employmentID", "serviceRequestID" },
                filter: "serviceRequestStatus =1");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_serviceRequestTypeID",
                table: "ServiceRequests",
                column: "serviceRequestTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestTypes_serviceRequestTypeName",
                table: "ServiceRequestTypes",
                column: "serviceRequestTypeName");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ServiceRequestTypes_serviceRequestTypeID",
                table: "ServiceRequests",
                column: "serviceRequestTypeID",
                principalTable: "ServiceRequestTypes",
                principalColumn: "serviceRequestTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ServiceRequestTypes_serviceRequestTypeID",
                table: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "ServiceRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_employmentID_serviceRequestID",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_serviceRequestTypeID",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "serviceRequestTypeID",
                table: "ServiceRequests",
                newName: "requestedService");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_employmentID_requestedService",
                table: "ServiceRequests",
                columns: new[] { "employmentID", "requestedService" },
                filter: "serviceRequestStatus =1");
        }
    }
}
