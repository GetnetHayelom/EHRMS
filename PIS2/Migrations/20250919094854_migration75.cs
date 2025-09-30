using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration75 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leaves_employmentID_leaveStartDate_leaveEndDate",
                table: "Leaves");

            migrationBuilder.AddColumn<int>(
                name: "businessUnitID",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordID = table.Column<int>(type: "int", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditID);
                });

            migrationBuilder.CreateTable(
                name: "BusinessUnits",
                columns: table => new
                {
                    businessUnitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    businessUnitName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    companyID = table.Column<int>(type: "int", nullable: false),
                    businessUnitAlias = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    employmentID = table.Column<int>(type: "int", nullable: true),
                    employmentModelemploymentID = table.Column<int>(type: "int", nullable: true),
                    addressID = table.Column<int>(type: "int", nullable: true),
                    addressModeladdressID = table.Column<int>(type: "int", nullable: true),
                    businessUnitStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnits", x => x.businessUnitID);
                    table.ForeignKey(
                        name: "FK_BusinessUnits_Addresses_addressModeladdressID",
                        column: x => x.addressModeladdressID,
                        principalTable: "Addresses",
                        principalColumn: "addressID");
                    table.ForeignKey(
                        name: "FK_BusinessUnits_Companies_companyID",
                        column: x => x.companyID,
                        principalTable: "Companies",
                        principalColumn: "companyID");
                    table.ForeignKey(
                        name: "FK_BusinessUnits_Employments_employmentModelemploymentID",
                        column: x => x.employmentModelemploymentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_businessUnitID",
                table: "Departments",
                column: "businessUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnits_addressModeladdressID",
                table: "BusinessUnits",
                column: "addressModeladdressID");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnits_businessUnitAlias",
                table: "BusinessUnits",
                column: "businessUnitAlias",
                unique: true,
                filter: "[businessUnitAlias] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnits_businessUnitName",
                table: "BusinessUnits",
                column: "businessUnitName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnits_companyID",
                table: "BusinessUnits",
                column: "companyID");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnits_employmentModelemploymentID",
                table: "BusinessUnits",
                column: "employmentModelemploymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_BusinessUnits_businessUnitID",
                table: "Departments",
                column: "businessUnitID",
                principalTable: "BusinessUnits",
                principalColumn: "businessUnitID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_BusinessUnits_businessUnitID",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BusinessUnits");

            migrationBuilder.DropIndex(
                name: "IX_Departments_businessUnitID",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "businessUnitID",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_employmentID_leaveStartDate_leaveEndDate",
                table: "Leaves",
                columns: new[] { "employmentID", "leaveStartDate", "leaveEndDate" },
                unique: true);
        }
    }
}
