using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIS2.Migrations
{
    /// <inheritdoc />
    public partial class migration118 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Employments_employmentModelemploymentID",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Employments_employmentModelemploymentID",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Persons_personModel2personID",
                table: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Families_personModel2personID",
                table: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Departments_employmentModelemploymentID",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Companies_employmentModelemploymentID",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "personModel2personID",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "employmentModelemploymentID",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "employmentModelemploymentID",
                table: "Companies");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_employmentModelemploymentID1",
                table: "Departments",
                newName: "IX_Departments_employmentModelemploymentID");

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    evaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employmentID = table.Column<int>(type: "int", nullable: false),
                    evaluationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    evaluationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    evaluationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    evaluationStatus = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.evaluationID);
                    table.ForeignKey(
                        name: "FK_Evaluations_Employments_employmentID",
                        column: x => x.employmentID,
                        principalTable: "Employments",
                        principalColumn: "employmentID");
                });

            migrationBuilder.CreateTable(
                name: "EvaluationTypes",
                columns: table => new
                {
                    evaluationTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    evaluationTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evaluationTypeWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    evaluationTypeStatus = table.Column<int>(type: "int", nullable: false),
                    isFixed = table.Column<bool>(type: "bit", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationTypes", x => x.evaluationTypeID);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationTasks",
                columns: table => new
                {
                    evaluationTaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    evaluationTaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evaluationTypeID = table.Column<int>(type: "int", nullable: false),
                    evaluationTaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationTasks", x => x.evaluationTaskID);
                    table.ForeignKey(
                        name: "FK_EvaluationTasks_EvaluationTypes_evaluationTypeID",
                        column: x => x.evaluationTypeID,
                        principalTable: "EvaluationTypes",
                        principalColumn: "evaluationTypeID");
                });

            migrationBuilder.CreateTable(
                name: "EvaluationSubTasks",
                columns: table => new
                {
                    evaluationSubTaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    evaluationSubTaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evaluationSubTaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    evaluationTaskID = table.Column<int>(type: "int", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationSubTasks", x => x.evaluationSubTaskID);
                    table.ForeignKey(
                        name: "FK_EvaluationSubTasks_EvaluationTasks_evaluationTaskID",
                        column: x => x.evaluationTaskID,
                        principalTable: "EvaluationTasks",
                        principalColumn: "evaluationTaskID");
                });

            migrationBuilder.CreateTable(
                name: "EvaluationValuations",
                columns: table => new
                {
                    evaluationValuation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    evaluationID = table.Column<int>(type: "int", nullable: false),
                    EvaluationModelevaluationID = table.Column<int>(type: "int", nullable: true),
                    evaluationSubTaskID = table.Column<int>(type: "int", nullable: false),
                    timeValuation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    resourceValuation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    performanceValuation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    modifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    modifedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationValuations", x => x.evaluationValuation);
                    table.ForeignKey(
                        name: "FK_EvaluationValuations_EvaluationSubTasks_evaluationSubTaskID",
                        column: x => x.evaluationSubTaskID,
                        principalTable: "EvaluationSubTasks",
                        principalColumn: "evaluationSubTaskID");
                    table.ForeignKey(
                        name: "FK_EvaluationValuations_Evaluations_EvaluationModelevaluationID",
                        column: x => x.EvaluationModelevaluationID,
                        principalTable: "Evaluations",
                        principalColumn: "evaluationID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Families_personID2",
                table: "Families",
                column: "personID2");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_employmentID",
                table: "Companies",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_employmentID",
                table: "Evaluations",
                column: "employmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationSubTasks_evaluationTaskID",
                table: "EvaluationSubTasks",
                column: "evaluationTaskID");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationTasks_evaluationTypeID",
                table: "EvaluationTasks",
                column: "evaluationTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationValuations_EvaluationModelevaluationID",
                table: "EvaluationValuations",
                column: "EvaluationModelevaluationID");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationValuations_evaluationSubTaskID",
                table: "EvaluationValuations",
                column: "evaluationSubTaskID");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Employments_employmentID",
                table: "Companies",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Employments_employmentID",
                table: "Departments",
                column: "employmentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Persons_personID2",
                table: "Families",
                column: "personID2",
                principalTable: "Persons",
                principalColumn: "personID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Employments_employmentID",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Employments_employmentID",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Persons_personID2",
                table: "Families");

            migrationBuilder.DropTable(
                name: "EvaluationValuations");

            migrationBuilder.DropTable(
                name: "EvaluationSubTasks");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "EvaluationTasks");

            migrationBuilder.DropTable(
                name: "EvaluationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Families_personID2",
                table: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Companies_employmentID",
                table: "Companies");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_employmentModelemploymentID",
                table: "Departments",
                newName: "IX_Departments_employmentModelemploymentID1");

            migrationBuilder.AddColumn<int>(
                name: "personModel2personID",
                table: "Families",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employmentModelemploymentID",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employmentModelemploymentID",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Families_personModel2personID",
                table: "Families",
                column: "personModel2personID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_employmentModelemploymentID",
                table: "Departments",
                column: "employmentModelemploymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_employmentModelemploymentID",
                table: "Companies",
                column: "employmentModelemploymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Employments_employmentModelemploymentID",
                table: "Companies",
                column: "employmentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Employments_employmentModelemploymentID",
                table: "Departments",
                column: "employmentModelemploymentID",
                principalTable: "Employments",
                principalColumn: "employmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Persons_personModel2personID",
                table: "Families",
                column: "personModel2personID",
                principalTable: "Persons",
                principalColumn: "personID");
        }
    }
}
