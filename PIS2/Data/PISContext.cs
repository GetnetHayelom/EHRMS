using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using PIS2.Models;
using PIS2.Models.Finance;
using PIS2.Models.Foundation;
using PIS2.Models.HR;
using PIS2.Models.Organization;
using PIS2.Pages.Report;
using PIS2.Views;


namespace PIS2.Data
{
    public class PISContext : IdentityDbContext<userModel, IdentityRole<int>, int>
    {
        public PISContext(DbContextOptions options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Automatically register all entity types in the Assembly
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(PISContext).Assembly);

            foreach (var property in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

            modelBuilder.Entity<accessModel>(entity =>
            {
                entity.HasKey(a => a.accessID);
                entity.HasOne(a => a.userModel).WithMany(u => u.Accesses).HasForeignKey(a => a.userID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(a => a.CompanyModel).WithMany(c => c.Accesses).HasForeignKey(a => a.companyID).OnDelete(DeleteBehavior.SetNull);
                entity.Property(a => a.modifiedBy).IsRequired().HasMaxLength(100);

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });

            ///summary
            /// accessHistoryModel configuration
            /// ------------------------------
            modelBuilder.Entity<accessHistoryModel>(entity =>
            {
                entity.HasKey(h => h.accessHistoryID);

                entity.HasOne(h => h.accessModel).WithMany(a => a.AccessHistories).HasForeignKey(h => h.accessID).OnDelete(DeleteBehavior.NoAction);
                entity.Property(h => h.modifiedBy).IsRequired().HasMaxLength(100);
                
            });


            modelBuilder.Entity<accountModel>(entity =>
            {
                entity.HasMany(e => e.SubAccounts).WithOne(p => p.accountModel).HasForeignKey(e => e.accountID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.accountNumber).IsUnique();
                entity.HasIndex(e => e.accountName).IsUnique();

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<addressModel>(entity =>
            {
                entity.HasMany(e => e.Persons).WithOne(p => p.addressModel).HasForeignKey(p => p.addressID);
                entity.HasMany(e => e.WorkSites).WithOne(p => p.addressModel).HasForeignKey(p => p.addressID);
                entity.HasMany(e => e.Companies).WithOne(p => p.addressModel).HasForeignKey(p => p.addressID);

                entity.HasIndex(e => new { e.addressCountry, e.addressRegion, e.addressZone, e.addressWoreda, e.addressTabya }).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<allowanceModel>(entity =>
            {
                entity.HasMany(e => e.AllowanceAssignments).WithOne(p => p.allowanceModel).HasForeignKey(e => e.allowanceID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(a => a.EarningType).WithMany(e => e.Allowances).HasForeignKey(e => e.earningTypeID).OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.allowanceName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<allowanceAssignmentModel>(entity =>
            {
                entity.HasIndex(e => e.allowanceID, "IX_AllowanceAssignments_allowanceModelallowanceID");
                entity.Property(e => e.allowanceAssignmentID).HasColumnName("allowanceAssignmentID");
                entity.Property(e => e.allowanceID).HasColumnName("allowanceID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.allowanceAssignmentAmount).HasColumnName("allowanceAssignmentAmount");
                entity.Property(e => e.allowanceStatus).HasColumnName("allowanceStatus");
                entity.Property(e => e.allowanceAssignmentEndDate).HasColumnName("allowanceAssignmentEndDate").IsRequired(false);
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.allowanceModel).WithMany(p => p.AllowanceAssignments).HasForeignKey(d => d.allowanceID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.AllowanceAssignmentHistories).WithOne(p => p.allowanceAssignmentModel).HasForeignKey(p => p.allowanceAssignmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(a => a.employmentModel).WithMany(e => e.AllowanceAssignments).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);


                entity.HasIndex(d => new { d.allowanceID, d.employmentID, d.allowanceStatus }).IsUnique().HasFilter("[allowanceStatus] = 1");

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<allowanceAssignmentHistoryModel>(entity =>
            {
                entity.Property(e => e.allowanceAssignmentHistoryID).HasColumnName("allowanceAssignmentHistoryID");
                entity.Property(e => e.allowanceAssignmentID).HasColumnName("allowanceAssignmentID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.allowanceAssignmentHistoryStatus).HasColumnName("allowanceAssignmentHistoryStatus");
                entity.Property(e => e.allowanceAssignmentAmount).HasColumnName("allowanceAssignmentAmount");
                entity.Property(e => e.allowanceAssignmentEndDate).HasColumnName("allowanceAssignmentEndDate").IsRequired(false);
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.allowanceAssignmentModel).WithMany(d => d.AllowanceAssignmentHistories).HasForeignKey(e => e.allowanceAssignmentID).OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<ApplicantModel>(e =>
            {
                e.HasOne(e => e.VacancyModel).WithMany(v => v.Applicants).HasForeignKey(v => v.VacancyID);
                e.HasOne(e => e.personModel).WithMany(v => v.Applicants).HasForeignKey(v => v.personID);

                e.ToTable(t => t.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<archiveModel>(entity =>
            {
                entity.HasKey(a => a.archiveID);
                // Relationship: Archive has one Letter
                entity.HasOne(a => a.Letter).WithOne(l => l.Archive).HasForeignKey<archiveModel>(a => a.letterID).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<AttachmentModel>(entity =>
            {
                entity.ToTable(t => t.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {});

            modelBuilder.Entity<bankInfoModel>(entity =>
            {
                entity.HasIndex(e => e.personID, "IX_BankInfos_personModelpersonID");
                entity.HasOne(d => d.personModel).WithMany(p => p.Banks).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<breakModel>(entity =>
            {
                entity.HasIndex(e => e.shiftID, "IX_Breaks_shiftModelshiftID");

                entity.HasOne(d => d.shiftModel).WithMany(p => p.Breaks).HasForeignKey(d => d.shiftID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new { d.breakEnd, d.breakStart, d.shiftID }).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<businessUnitModel>(entity =>
            {
                entity.HasMany(e => e.Departments).WithOne(p => p.businessUnitModel).HasForeignKey(p => p.businessUnitID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.companyModel).WithMany(p => p.BusinessUnits).HasForeignKey(p => p.companyID).OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(e => e.businessUnitName).IsUnique();
                entity.HasIndex(e => e.businessUnitAlias).IsUnique();

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<companyModel>(entity =>
            {
                entity.HasMany(e => e.Departments).WithOne(p => p.companyModel).HasForeignKey(p => p.companyID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.BusinessUnits).WithOne(p => p.companyModel).HasForeignKey(p => p.companyID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.addressModel).WithMany(p => p.Companies).HasForeignKey(p => p.addressID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.employmentModel).WithMany(p => p.companyModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.companyName).IsUnique();

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<contractModel>(entity =>
            {
                entity.Property(e => e.contractID).HasColumnName("contractID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.startDate).HasColumnName("startDate");
                entity.Property(e => e.endDate).HasColumnName("endDate");
                entity.Property(e => e.contractRemark).HasColumnName("contractRemark");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(c => c.ContractHistories).WithOne(ch => ch.contractModel).HasForeignKey(p => p.contractID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(c => c.employmentModel).WithOne(e => e.contractModel).HasForeignKey<contractModel>(p => p.employmentID).OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(t => t.UseSqlOutputClause(false));
               
            });

            modelBuilder.Entity<contractHistoryModel>(entity =>
            {
                entity.Property(e => e.contractHistotyID).HasColumnName("contractHistotyID");
                entity.Property(e => e.contractID).HasColumnName("contractID");
                entity.Property(e => e.startDate).HasColumnName("startDate");
                entity.Property(e => e.endDate).HasColumnName("endDate");
                entity.Property(e => e.contractHistoryRemark).HasColumnName("contractHistoryRemark");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(ch => ch.contractModel).WithMany(c => c.ContractHistories).HasForeignKey(p => p.contractID).OnDelete(DeleteBehavior.NoAction);
                
            });
            modelBuilder.Entity<delegationModel>(entity =>
            {
                entity.Property(e => e.delegationID).HasColumnName("delegationID");
                entity.Property(e => e.delegationEndDate).HasColumnName("delegationEndDate");
                entity.Property(e => e.delegationStartDate).HasColumnName("delegationStartDate");
                entity.Property(e => e.delegationFrom).HasColumnName("delegationFrom");
                entity.Property(e => e.delegationTo).HasColumnName("delegationTo");
                entity.Property(e => e.delegationScope).HasColumnName("delegationScope");
                entity.Property(e => e.delegationStatus).HasColumnName("delegationStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.FromEmployment).WithMany(e => e.delegationsFrom).HasForeignKey(e => e.delegationFrom).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.ToEmployment).WithMany(e => e.delegationsTo).HasForeignKey(e => e.delegationTo).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.DelegationHistories).WithOne(d => d.delegationModel).HasForeignKey(d => d.delegationID).OnDelete(DeleteBehavior.NoAction);
                entity.ToTable(tb => tb.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<delegationHistoryModel>(entity =>
            {
                entity.Property(e => e.delegationHistoryID).HasColumnName("delegationHistoryID");
                entity.Property(e => e.delegationID).HasColumnName("delegationID");
                entity.Property(e => e.delegationEndDate).HasColumnName("delegationEndDate");
                entity.Property(e => e.delegationStartDate).HasColumnName("delegationStartDate");
                entity.Property(e => e.delegationStatus).HasColumnName("delegationStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.delegationModel).WithMany(e => e.DelegationHistories).HasForeignKey(e => e.delegationID).OnDelete(DeleteBehavior.NoAction);
               
            });
            modelBuilder.Entity<departmentModel>(entity =>
            {
                entity.HasIndex(e => e.employmentID, "IX_Departments_employmentModelemploymentID");

                entity.HasIndex(e => e.subAccountID, "IX_Departments_subAccountModelsubAccountID");

                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.companyID).HasColumnName("companyID");
                entity.Property(e => e.departmentName).HasColumnName("departmentName");
                entity.Property(e => e.departmentShort).HasColumnName("departmentShort");
                entity.Property(e => e.departmentStatus).HasColumnName("departmentStatus");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.subAccountID).HasColumnName("subAccountID");
                entity.Property(e => e.businessUnitID).HasColumnName("businessUnitID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.companyModel).WithMany(p => p.Departments).HasForeignKey(d => d.companyID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.businessUnitModel).WithMany(p => p.Departments).HasForeignKey(d => d.businessUnitID).OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.employmentModel).WithMany(p => p.departmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.subAccountModel).WithMany(p => p.Departments).HasForeignKey(d => d.subAccountID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e =>e.JobPlacements).WithOne(p => p.departmentModel).HasForeignKey(d => d.departmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.OvertimeRecords).WithOne(or => or.departmentModel).HasForeignKey(d => d.departmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.businessUnitModel).WithMany(p => p.Departments).HasForeignKey(d => d.businessUnitID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => new { e.companyID, e.departmentName}).IsUnique();

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<departmentHistoryModel>(entity =>
            {
                entity.Property(e => e.departmentHistoryID).HasColumnName("departmentHistoryID");
                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.departmentName).HasColumnName("departmentName");
                entity.Property(e => e.departmentStatus).HasColumnName("departmentStatus");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.departmentModel).WithMany(e => e.DepartmentHistories).HasForeignKey(d => d.departmentID).OnDelete(DeleteBehavior.NoAction);
                
            });

            modelBuilder.Entity<disciplineModel>(entity =>
            {
                entity.ToTable("Disciplines");
                entity.Property(e => e.disciplineName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });


            modelBuilder.Entity<educationLevelModel>(entity =>
            {
                entity.HasIndex(e => e.educationLevelName, "IX_EducationLevels_personModelpersonID").IsUnique();

                entity.HasMany(d => d.PersonEducationLevels).WithOne(p => p.educationLevelModel).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.Jobs).WithMany(p => p.EducationLevels);

                entity.HasIndex(d => d.educationLevelName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<employmentModel>(entity =>
            {

                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.employmentDate).HasColumnName("employmentDate");
                entity.Property(e => e.employmentReference).HasColumnName("employmentReference");
                entity.Property(e => e.employmentStatus).HasColumnName("employmentStatus");
                entity.Property(e => e.employmentTypeID).HasColumnName("employmentTypeID");
                entity.Property(e => e.givenID).HasColumnName("givenID");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.workingHoursPerWeek).HasColumnName("workingHoursPerWeek");
                entity.Property(e => e.employmentMethodID).HasColumnName("employmentMethodID");
                entity.Property(e => e.jobRequirementID).HasColumnName("jobRequirementID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.employmentPosition).HasColumnName("employmentPosition");

                entity.HasOne(d => d.personModel).WithMany(p => p.Employments).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.EmploymentHistories).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.OvertimeRecords).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.JobPlacements).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.Leaves).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.AllowanceAssignments).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.LoyaltyHistories).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.WorkSites).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.employmentMethodModel).WithMany(em => em.Employments).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.JobRequirementModel).WithMany(er => er.Employments).HasForeignKey(d => d.jobRequirementID).OnDelete(DeleteBehavior.NoAction);
                
                entity.HasOne(d => d.employmentTypeModel).WithMany(e => e.Employments).HasForeignKey(d => d.employmentTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(d => d.personID).HasFilter("[employmentStatus] = 1").IsUnique();
                entity.HasIndex(d => d.givenID).IsUnique();

                entity.ToTable(tb => tb.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<employmentHistoryModel>(entity =>
            {
                entity.Property(e => e.employmentHistoryID).HasColumnName("employmentHistoryID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.employmentTypeID).HasColumnName("employmentTypeID");
                entity.Property(e => e.employmentHistoryRemark).HasColumnName("employmentrHistoryRemark");
                entity.Property(e => e.employmentPosition).HasColumnName("employmentPosition");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.employmentModel).WithMany(d => d.EmploymentHistories).HasForeignKey(p => p.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.employmentTypeModel).WithMany(d => d.EmploymentHistories).HasForeignKey(p => p.employmentID).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<employmentMethodModel>(entity =>
            {
                entity.Property(e => e.employmentMethodID).HasColumnName("employmentMethodID");
                entity.Property(e => e.employmentMethodName).HasColumnName("employmentMethodName");
                entity.Property(e => e.employmentMethodDescription).HasColumnName("employmentMethodDescription");
                entity.Property(e => e.employmentMethodStatus).HasColumnName("employmentMethodStatus");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(em => em.Employments).WithOne(e => e.employmentMethodModel).HasForeignKey(p => p.employmentMethodID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(em => em.EmploymentMethodHistories).WithOne(emh => emh.employmentMethodModel).HasForeignKey(p => p.employmentMethodID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => d.employmentMethodName).IsUnique();

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<employmentMethodHistoryModel>(entity =>
            {
                entity.Property(e => e.employmentMethodHistoryID).HasColumnName("employmentMethodHistoryID");
                entity.Property(e => e.employmentMethodID).HasColumnName("employmentMethodID");
                entity.Property(e => e.employmentMethodName).HasColumnName("employmentMethodName");
                entity.Property(e => e.employmentMethodDescription).HasColumnName("employmentMethodDescription");
                entity.Property(e => e.employmentMethodStatus).HasColumnName("employmentMethodStatus");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(emh => emh.employmentMethodModel).WithMany(em => em.EmploymentMethodHistories).HasForeignKey(p => p.employmentMethodID).OnDelete(DeleteBehavior.NoAction);
            });
           
            modelBuilder.Entity<employmentTypeModel>(entity =>
            {
                entity.HasMany(e => e.EmploymentHistories).WithOne(p => p.employmentTypeModel).HasForeignKey(p => p.employmentTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.Employments).WithOne(p => p.employmentTypeModel).HasForeignKey(p => p.employmentTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(et => et.JobRequirements).WithOne(er => er.employmentTypeModel).HasForeignKey(p =>p.employmentTypeID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.employmentTypeName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<evaluationTypeModel>(e =>
            {
                e.HasOne(e => e.jobClassModel).WithMany(j => j.EvaluationTypes).HasForeignKey(e => e.jobClassID).OnDelete(DeleteBehavior.NoAction);
                e.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<evaluationModel>(e =>
            {
                e.HasOne(e => e.EmploymentModel)
                    .WithMany(p => p.Evaluations) 
                    .HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(e => e.JobPlacementModel).WithMany(j => j.Evaluations).HasForeignKey(e => e.jobPlacementID).OnDelete(DeleteBehavior.NoAction);

                e.ToTable(tb => tb.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<evaluationTaskModel>(e =>
            {
                e.HasOne(e => e.EvaluationTypeModel)
                    .WithMany(p => p.EvaluationTasks) 
                    .HasForeignKey(e => e.evaluationTypeID).OnDelete(DeleteBehavior.NoAction);

                e.HasMany(e => e.EvaluationSubTasks)
                    .WithOne(p => p.EvaluationTaskModel) 
                    .HasForeignKey(e => e.evaluationTaskID).OnDelete(DeleteBehavior.NoAction);

                e.ToTable(tb => tb.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<evaluationSubTaskModel>(e =>
            {
                e.HasOne(e => e.EvaluationTaskModel)
                    .WithMany(p => p.EvaluationSubTasks)
                    .HasForeignKey(e => e.evaluationTaskID).OnDelete(DeleteBehavior.NoAction);

                e.HasMany(e => e.EvaluationValuations)
                    .WithOne(p => p.EvaluationSubTaskModel)
                    .HasForeignKey(e => e.evaluationSubTaskID).OnDelete(DeleteBehavior.NoAction);

                e.ToTable(tb => tb.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<evaluationValuationModel>(e =>
            {
                e.HasOne(e => e.EvaluationSubTaskModel)
                    .WithMany(p => p.EvaluationValuations)
                    .HasForeignKey(e => e.evaluationSubTaskID).OnDelete(DeleteBehavior.NoAction);

                e.ToTable(tb => tb.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<experienceModel>(e =>
            {
                e.HasOne(e => e.personModel)
                    .WithMany(p => p.Experiences) // optional if person has a collection of experiences
                    .HasForeignKey(e => e.personID).OnDelete(DeleteBehavior.NoAction);

                e.ToTable(tb => tb.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<familyModel>(entity =>
            {
                entity.HasOne(e => e.personModel).WithMany(e => e.Families1).HasForeignKey(e => e.personID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.personModel2).WithMany(e => e.Families2).HasForeignKey(e => e.personID2).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => new { e.personID, e.personID2 }).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<holidayModel>(entity =>
            {
                entity.HasIndex(e => e.holidayName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<guarantyModel>(entity =>
            {
                entity.Property(e => e.guarantyID).HasColumnName("guarantyID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.guarantyStartDate).HasColumnName("guarantyStartDate");
                entity.Property(e => e.guarantyEndDate).HasColumnName("guarantyEndDate");
                entity.Property(e => e.guarantyAmount).HasColumnName("guarantyAmount");
                entity.Property(e => e.guarantyType).HasColumnName("guarantyType");
                entity.Property(e => e.guarantyFor).HasColumnName("guarantyFor");
                entity.Property(e => e.guarantyStatus).HasColumnName("guarantyStatus");
                entity.Property(e => e.guarantyBeneficiary).HasColumnName("guarantyBeneficiary");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.serviceRequestID).HasColumnName("serviceRequestID");

                entity.HasMany(e => e.GuarantyHistories).WithOne(g => g.guarantyModel).HasForeignKey(g => g.guarantyID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(g => g.Employment).WithMany(e => e.Guaranties).HasForeignKey(g => g.employmentID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new { d.employmentID, d.guarantyType, d.guarantyStatus }).HasFilter("guarantyStatus = 1").IsUnique();

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<guarantyHistoryModel>(entity => 
            {
                entity.Property(e => e.guarantyHistoryID).HasColumnName("guarantyHistoryID");
                entity.Property(e => e.guarantyID).HasColumnName("guarantyID");
                entity.Property(e => e.guarantyStatus).HasColumnName("guarantyStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(g => g.guarantyModel).WithMany(e => e.GuarantyHistories).HasForeignKey(g => g.guarantyID).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<jobClassModel>(entity =>
            {
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<jobModel>(entity =>
            {

                entity.HasIndex(e => e.jobCategoryID, "IX_Jobs_jobCategoryModeljobCategoryID");

                entity.HasIndex(e => e.jobClassID, "IX_Jobs_jobClassModelJobClassId");

                entity.HasOne(d => d.jobCategoryModel).WithMany(p => p.Jobs).HasForeignKey(p => p.jobCategoryID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.jobClassModel).WithMany(p => p.Jobs).HasForeignKey(d => d.jobClassID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.EducationLevels).WithMany(p => p.Jobs);
                entity.HasOne(d => d.jobGradeModel).WithMany(e => e.Jobs).HasForeignKey(p => p.jobGradeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.jobGradeModelMax).WithMany(e => e.JobsMax).HasForeignKey(p => p.jobGradeIDMax).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(j => j.JobRequirements).WithOne(er => er.JobModel).HasForeignKey(p => p.jobID).OnDelete(DeleteBehavior.NoAction);
                
                entity.HasIndex(d => d.jobCode).IsUnique().HasFilter("[jobCode] IS NOT NULL");
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<jobGradeModel>(entity =>
            {
                entity.HasMany(e => e.Jobs).WithOne(d => d.jobGradeModel).HasForeignKey(d => d.jobGradeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(g => g.NextJobGrade).WithOne().HasForeignKey<jobGradeModel>(g => g.NextJobGradeID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(g => g.PreviousJobGrade).WithOne().HasForeignKey<jobGradeModel>(g => g.PreviousJobGradeID).OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.jobGradeName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<jobCategoryModel>(entity =>
            {
                entity.HasIndex(e => e.jobCategoryName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<jobPlacementModel>(entity =>
            {
                entity.HasIndex(e => e.departmentID, "IX_JobPlacements_departmentModeldepartmentID");

                entity.HasIndex(e => e.employmentID, "IX_JobPlacements_employmentModelemploymentID");

                entity.HasIndex(e => e.jobID, "IX_JobPlacements_jobModeljobID");

                entity.Property(e => e.jobPlacementID).HasColumnName("jobPlacementID");
                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.jobPlacementDate).HasColumnName("jobPlacementDate");
                entity.Property(e => e.jobPlacementReference).HasColumnName("jobPlacementReference");
                entity.Property(e => e.jobPlacementCareer).HasColumnName("jobPlacementCareer");
                entity.Property(e => e.jobPlacementSalary).HasColumnType("decimal(18, 2)").HasColumnName("jobPlacementSalary");
                entity.Property(e => e.jobPlacementStatus).HasColumnName("jobPlacementStatus");
                entity.Property(e => e.jobPlacementReason).HasColumnName("jobPlacementReason");
                entity.Property(e => e.jobStepID).HasColumnName("jobStepID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.departmentModel).WithMany(p => p.JobPlacements).HasForeignKey(d => d.departmentID).OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.employmentModel).WithMany(p => p.JobPlacements).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                    

                entity.HasOne(d => d.jobModel).WithMany(p => p.JobPlacements).HasForeignKey(d => d.jobID);
                entity.HasOne(e => e.jobStepModel).WithMany(e => e.JobPlacements).HasForeignKey(e => e.jobStepID).OnDelete(DeleteBehavior.NoAction);
                
                entity.HasIndex(d => d.employmentID).HasFilter("[jobPlacementStatus]=1").IsUnique();

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<jobPlacementHistoryModel>(entity =>
            {
                entity.Property(e => e.jobPlacementHistoryID).HasColumnName("jobPlacementHistoryID");
                entity.Property(e => e.jobPlacementID).HasColumnName("jobPlacementID");
                entity.Property(e => e.jobStepID).HasColumnName("jobStepID");
                entity.Property(e => e.jobPlacementReference).HasColumnName("jobPlacementReference");
                entity.Property(e => e.jobPlacementSalary).HasColumnName("jobPlacementSalary");
                entity.Property(e => e.jobPlacementStatus).HasColumnName("jobPlacementStatus");
                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.jobPlacementReason).HasColumnName("jobPlacementReason");
                entity.Property(e => e.jobPlacementDate).HasColumnName("jobPlacementDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.jobPlacementModel).WithMany(e => e.JobPlacementHistories).HasForeignKey(e => e.jobPlacementID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.jobStepModel).WithMany(e => e.JobPlacementHistories).HasForeignKey(e => e.jobStepID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.departmentModel).WithMany(e => e.JobPlacementHistories).HasForeignKey(e => e.departmentID).OnDelete(DeleteBehavior.NoAction); 
            });
            modelBuilder.Entity<jobRequirementModel>(entity =>
            {
                entity.Property(e => e.jobRequirementID).HasColumnName("jobRequirementID");
                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.requiredNumber).HasColumnName("requiredNumber");
                entity.Property(e => e.approvedNumber).HasColumnName("approvedNumber");
                entity.Property(e => e.hiredNumber).HasColumnName("hiredNumber");
                entity.Property(e => e.jobRequirementStatus).HasColumnName("jobRequirementStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.JobRequirementHistories).WithOne(p => p.JobRequirementModel).HasForeignKey(p => p.jobRequirementID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(jr => jr.DepartmentModel).WithMany(d => d.JobRequirements).HasForeignKey(jr => jr.departmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(jr => jr.JobModel).WithMany(j => j.JobRequirements).HasForeignKey(j => j.jobID).OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(t => t.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<jobRequirementHistoryModel>(entity =>
            {
                entity.Property(e => e.jobRequirementHistoryID).HasColumnName("jobRequirementHistoryID");
                entity.Property(e => e.jobRequirementID).HasColumnName("jobRequirementID");                
                entity.Property(e => e.requiredNumber).HasColumnName("requiredNumber");
                entity.Property(e => e.jobRequirementStatus).HasColumnName("jobRequirementStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(jr => jr.JobRequirementModel).WithMany(jr => jr.JobRequirementHistories).HasForeignKey(jr => jr.jobRequirementID).OnDelete(DeleteBehavior.NoAction);
                
            });

      

            modelBuilder.Entity<jobReqCost>(entity =>
            {
                
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<jobStepModel>(entity =>
            {
                entity.Property(e => e.jobStepID).HasColumnName("jobStepID");
                entity.Property(e => e.jobStepName).HasColumnName("jobStepName");
                entity.Property(e => e.jobStepNumber).HasColumnName("jobStepNumber");
                entity.Property(e => e.jobGradeID).HasColumnName("jobGradeID");
                entity.Property(e => e.jobStepSalary).HasColumnName("jobStepSalary");
                entity.Property(e => e.jobStepStatus).HasColumnName("jobStepStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.JobStepHistories).WithOne(e => e.jobStepModel).HasForeignKey(e => e.jobStepID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.jobGradeModel).WithMany(e => e.JobSteps).HasForeignKey(e => e.jobGradeID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new { d.jobStepName }).IsUnique();
                entity.HasIndex(d => new { d.jobGradeID, d.jobStepNumber, d.jobStepStatus }).IsUnique();

                entity.ToTable(tb => tb.UseSqlOutputClause(false));


            });
            modelBuilder.Entity<jobStepHistoryModel>(entity =>
            {
                entity.Property(e => e.jobStepHistoryID).HasColumnName("jobStepHistoryID");
                entity.Property(e => e.jobStepID).HasColumnName("jobStepID");
                entity.Property(e => e.jobStepName).HasColumnName("jobStepName");
                entity.Property(e => e.jobStepSalary).HasColumnName("jobStepSalary");
                entity.Property(e => e.jobStepStatus).HasColumnName("jobStepStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.jobStepModel).WithMany(e => e.JobStepHistories).HasForeignKey(e => e.jobStepID).OnDelete(DeleteBehavior.NoAction);

            });
            modelBuilder.Entity<leaveModel>(entity =>
            {
                entity.HasKey(e => e.leaveID);

                entity.HasIndex(e => e.employmentID, "IX_Leaves_employmentModelemploymentID");

                entity.HasIndex(e => e.leaveTypeID, "IX_Leaves_leaveTypeModelleaveTypeID");

                entity.Property(e => e.leaveID).HasColumnName("leaveID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.leaveDays).HasColumnName("leaveDays");
                entity.Property(e => e.leaveEndDate).HasColumnName("leaveEndDate");
                entity.Property(e => e.leaveRequestDate).HasColumnName("leaveRequestDate");
                entity.Property(e => e.leaveStartDate).HasColumnName("leaveStartDate");
                entity.Property(e => e.leaveStatus).HasColumnName("leaveStatus");
                entity.Property(e => e.leaveTypeID).HasColumnName("leaveTypeID");
                entity.Property(e => e.oldBatchNbr).HasColumnName("oldBatchNbr");
                entity.Property(e => e.ratePerHour).HasColumnName("ratePerHour");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                
                entity.HasOne(d => d.employmentModel).WithMany(p => p.Leaves).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.leaveTypeModel).WithMany(p => p.Leaves).HasForeignKey(d => d.leaveTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.LeaveHistories).WithOne(p => p.leaveModel).HasForeignKey(d => d.leaveID).OnDelete(DeleteBehavior.NoAction);
 

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<leaveHistoryModel>(entity =>
            {
                entity.Property(e => e.leaveHistoryID).HasColumnName("leaveHistoryID");
                entity.Property(e => e.leaveID).HasColumnName("leaveID");
                entity.Property(e => e.leaveHistoryAction).HasColumnName("leaveHistoryAction");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.leaveModel).WithMany(p => p.LeaveHistories).HasForeignKey(e => e.leaveID).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<leaveTypeModel>(entity =>
            {
                entity.HasMany(p => p.Leaves).WithOne(e => e.leaveTypeModel).HasForeignKey(p => p.leaveTypeID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.leaveTypeName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<letterModel>(entity =>
            {
                entity.HasOne(p => p.LetterType).WithMany(e => e.Letters).HasForeignKey(p => p.letterTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.LetterParent).WithMany(p => p.ChildLetters).HasForeignKey(d => d.letterParent).OnDelete(DeleteBehavior.Restrict);
                
                entity.HasIndex(e => new { e.letterNumber, e.letterGroup }).IsUnique().HasFilter("[letterNumber] IS NOT NULL");
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<letterTypeModel>(entity =>
            {
                entity.HasIndex(e => e.letterTypeCode).IsUnique();
                entity.HasIndex(e => e.letterTypeName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<LetterSequence>(entity =>
            { });
                modelBuilder.Entity<loyaltyModel>(entity =>
            {
                entity.HasMany(e => e.LoyaltyHistories).WithOne(d => d.loyaltyModel).HasForeignKey(e => e.loyaltyID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e =>e.loyaltyName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<loyaltyHistoryModel>(entity =>
            {
                entity.Property(e => e.loyaltyHistoryID).HasColumnName("loyaltyHistoryID");
                entity.Property(e => e.loyaltyID).HasColumnName("loyaltyID");
                entity.Property(e => e.loyaltyAmount).HasColumnName("loyaltyAmount");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.loyaltyHistoryStatus).HasColumnName("loyaltyHistoryStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.employmentModel).WithMany(d => d.LoyaltyHistories).HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.loyaltyModel).WithMany(p => p.LoyaltyHistories).HasForeignKey(e => e.loyaltyID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new {d.loyaltyID, d.employmentID}).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });



            modelBuilder.Entity<NoticeModel>(entity =>
            {
                entity.ToTable("Notices");

                entity.HasKey(e => e.noticeID);

                entity.Property(e => e.noticeID)
                    .HasColumnName("noticeID");

                entity.Property(e => e.noticeTitle)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("noticeTitle");

                entity.Property(e => e.noticeContent)
                    .IsRequired()
                    .HasColumnName("noticeContent");

                entity.Property(e => e.noticeStatus)
                    .HasColumnName("noticeStatus");

                entity.Property(e => e.DatePosted)
                    .HasColumnType("datetime")
                    .HasColumnName("DatePosted")
                    .HasDefaultValueSql("GETDATE()"); // DB default

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("ExpiryDate");

                entity.Property(e => e.IsActive)
                    .HasColumnName("IsActive")
                    .HasDefaultValue(true);

                entity.Property(e => e.noticeSignedBy)
                    .HasMaxLength(255)
                    .HasColumnName("noticeSignedBy");
                entity.ToTable(tb => tb.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<overtimeModel>(entity =>
            {
                entity.HasMany(e => e.OvertimeRecords).WithOne(p => p.overtimeModel).HasForeignKey(d => d.overtimeID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.overtimeName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<overtimeRecordModel>(entity =>
            {
                entity.HasIndex(e => e.employmentID, "IX_OvertimeRecords_employmentModelemploymentID");

                entity.HasIndex(e => e.overtimeID, "IX_OvertimeRecords_overtimeModelovertimeID");

                entity.Property(e => e.overtimeRecordID).HasColumnName("overtimeRecordID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.overtimeID).HasColumnName("overtimeID");
                entity.Property(e => e.overtimeRecordDate).HasColumnName("overtimeRecordDate");
                entity.Property(e => e.overtimeRecordEndTime).HasColumnName("overtimeRecordEndTime");
                entity.Property(e => e.overtimeRecordStartTime).HasColumnName("overtimeRecordStartTime");
                entity.Property(e => e.overtimeRecordStatus).HasColumnName("overtimeRecordStatus");
                entity.Property(e => e.overtimeRecordReason).HasColumnName("overtimeRecordReason");
                entity.Property(e => e.overtimeRecordEmploymentRate).HasColumnName("overtimeRecordEmploymentRate");
                entity.Property(e => e.overtimeRate).HasColumnName("overtimeRate");
                entity.Property(e => e.overtimeAmount).HasColumnName("overtimeAmount").HasComputedColumnSql("(DATEDIFF(MINUTE, overtimeRecordStartTime, overtimeRecordEndTime)/60) * overtimeRate * overtimeRecordEmploymentRate", stored:true);
                entity.Property(e => e.oldBatchNbr).HasColumnName("oldBatchNbr");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.employmentModel).WithMany(p => p.OvertimeRecords).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.OvertimeHistories).WithOne(p => p.overtimeRecordModel).HasForeignKey(d => d.overtimeRecordID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.overtimeModel).WithMany(p => p.OvertimeRecords).HasForeignKey(d => d.overtimeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(or => or.departmentModel).WithMany(d => d.OvertimeRecords).HasForeignKey(d => d.departmentID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new { d.employmentID, d.overtimeRecordDate, d.overtimeRecordStartTime, d.overtimeRecordEndTime}).IsUnique();
                
                entity.ToTable(tb => tb.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<overtimeHistoryModel>(entity =>
            {
                entity.Property(e => e.overtimeHistoryID).HasColumnName("overtimeHistoryID");
                entity.Property(e => e.overtimeRecordID).HasColumnName("overtimeRecordID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.overtimeHistoryAction).HasColumnName("overtimeHistoryAction");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.overtimeRecordModel).WithMany(p => p.OvertimeHistories).HasForeignKey(d => d.overtimeRecordID).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<penaltyModel>(entity => {
               
                entity.HasOne(e => e.employmentModel).WithMany(e => e.Penalties).HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.departmentModel).WithMany(e => e.Penalties).HasForeignKey(e => e.departmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.penaltyTypeModel).WithMany(e => e.Penalties).HasForeignKey(e => e.penaltyTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.PenaltyHistories).WithOne(e => e.penaltyModel).HasForeignKey(e => e.penaltyID).OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(t => t.UseSqlOutputClause(false));
               
            });
            modelBuilder.Entity<penaltyHistoryModel>();
            modelBuilder.Entity<penaltyTypeModel>(entity =>
            {
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            
            modelBuilder.Entity<personModel>(entity =>
            {
                entity.HasIndex(e => e.addressID, "IX_Persons_addressModeladdressID");

                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.personDoB).HasColumnName("personDoB");
                entity.Property(e => e.personEmailAddress).HasColumnName("personEmailAddress");
                entity.Property(e => e.personFatherName)
                    .HasMaxLength(100)
                    .HasColumnName("personFatherName");
                entity.Property(e => e.personFirstName)
                    .HasMaxLength(100)
                    .HasColumnName("personFirstName");
                entity.Property(e => e.personGender).HasColumnName("personGender");
                entity.Property(e => e.personIDNumber).HasColumnName("personIDNumber");
                entity.Property(e => e.personIDType).HasColumnName("personIDType");
                entity.Property(e => e.personLastName)
                    .HasMaxLength(100)
                    .HasColumnName("personLastName");
                entity.Property(e => e.personPhoneNumber).HasColumnName("personPhoneNumber");
                entity.Property(e => e.personRecordNumber).HasColumnName("personRecordNumber");
                entity.Property(e => e.personStatus).HasColumnName("personStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.addressModel).WithMany(p => p.Persons).HasForeignKey(d => d.addressID).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasMany(d => d.Employments).WithOne(p => p.personModel).HasForeignKey(f => f.personID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.PersonEducationLevels).WithOne(p => p.personModel).HasForeignKey(f => f.personID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e =>e.Banks).WithOne(p => p.personModel).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.SubAccount).WithOne(p => p.Person).HasForeignKey<personModel>(p => p.subAccountID).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.personIDType, e.personIDNumber }).IsUnique();

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<personHistoryModel>(entity =>
            {
                entity.Property(e => e.personHistoryID).HasColumnName("personHistoryID");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.personDoB).HasColumnName("personDoB");
                entity.Property(e => e.personEmailAddress).HasColumnName("personEmailAddress");
                entity.Property(e => e.personFatherName).HasColumnName("personFatherName");
                entity.Property(e => e.personFirstName).HasColumnName("personFirstName");
                entity.Property(e => e.personGender).HasColumnName("personGender");
                entity.Property(e => e.personIDNumber).HasColumnName("personIDNumber");
                entity.Property(e => e.personIDType).HasColumnName("personIDType");
                entity.Property(e => e.personLastName).HasColumnName("personLastName");
                entity.Property(e => e.personPhoneNumber).HasColumnName("personPhoneNumber");
                entity.Property(e => e.personRecordNumber).HasColumnName("personRecordNumber");
                entity.Property(e => e.personStatus).HasColumnName("personStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(d => d.personModel).WithMany(p => p.PersonHistories).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.addressModel).WithMany(p => p.PersonHistories).HasForeignKey(d => d.addressID).OnDelete(DeleteBehavior.NoAction);
                
            });

            modelBuilder.Entity<personEducationLevelModel>(entity =>
            {
                entity.HasIndex(e => e.educationLevelID, "IX_PersonEducationLevels_educationLevelModeleducationLevelID");

                entity.HasIndex(e => e.personID, "IX_PersonEducationLevels_personModelpersonID");
               
                entity.HasOne(d => d.educationLevelModel).WithMany(p => p.PersonEducationLevels).HasForeignKey(d => d.educationLevelID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.personModel).WithMany(e => e.PersonEducationLevels).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new { d.educationLevelID, d.personID, d.educationField}).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<prohibitionModel>(e =>
            {
                e.Property(e => e.prohibitionID).HasColumnName("prohibitionID");
                e.Property(e => e.employmentID).HasColumnName("employmentID");
                e.Property(e => e.prohibitionStart).HasColumnName("prohibitionStart");
                e.Property(e => e.prohibitionEnd).HasColumnName("prohibitionEnd");
                e.Property(e => e.prohibitionStatus).HasColumnName("prohibitionStatus");
                e.Property(e => e.prohibitionReason).HasColumnName("prohibitionReason");
                e.Property(e => e.prohibitionType).HasColumnName("prohibitionType");
                e.Property(e => e.prohibitionRemark).HasColumnName("prohibitionRemark");
                e.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                e.HasMany(p => p.ProhibitionHistories).WithOne(p => p.prohibitionModel).HasForeignKey(p => p.prohibitionID).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(p => p.employmentModel).WithMany(p => p.Prohibitions).HasForeignKey(p => p.employmentID).OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(d => new { d.employmentID, d.prohibitionStart, d.prohibitionEnd, d.prohibitionType }).HasFilter("prohibitionStatus=1").IsUnique();

                e.ToTable(t => t.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<prohibitionHistoryModel>(e =>
            {
                e.Property(e => e.prohibitionHistoryID).HasColumnName("prohibitionHistoryID");
                e.Property(e => e.prohibitionID).HasColumnName("prohibitionID");                
                e.Property(e => e.prohibitionStart).HasColumnName("prohibitionStart");
                e.Property(e => e.prohibitionEnd).HasColumnName("prohibitionEnd");
                e.Property(e => e.prohibitionStatus).HasColumnName("prohibitionStatus");
                e.Property(e => e.prohibitionReason).HasColumnName("prohibitionReason");
                e.Property(e => e.prohibitionType).HasColumnName("prohibitionType");
                e.Property(e => e.prohibitionRemark).HasColumnName("prohibitionRemark");
                e.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                e.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                e.HasOne(p => p.prohibitionModel).WithMany(p => p.ProhibitionHistories).HasForeignKey(p => p.prohibitionID).OnDelete(DeleteBehavior.NoAction);               

            });

            modelBuilder.Entity<serviceRequestModel>(entity =>
            {
                entity.HasOne(e => e.Employment).WithMany(e => e.ServiceRequests).HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.ServiceRequestHistoies).WithOne(e => e.ServiceRequest).HasForeignKey(e => e.serviceRequestID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new { d.employmentID, d.serviceRequestID }).HasFilter("serviceRequestStatus =1");

                entity.ToTable(t => t.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<serviceRequestHistoryModel>(entity =>
            {
                entity.Property(e => e.serviceRequestHistoryID).HasColumnName("serviceRequestHistoryID");
                entity.Property(e => e.serviceRequestID).HasColumnName("serviceRequestID");
                entity.Property(e => e.serviceRequestStatus).HasColumnName("serviceRequestStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.ServiceRequest).WithMany(e => e.ServiceRequestHistoies).HasForeignKey(e => e.serviceRequestID).OnDelete(DeleteBehavior.NoAction);

            });
            modelBuilder.Entity<serviceRequestTypeModel>(entity =>
            {
                entity.HasMany(e => e.ServiceRequests).WithOne(e => e.ServiceRequestType).HasForeignKey(e => e.serviceRequestTypeID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(d => new { d.serviceRequestTypeName });

                entity.ToTable(t => t.UseSqlOutputClause(false));
                 
            });
            modelBuilder.Entity<shiftModel>(entity =>
            {
                entity.HasMany(e => e.Breaks).WithOne(p => p.shiftModel).HasForeignKey(e => e.shiftID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.shiftName).IsUnique();
                entity.HasIndex(e => new { e.shiftStart, e.shiftEnd }).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<shiftAssignmentModel>(entity =>
            {
                entity.Property(e => e.shiftAssignmentID).HasColumnName("shiftAssignmentID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.shiftID).HasColumnName("shiftID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.shiftModel).WithMany(e => e.ShiftAssignments).HasForeignKey(e => e.shiftID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.EmploymentModel).WithMany(e => e.ShiftAssignments).HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.NoAction);


            });
            modelBuilder.Entity<siteAssignmentModel>(entity =>
            {
                entity.Property(e => e.siteAssignmentID).HasColumnName("siteAssignmentID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.workSiteID).HasColumnName("workSiteID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.workSiteModel).WithMany(e => e.SiteAssignments).HasForeignKey(e => e.workSiteID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.employmentModel).WithMany(e => e.SiteAssignments).HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<structureModel>(entity =>
            {
                entity.Property(e => e.structureID).HasColumnName("structureID");
                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.requiredNumber).HasColumnName("requiredNumber");
                entity.Property(e => e.structureStatus).HasColumnName("structureStatus");
                entity.Property(e => e.reportsTo).HasColumnName("reportsTo");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.StructureHistories).WithOne(e => e.structureModel).HasForeignKey(e => e.structureID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.departmentModel).WithMany(e => e.Structures).HasForeignKey(e => e.departmentID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.jobModel).WithMany(e => e.Structures).HasForeignKey(e => e.jobID).OnDelete(DeleteBehavior.NoAction);
                
                entity.HasOne(s => s.ReportsTo).WithMany(s => s.Subordinates).HasForeignKey(s => s.reportsTo).OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(t => t.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<structureHistoryModel>(entity =>
            {
                entity.HasOne(e => e.structureModel).WithMany(e => e.StructureHistories).HasForeignKey(e => e.structureID).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<subAccountModel>(entity =>
            {
                entity.HasIndex(e => e.subAccountID, "IX_SubAccounts_accountModelaccountID");

                entity.HasOne(d => d.accountModel).WithMany(p => p.SubAccounts).HasForeignKey(d => d.accountID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.Departments).WithOne(p => p.subAccountModel).HasForeignKey(d => d.subAccountID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.WorkSites).WithOne(p => p.subAccountModel).HasForeignKey(d => d.subAccountID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.subAccountName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<terminationModel>(entity =>
            {
                entity.HasOne(t => t.EmploymentModel).WithOne(e => e.TerminationModel).HasForeignKey<terminationModel>(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.employmentID).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<userModel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.userStatus).HasColumnName("userStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.personModel).WithOne(p => p.userModel).HasForeignKey<userModel>(d => d.personID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.UserHistories).WithOne(u => u.userModel).HasForeignKey(d => d.Id).OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<userHistoryModel>(entity =>
            {
                entity.Property(e => e.userHistoryID).HasColumnName("userHistoryID");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.userName).HasColumnName("userName");
                entity.Property(e => e.userStatus).HasColumnName("userStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.userModel).WithMany(u => u.UserHistories).HasForeignKey(d => d.Id).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<VacancyModel>(e =>
            {
                e.HasOne(v => v.departmentModel).WithMany(d => d.Vacancies).HasForeignKey(d => d.departmentID);
                e.HasOne(v => v.jobModel).WithMany(d => d.Vacancies).HasForeignKey(d => d.jobID);
                e.HasOne(v => v.jobRequirmentmodel).WithMany(d => d.Vacancies).HasForeignKey(d => d.jobRequirementID);
                e.HasOne(v => v.employmentMethodModel).WithMany(d => d.Vacancies).HasForeignKey(d => d.employmentMethodID);
                e.HasOne(v => v.employmentTypeModel).WithMany(d => d.Vacancies).HasForeignKey(d => d.employmentTypeID);
                e.HasMany(v => v.Applicants).WithOne(d => d.VacancyModel).HasForeignKey(d => d.VacancyID);
                e.HasMany(v => v.JobReqCost).WithOne(d => d.VacancyModel).HasForeignKey(d => d.VacancyID);

                e.ToTable(t => t.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<workSiteModel>(entity =>
            {
                entity.HasKey(e => e.workSiteID);

                entity.ToTable("workSiteModel");

                entity.HasIndex(e => e.addressID, "IX_workSiteModel_AddressModeladdressID");

                entity.HasOne(d => d.addressModel).WithMany(p => p.WorkSites).HasForeignKey(d => d.addressID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(d => d.WorkSiteHistories).WithOne(p => p.workSiteModel).HasForeignKey(d => d.workSiteID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.employmentModel).WithMany(e => e.WorkSites).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                
                entity.HasIndex(d => d.workSiteName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<workSiteHistoryModel>(entity =>
            {
                entity.HasOne(e => e.workSiteModel).WithMany(p => p.WorkSiteHistories).HasForeignKey(d => d.workSiteID).OnDelete(DeleteBehavior.NoAction);
            });
            //
            /// <summary>
            /// Payroll related
            /// </summary>
            // payrollModel
            modelBuilder.Entity<payrollModel>(entity =>
            {
                entity.HasMany(e => e.payrollHistories).WithOne(p => p.payrollModel).HasForeignKey(p => p.payrollID).OnDelete(DeleteBehavior.NoAction);
            });

            // payrollHistory
            modelBuilder.Entity<payrollHistory>(entity =>
            {
                entity.Property(e => e.payrollHistoryID).HasColumnName("payrollHistoryID");
                entity.Property(e => e.payrollID).HasColumnName("payrollID");
                entity.Property(e => e.payrollStatus).HasColumnName("payrollStatus");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.payrollModel).WithMany(p => p.payrollHistories).HasForeignKey(e => e.payrollID).OnDelete(DeleteBehavior.NoAction);
            });

            // payrollPay
            modelBuilder.Entity<payrollPay>(entity =>
            {
                entity.HasOne(e => e.payrollModel).WithMany(p =>p.PayrollPays).HasForeignKey(e => e.payrollID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.DeductionRecords).WithOne(d => d.PayrollPay).HasForeignKey(e => e.payrollPayID).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.EarningRecords).WithOne(d => d.PayrollPay).HasForeignKey(e => e.payrollPayID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.CreditAccount).WithMany(p => p.CPayrollPays).HasForeignKey(e => e.CreditAccountID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.DebitAccount).WithMany(p => p.DPayrollPays).HasForeignKey(e => e.DebitAccountID).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Department).WithMany(p => p.PayrollPays).HasForeignKey(e => e.departmentID).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.JobPlacement).WithMany(p => p.PayrollPays).HasForeignKey(e => e.jobPlacementID).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.EmploymentModel).WithMany(p => p.PayrollPays).HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            //earningModel
            modelBuilder.Entity<earningModel>(e=> 
            {
                e.ToTable(tb => tb.UseSqlOutputClause(false));
                e.HasOne(e => e.EmploymentModel).WithMany(e => e.Earnings).HasForeignKey(e => e.employmentID);
            });
            

            //earningModel
            modelBuilder.Entity<earningHistoryModel>();
            // earningRecordModel
            modelBuilder.Entity<earningRecordModel>(entity =>
            {
                entity.HasOne(e => e.earningType).WithMany(p => p.Earnings).HasForeignKey(e => e.earningTypeID).OnDelete(DeleteBehavior.NoAction);
                // PAYROLL PAY -> DEDUCTIONS
                entity.HasOne(d => d.PayrollPay).WithMany(p => p.EarningRecords).HasForeignKey(d => d.payrollPayID).OnDelete(DeleteBehavior.NoAction);
            });

            // earningType
            modelBuilder.Entity<earningType>(entity =>
            {
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
                
            });
            //deductionModel
            modelBuilder.Entity<deductionModel>(e =>
            {
                e.ToTable(tb => tb.UseSqlOutputClause(false));
                e.HasOne(e => e.EmploymentModel).WithMany(e => e.Deductions).HasForeignKey(e => e.employmentID);
            });
            modelBuilder.Entity<deductionHistoryModel>();
            // deductionRecordModel
            modelBuilder.Entity<deductionRecordModel>(entity =>
            {
                entity.HasOne(e => e.DeductionType).WithMany(p => p.Deductions).HasForeignKey(e => e.deductionTypeID).OnDelete(DeleteBehavior.NoAction);
               // PAYROLL PAY -> DEDUCTIONS
                entity.HasOne(d => d.PayrollPay).WithMany(p => p.DeductionRecords).HasForeignKey(d => d.payrollPayID).OnDelete(DeleteBehavior.NoAction);
            });

            // deductionType
            modelBuilder.Entity<deductionType>(entity =>
            {
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });

            //tax rates
            modelBuilder.Entity<taxRateModel>(entity =>
            {
                entity.ToTable("TaxRates");
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            // BUDGET PLAN
            modelBuilder.Entity<BudgetPlan>(entity =>
            {
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            // BUDGET PLAN
            modelBuilder.Entity<BudgetLine>(entity =>
            {
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });            

            //account Journal
            modelBuilder.Entity<JournalEntry>()
                .HasMany(j => j.Lines)
                .WithOne(l => l.JournalEntry)
                .HasForeignKey(l => l.JournalEntryID);

            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(l => l.Account)
                .WithMany()
                .HasForeignKey(l => l.accountID);

            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(l => l.SubAccount)
                .WithMany()
                .HasForeignKey(l => l.subAccountID);

            ///<summary>
            ///TRAINING MODULE
            /// </summary>
            /// 
            //TRAINING
            modelBuilder.Entity<trainingModel>(entity =>
            {
                entity.ToTable("Trainings", tb => tb.UseSqlOutputClause(false));

                entity.HasMany(t => t.TrainingSessions).WithOne(r => r.Training).HasForeignKey(r => r.trainingID);
                
            });

            //TRAINING SESSION
            modelBuilder.Entity<trainingSessionModel>(entity =>
            {
                entity.ToTable("TrainingSessions",tb => tb.UseSqlOutputClause(false));
                entity.HasMany(t => t.Attendances).WithOne(r => r.TrainingSession).HasForeignKey(r => r.trainingSessionID);
                entity.HasOne(t => t.PersonModel).WithMany(p => p.TrainingSessions).HasForeignKey(r => r.personID).OnDelete(DeleteBehavior.NoAction);
                
            });

            //TRAINING ATTENDANCE
            modelBuilder.Entity<trainingAttendanceModel>(entity =>
            {
                entity.ToTable("TrainingAttendances",tb => tb.UseSqlOutputClause(false));
                entity.HasOne(t => t.EmploymentModel).WithMany(r => r.TrainingAttendaces).HasForeignKey(r => r.employmentID);
                
            });

            //TRAINING COST ALLOCATION
            modelBuilder.Entity<trainingCostAllocationModel>(entity =>
            {
                entity.ToTable("TrainingCostAllocations",tb => tb.UseSqlOutputClause(false));
                
            });

            /// <summary>
            /// VIEWS
            ///</summary>
            // Leave Report View
            modelBuilder.Entity<LeaveReportView>()
            .HasNoKey()
            .ToView("vw_LeaveReport");


            ///<summary>
            ///ABSENTISM PER COMPANY
            /// </summary>
            /// 
            modelBuilder.Entity<LeaveReportCompany>()
                .HasNoKey()
                .ToView("vw_LeaveCompanyReport");

            // Certification View
            modelBuilder.Entity<CertificationSummaryView>()
            .HasNoKey()
            .ToView("vw_CertificationSummary_Active");

            // Company Summary View
            modelBuilder.Entity<CompanySummary>()
            .HasNoKey()
            .ToView("vw_CompanySummary");

            // Annual Leave Summary
            modelBuilder.Entity<AnnualLeaveSummary>()
            .HasNoKey()
            .ToView("vw_LeaveBalance");

            // Employment Yearly Status
            modelBuilder.Entity<EmploymentYearlyStat>()
            .HasNoKey()
            .ToView("vw_EmployeeYearlyStats");

            // Department Employment Stats
            modelBuilder.Entity<DepartmentEmploymentStats>()
            .HasNoKey()
            .ToView("vw_DepartmentEmployeeStats");

            // Employment Details View
            modelBuilder.Entity<EmployeeDetailView>()
            .HasNoKey()
            .ToView("vw_EmploymentDetails");

            //Overtime Detail View
            modelBuilder.Entity<OvertimeDetailView>()
            .HasNoKey()
            .ToView("vw_OvertimeDetailView");

            //Overtime Summary View
            modelBuilder.Entity<OvertimeSummaryView>()
            .HasNoKey()
            .ToView("vw_OvertimeSummaryView");

            //Allowance Detail View
            modelBuilder.Entity<AllowanceDetailView>()
            .HasNoKey()
            .ToView("vw_AllowanceDetailView");

            //Talent Experience View
            modelBuilder.Entity<TalentExperienceView>()
            .HasNoKey()
            .ToView("vw_TalentPoolExperience");

            //Certification Detail View
            modelBuilder.Entity<CertificationDetailsView>()
            .HasNoKey()
            .ToView("vw_CertificationDetails");

            /// <summary>
            /// Structure View
            ///</summary>
            modelBuilder.Entity<StructureView>()
            .HasNoKey()
            .ToView("vw_StructureView");

            /// <summary>
            /// Termination Detail View
            ///</summary>
            modelBuilder.Entity<TerminationDetailView>()
            .HasNoKey()
            .ToView("vw_TerminationDetail");

            /// <summary>
            /// Evaluation Report View
            ///</summary>
            modelBuilder.Entity<EvaluationSummaryView>()
            .HasNoKey()
            .ToView("vw_EvaluationSummary");


            modelBuilder.Entity<EvalGrandView>()
            .HasNoKey()
            .ToView("vw_EvaluationPersonGrandReport");

            /// <summary>
            /// Experience Report View
            ///</summary>
            modelBuilder.Entity<ExperienceView>()
            .HasNoKey()
            .ToView("vw_ExperienceView");

            /// <summary>
            /// JobGapAnalysis View
            ///</summary>
            modelBuilder.Entity<JobGapAnalysis>()
            .HasNoKey()
            .ToView("vw_JobGapAnalysisView");

            /// <summary>
            /// Certification Summary Break Down View
            ///</summary>
            modelBuilder.Entity<CertificationBreakDownView>()
            .HasNoKey()
            .ToView("vw_CertificationBreakDown");

            /// <summary>
            /// Worksite Summary View
            ///</summary>
            modelBuilder.Entity<WorksiteSummaryView>()
            .HasNoKey()
            .ToView("vw_WorksiteSummaryView");

            /// <summary>
            /// Leave Balance By Department View
            ///</summary>
            modelBuilder.Entity<LeaveBalanceDepartmentView>()
            .HasNoKey()
            .ToView("vw_LeaveBalanceDepartmentView");
            /// <summary>
            /// Leave History View
            ///</summary>
            modelBuilder.Entity<LeaveHistoryView>()
            .HasNoKey()
            .ToView("vw_LeaveHistoryView");
            /// <summary>
            /// Overtime History View
            ///</summary>
            modelBuilder.Entity<OvertimeHistoryView>()
            .HasNoKey()
            .ToView("vw_OvertimeHistoryView");

            /// <summary>
            /// Earning View
            ///</summary>
            modelBuilder.Entity<EarningView>()
            .HasNoKey()
            .ToView("vw_EarningsView");

            /// <summary>
            /// Job Placement View
            ///</summary>
            modelBuilder.Entity<JobPlacementView>()
            .HasNoKey()
            .ToView("vw_JobPlacementView");

            /// <summary>
            /// Job Placement Detail View
            ///</summary>
            modelBuilder.Entity<JobPlacementDetailView>()
            .HasNoKey()
            .ToView("vw_JobPlacementDetails");

            /// <summary>
            /// Yearly Job PlacementSalary View
            ///</summary>
            modelBuilder.Entity<YearlyJobPlacementSalaryView>()
            .HasNoKey()
            .ToView("vw_YearlyJobPlacementSummary");

            /// <summary>
            /// Employee Salary Growth View
            ///</summary>
            modelBuilder.Entity<EmployeeSalaryGrowthView>()
            .HasNoKey()
            .ToView("vw_EmployeeSalaryGrowth");

            ///<summary>
            ///Users Short View
            /// </summary>
            modelBuilder.Entity<UserView>()
           .HasNoKey()
           .ToView("vw_UserView");

            ///<summary>
            ///Language Related
            /// </summary>
            modelBuilder.Entity<AppTranslationModel>()
             .HasOne(x => x.Language)
             .WithMany(x => x.AppTranslations)
             .HasForeignKey(x => x.languageID)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EntityTranslationModel>(e => 
            {
                e.HasOne(x => x.Language).WithMany(x => x.EntityTranslations).HasForeignKey(x => x.languageID).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => new
                {
                    x.entityType,
                    x.entityID,
                    x.propertyName,
                    x.languageID
                })
                .IsUnique();
            });
                

            modelBuilder.Entity<AppTranslationModel>()
                .HasIndex(x => new
                {
                    x.translationKey,
                    x.languageID
                })
                .IsUnique();

            modelBuilder.Entity<LanguageModel>()
            .HasIndex(x => x.languageCode)
            .IsUnique();

        }
        
        public DbSet<accessModel> Accesses { get; set; }
        public DbSet<accessHistoryModel> AccessHistories { get; set; }
        public DbSet<accountModel> Accounts { get; set; }
        public DbSet<addressModel> Addresses { get; set; }
        public DbSet<allowanceAssignmentModel> AllowanceAssignments { get; set; }
        public DbSet<allowanceAssignmentHistoryModel> AllowanceAssignmentsHistories { get; set; }
        public DbSet<allowanceModel> Allowances { get; set; }
        public DbSet<ApplicantModel> Applicants { get; set; }
        public DbSet<archiveModel> Archives { get; set; }
        public DbSet<AttachmentModel> Attachments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<bankInfoModel> BankInfos { get; set; }
        public DbSet<businessUnitModel> BusinessUnits { get; set; }
        public DbSet<companyModel> Companies { get; set; }
        public DbSet<contractModel> Contracts { get; set; }
        public DbSet<contractHistoryModel> ContractHistories { get; set; }
        public DbSet<delegationModel> Delegations { get; set; }
        public DbSet<delegationHistoryModel> DelegationHistories {  get; set; }
        public DbSet<departmentModel> Departments { get; set; }
        public DbSet<departmentHistoryModel> DepartmentHistories { get; set; }
        public DbSet<disciplineModel> Desciplines { get; set; }
        public DbSet<educationLevelModel> EducationLevels { get; set; }
        public DbSet<employmentModel> Employments { get; set; }
        public DbSet<employmentHistoryModel> EmploymentHistories { get; set; }
        public DbSet<employmentMethodModel> EmploymentMethods { get; set; }
        public DbSet<employmentMethodHistoryModel> EmploymentMethodHistories { get; set; }
        public DbSet<employmentTypeModel> EmploymentTypes { get; set; }
        public DbSet<evaluationModel> Evaluations { get; set; }
        public DbSet<evaluationTypeModel> EvaluationTypes { get; set; }
        public DbSet<evaluationTaskModel> EvaluationTasks { get; set; }
        public DbSet<evaluationSubTaskModel> EvaluationSubTasks { get; set; }
        public DbSet<evaluationValuationModel> EvaluationValuations { get; set; }
        public DbSet<experienceModel> Experiences { get; set; }
        public DbSet<familyModel> Families { get; set; }
        public DbSet<guarantyModel> Guaranties { get; set; }
        public DbSet<guarantyHistoryModel> GuarantyHistories { get; set; }
        public DbSet<holidayModel> Holidays { get; set; }
        public DbSet<jobModel> Jobs { get; set; }
        public DbSet<jobCategoryModel> JobCategories { get; set; }
        public DbSet<jobClassModel> JobClasses { get; set; }
        public DbSet<jobPlacementModel> JobPlacements { get; set; }
        
        public DbSet<jobPlacementHistoryModel> JobPlacementHistories { get; set; }
        public DbSet<jobGradeModel> JobGrades { get; set; }
        public DbSet<jobRequirementModel> JobRequirements { get; set; }
        public DbSet<jobRequirementHistoryModel> JobRequirementHistories { get; set; }
        public DbSet<jobReqCost> JobReqCosts { get; set; }
        public DbSet<jobStepModel> JobSteps { get; set; }
        public DbSet<jobStepHistoryModel> JobStepHistories { get; set; }
        public DbSet<leaveModel> Leaves { get; set; }
        public DbSet<leaveHistoryModel> LeaveHistories { get; set; }
        public DbSet<leaveTypeModel> LeaveTypes { get; set; }
        public DbSet<letterModel> Letters { get; set; }
        public DbSet<letterTypeModel> LetterTypes { get; set; }
        public DbSet<LetterSequence> LetterSequences { get; set; }
        public DbSet<NoticeModel> Notices { get; set; }
        public DbSet<overtimeModel> Overtimes { get; set; }
        public DbSet<overtimeRecordModel> OvertimeRecords { get; set; }
        public DbSet<overtimeHistoryModel> OvertimeHistories { get; set; }
        public DbSet<penaltyModel> Penalties { get; set; }
        public DbSet<penaltyHistoryModel> PenaltyHistories { get; set; }
        public DbSet<penaltyTypeModel> PenaltyTypes { get; set; }
        public DbSet<personModel> Persons { get; set; }
        public DbSet<personHistoryModel> PersonHistories { get; set; }
        public DbSet<personEducationLevelModel> PersonEducationLevels { get; set; }
        public DbSet<prohibitionModel> Prohibitions { get; set; }
        public DbSet<prohibitionHistoryModel> prohibitionHistories { get; set; }
        public DbSet<serviceRequestModel> ServiceRequests { get; set; }
        public DbSet<serviceRequestHistoryModel> ServiceRequestHistories { get; set; }
        public DbSet<serviceRequestTypeModel> ServiceRequestTypes { get; set; }
        public DbSet<siteAssignmentModel> SiteAssignments { get; set; }
        public DbSet<shiftModel> Shifts { get; set; }
        public DbSet<shiftAssignmentModel> ShiftAssignments { get; set; }
        public DbSet<structureModel> Structures { get; set; }
        public DbSet<structureHistoryModel> StructureHistories { get; set; }
        public DbSet<subAccountModel> SubAccounts { get; set; }
        public DbSet<terminationModel> Terminations { get; set; }
        public DbSet<breakModel> Breaks { get; set; } = default!;
        public DbSet<userModel> Users { get; set; } = default!;
        public DbSet<userHistoryModel> UserHistories { get; set; } = default!;
        public DbSet<VacancyModel> Vacancies { get; set; }
        public DbSet<workSiteModel> WorkSites { get; set; }       
        public DbSet<workSiteHistoryModel> WorkSitesHistories { get; set; }
        public DbSet<loyaltyModel> Loyalties { get; set; } = default!;
        public DbSet<loyaltyHistoryModel> LoyaltyHistories { get; set; } = default!;

        ///<summary>
        /// Payroll related
        ///</summary>
        public DbSet<payrollModel> Payrolls { get; set; }
        public DbSet<payrollHistory> PayrollHistories { get; set; }
        public DbSet<payrollPay> PayrollPays { get; set; }
        public DbSet<earningModel> Earnings { get; set; }
        public DbSet<earningHistoryModel> EarningHistories { get; set; }
        public DbSet<earningRecordModel> EarningRecords { get; set; }
        public DbSet<earningType> EarningTypes { get; set; }
        public DbSet<deductionModel> Deductions { get; set; }
        public DbSet<deductionHistoryModel> DeductionHistories { get; set; }
        public DbSet<deductionRecordModel> DeductionRecords { get; set; }
        public DbSet<deductionType> DeductionTypes { get; set; }
        public DbSet<taxRateModel> TaxRates { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BudgetLine> BudgetLines { get; set; }

        /// <summary>
        /// ///Views
        /// </summary>

        public DbSet<LeaveReportView> LeaveReportView { get; set; } = default!;
        public DbSet<CertificationSummaryView> CertificationSummaryView { get; set; } = default!;
        public DbSet<CompanySummary> CompanySummaryView { get; set; } = default!;
        public DbSet<AnnualLeaveSummary> AnnualLeaveSummary { get; set; } = default!;
        public DbSet<EmploymentYearlyStat> EmploymentYearlyStats { get; set; } = default!;
        public DbSet<DepartmentEmploymentStats> DepartmentEmploymentStats { get; set; } = default!;
        public DbSet<EmployeeDetailView> EmployeeDetailViews { get; set; } = default!;
        public DbSet<OvertimeDetailView> OvertimeDetailView { get; set; } = default!;
        public DbSet<OvertimeSummaryView> OvertimeSummaryView { get; set; } = default!;
        public DbSet<AllowanceDetailView> AllowanceDetailView { get; set; } = default!;
        public DbSet<TalentExperienceView> TalentExperienceView { get; set; } = default!;
        public DbSet<CertificationDetailsView> CertificationDetailsView { get; set; } = default!;
        public DbSet<LeaveReportCompany> LeaveReportCompany { get; set; } = default!;
        public DbSet<StructureView> StructureView { get; set; } = default!;
        public DbSet<TerminationDetailView> TerminationDetailView { get; set; } = default!;
        public DbSet<EvaluationSummaryView> EvaluationSummaryView { get; set; } = default!;
        public DbSet<EvalGrandView> EvalGrandView { get; set; } = default!;
        public DbSet<ExperienceView> ExperienceView { get; set; } = default!;
        public DbSet<JobGapAnalysis> JobGapAnalysisView { get; set; } = default!;
        public DbSet<CertificationBreakDownView> CertificationBreakDownView { get; set; } = default!;
        public DbSet<WorksiteSummaryView> WorksiteSummaryView { get; set; } = default!;
        public DbSet<LeaveBalanceDepartmentView> LeaveBalanceDepartmentView { get; set; } = default!;
        public DbSet<LeaveHistoryView> LeaveHistoryView { get; set; } = default!;
        public DbSet<OvertimeHistoryView> OvertimeHistoryView { get; set; } = default!;
        public DbSet<EarningView> EarningView { get; set; } = default!;
        public DbSet<JobPlacementDetailView> JobPlacementDetailView { get; set; } = default!;
        public DbSet<JobPlacementView> JobPlacementView { get; set; } = default!;
        public DbSet<YearlyJobPlacementSalaryView> YearlyJobPlacementSalaryView { get; set; } = default!;
        public DbSet<EmployeeSalaryGrowthView> EmployeeSalaryGrowthView { get; set; } = default!;
        public DbSet<UserView> UserView { get; set; } = default!;

        /// <summary>
        /// Training Module
        /// </summary>
        public DbSet<trainingModel> Trainings { get; set; }
        public DbSet<trainingSessionModel> TrainingSessions { get; set; }
        public DbSet<trainingAttendanceModel> TrainingAttendances { get; set; }
        public DbSet<trainingCostAllocationModel> TrainingCostAllocations { get; set; }

        /// <summary>
        /// Language Related 
        /// </summary>
        /// 
        public DbSet<LanguageModel> Languages { get; set; }

        public DbSet<AppTranslationModel> AppTranslations { get; set; }

        public DbSet<EntityTranslationModel> EntityTranslations { get; set; }
    }
    
}
