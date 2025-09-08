using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration63 : Migration
    {
        /// <inheritdoc />
        /// 
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "Delegations",
                columns: table => new
                {
                    delegationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    delegationFrom = table.Column<int>(type: "int", nullable: false),
                    delegationTo = table.Column<int>(type: "int", nullable: false),
                    delegationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delegationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delegationScope = table.Column<int>(type: "int", nullable: false),
                    delegationStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegations", x => x.delegationID);
                    table.ForeignKey(
                        name: "FK_delegations_Employments_delegationFrom",
                        column: x => x.delegationFrom,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                    table.ForeignKey(
                        name: "FK_delegations_Employments_delegationTo",
                        column: x => x.delegationTo,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                });

            

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Employments_employmentID",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Families_employmentID",
                table: "Families");

          

            migrationBuilder.RenameTable(
                name: "delegations",
                newName: "Delegations");

            migrationBuilder.RenameColumn(
                name: "employmentID",
                table: "Families",
                newName: "personID2");

            migrationBuilder.RenameIndex(
                name: "IX_Families_personID_employmentID",
                table: "Families",
                newName: "IX_Families_personID_personID2");

          

            migrationBuilder.AddColumn<int>(
                name: "personStatus",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "personStatus",
                table: "PersonHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "Families",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "personModel2personID",
                table: "Families",
                type: "int",
                nullable: true);

            

            migrationBuilder.CreateTable(
                name: "DelegationHistories",
                columns: table => new
                {
                    delegationHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    delegationID = table.Column<int>(type: "int", nullable: false),
                    delegationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delegationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    delegationStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegationHistories", x => x.delegationHistoryID);
                    table.ForeignKey(
                        name: "FK_DelegationHistories_Delegations_delegationID",
                        column: x => x.delegationID,
                        principalTable: "Delegations",
                        principalColumn: "delegationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyTypes",
                columns: table => new
                {
                    penaltyTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    penaltyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    penaltyTypeStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyTypes", x => x.penaltyTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Penalties",
                columns: table => new
                {
                    penaltyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    penaltyIssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    penaltyReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    penaltyTypeID = table.Column<int>(type: "int", nullable: false),
                    penaltyStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalties", x => x.penaltyID);
                    table.ForeignKey(
                        name: "FK_Penalties_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Penalties_PenaltyTypes_penaltyTypeID",
                        column: x => x.penaltyTypeID,
                        principalTable: "PenaltyTypes",
                        principalColumn: "penaltyTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PenaltyHistories",
                columns: table => new
                {
                    penaltyHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    penaltyID = table.Column<int>(type: "int", nullable: false),
                    penaltyStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyHistories", x => x.penaltyHistoryID);
                    table.ForeignKey(
                        name: "FK_PenaltyHistories_Penalties_penaltyID",
                        column: x => x.penaltyID,
                        principalTable: "Penalties",
                        principalColumn: "penaltyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPlacementHistories_departmentID",
                table: "JobPlacementHistories",
                column: "departmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Families_personModel2personID",
                table: "Families",
                column: "personModel2personID");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationHistories_delegationID",
                table: "DelegationHistories",
                column: "delegationID");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_employmentID",
                table: "Penalties",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_penaltyTypeID",
                table: "Penalties",
                column: "penaltyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyHistories_penaltyID",
                table: "PenaltyHistories",
                column: "penaltyID");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Persons_personModel2personID",
                table: "Families",
                column: "personModel2personID",
                principalTable: "Persons",
                principalColumn: "personID");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPlacementHistories_Departments_departmentID",
                table: "JobPlacementHistories",
                column: "departmentID",
                principalTable: "Departments",
                principalColumn: "departmentID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Persons_personModel2personID",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPlacementHistories_Departments_departmentID",
                table: "JobPlacementHistories");

            migrationBuilder.DropTable(
                name: "DelegationHistories");

            migrationBuilder.DropTable(
                name: "PenaltyHistories");

            migrationBuilder.DropTable(
                name: "Penalties");

            migrationBuilder.DropTable(
                name: "PenaltyTypes");

            migrationBuilder.DropIndex(
                name: "IX_JobPlacementHistories_departmentID",
                table: "JobPlacementHistories");

            migrationBuilder.DropIndex(
                name: "IX_Families_personModel2personID",
                table: "Families");

           

            migrationBuilder.DropColumn(
                name: "personStatus",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "personStatus",
                table: "PersonHistories");

            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "personModel2personID",
                table: "Families");

           

            migrationBuilder.RenameColumn(
                name: "personID2",
                table: "Families",
                newName: "employmentID");

            migrationBuilder.RenameIndex(
                name: "IX_Families_personID_personID2",
                table: "Families",
                newName: "IX_Families_personID_employmentID");

           

           

            migrationBuilder.CreateIndex(
                name: "IX_Families_employmentID",
                table: "Families",
                column: "employmentID");

           

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Employments_employmentID",
                table: "Families",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Persons_personID",
                table: "Families",
                column: "personID",
                principalTable: "Persons",
                principalColumn: "personID");
        }
    }
}
