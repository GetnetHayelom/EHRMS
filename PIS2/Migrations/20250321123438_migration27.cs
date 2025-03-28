using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "employmentHistoryUser",
                table: "EmploymentHistories");

            migrationBuilder.AlterColumn<string>(
                name: "mapLink",
                table: "workSiteModel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "employmentMethodID",
                table: "Employments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employmentRequestID",
                table: "Employments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    contractID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.contractID);
                    table.ForeignKey(
                        name: "FK_Contracts_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentMethods",
                columns: table => new
                {
                    employmentMethodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentMethodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentMethodDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentMethodStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentMethods", x => x.employmentMethodID);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentRequests",
                columns: table => new
                {
                    employmentRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    employmentRequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    employmentTypeID = table.Column<int>(type: "int", nullable: false),
                    requiredNo = table.Column<int>(type: "int", nullable: false),
                    requestStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentModelemploymentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentRequests", x => x.employmentRequestID);
                    table.ForeignKey(
                        name: "FK_EmploymentRequests_EmploymentTypes_employmentTypeID",
                        column: x => x.employmentTypeID,
                        principalTable: "EmploymentTypes",
                        principalColumn: "employmentTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentRequests_Employments_employmentModelemploymentID",
                        column: x => x.employmentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_EmploymentRequests_Jobs_jobID",
                        column: x => x.jobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractHistories",
                columns: table => new
                {
                    contractHistotyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contractID = table.Column<int>(type: "int", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    contractHistoryRemark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractHistories", x => x.contractHistotyID);
                    table.ForeignKey(
                        name: "FK_ContractHistories_Contracts_contractID",
                        column: x => x.contractID,
                        principalTable: "Contracts",
                        principalColumn: "contractID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentMethodHistories",
                columns: table => new
                {
                    employmentMethodHistoryID = table.Column<int>(type: "int", nullable: false),
                    employmentMethodID = table.Column<int>(type: "int", nullable: false),
                    employmentMethodName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentMethodDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employmentMethodStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentMethodHistories", x => x.employmentMethodHistoryID);
                    table.ForeignKey(
                        name: "FK_EmploymentMethodHistories_EmploymentMethods_employmentMethodHistoryID",
                        column: x => x.employmentMethodHistoryID,
                        principalTable: "EmploymentMethods",
                        principalColumn: "employmentMethodID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentRequestHistories",
                columns: table => new
                {
                    employmentRequestHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentRequestID = table.Column<int>(type: "int", nullable: false),
                    jobID = table.Column<int>(type: "int", nullable: false),
                    jobModeljobID = table.Column<int>(type: "int", nullable: true),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    employmentType = table.Column<int>(type: "int", nullable: false),
                    employmentTypeModelemploymentTypeID = table.Column<int>(type: "int", nullable: true),
                    requestStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentRequestHistories", x => x.employmentRequestHistoryID);
                    table.ForeignKey(
                        name: "FK_EmploymentRequestHistories_EmploymentRequests_employmentRequestID",
                        column: x => x.employmentRequestID,
                        principalTable: "EmploymentRequests",
                        principalColumn: "employmentRequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentRequestHistories_EmploymentTypes_employmentTypeModelemploymentTypeID",
                        column: x => x.employmentTypeModelemploymentTypeID,
                        principalTable: "EmploymentTypes",
                        principalColumn: "employmentTypeID");
                    table.ForeignKey(
                        name: "FK_EmploymentRequestHistories_Jobs_jobModeljobID",
                        column: x => x.jobModeljobID,
                        principalTable: "Jobs",
                        principalColumn: "jobID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employments_employmentMethodID",
                table: "Employments",
                column: "employmentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_employmentRequestID",
                table: "Employments",
                column: "employmentRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractHistories_contractID",
                table: "ContractHistories",
                column: "contractID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_employmentID",
                table: "Contracts",
                column: "employmentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequestHistories_employmentRequestID",
                table: "EmploymentRequestHistories",
                column: "employmentRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequestHistories_employmentTypeModelemploymentTypeID",
                table: "EmploymentRequestHistories",
                column: "employmentTypeModelemploymentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequestHistories_jobModeljobID",
                table: "EmploymentRequestHistories",
                column: "jobModeljobID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequests_employmentModelemploymentID",
                table: "EmploymentRequests",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequests_employmentTypeID",
                table: "EmploymentRequests",
                column: "employmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentRequests_jobID",
                table: "EmploymentRequests",
                column: "jobID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_EmploymentMethods_employmentMethodID",
                table: "Employments",
                column: "employmentMethodID",
                principalTable: "EmploymentMethods",
                principalColumn: "employmentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_EmploymentRequests_employmentRequestID",
                table: "Employments",
                column: "employmentRequestID",
                principalTable: "EmploymentRequests",
                principalColumn: "employmentRequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_EmploymentMethods_employmentMethodID",
                table: "Employments");

            migrationBuilder.DropForeignKey(
                name: "FK_Employments_EmploymentRequests_employmentRequestID",
                table: "Employments");

            migrationBuilder.DropTable(
                name: "ContractHistories");

            migrationBuilder.DropTable(
                name: "EmploymentMethodHistories");

            migrationBuilder.DropTable(
                name: "EmploymentRequestHistories");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "EmploymentMethods");

            migrationBuilder.DropTable(
                name: "EmploymentRequests");

            migrationBuilder.DropIndex(
                name: "IX_Employments_employmentMethodID",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_Employments_employmentRequestID",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "employmentMethodID",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "employmentRequestID",
                table: "Employments");

            migrationBuilder.AlterColumn<string>(
                name: "mapLink",
                table: "workSiteModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "employmentHistoryUser",
                table: "EmploymentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
