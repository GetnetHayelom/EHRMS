using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "jobGradeMidSalary",
                table: "JobGrades",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Guaranties",
                columns: table => new
                {
                    guarantyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    guarantyStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    guarantyEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    guarantyAmount = table.Column<double>(type: "float", nullable: true),
                    guarantyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    guarantyFor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    guarantyStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guaranties", x => x.guarantyID);
                    table.ForeignKey(
                        name: "FK_Guaranties_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    serviceRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    requestedService = table.Column<int>(type: "int", nullable: false),
                    serviceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.serviceRequestID);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuarantyHistories",
                columns: table => new
                {
                    guarantyHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guarantyID = table.Column<int>(type: "int", nullable: false),
                    guarantyStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuarantyHistories", x => x.guarantyHistoryID);
                    table.ForeignKey(
                        name: "FK_GuarantyHistories_Guaranties_guarantyID",
                        column: x => x.guarantyID,
                        principalTable: "Guaranties",
                        principalColumn: "guarantyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestHistories",
                columns: table => new
                {
                    serviceRequestHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    serviceRequestStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    serviceRequestID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestHistories", x => x.serviceRequestHistoryID);
                    table.ForeignKey(
                        name: "FK_ServiceRequestHistories_ServiceRequests_serviceRequestID",
                        column: x => x.serviceRequestID,
                        principalTable: "ServiceRequests",
                        principalColumn: "serviceRequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guaranties_employmentID",
                table: "Guaranties",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_GuarantyHistories_guarantyID",
                table: "GuarantyHistories",
                column: "guarantyID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestHistories_serviceRequestID",
                table: "ServiceRequestHistories",
                column: "serviceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_employmentID",
                table: "ServiceRequests",
                column: "employmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuarantyHistories");

            migrationBuilder.DropTable(
                name: "ServiceRequestHistories");

            migrationBuilder.DropTable(
                name: "Guaranties");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "jobGradeMidSalary",
                table: "JobGrades");
        }
    }
}
