using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Models
{
    public class PISContext : DbContext
    {
        public PISContext(DbContextOptions options) : base(options) 
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Automatically register all entity types in the Assembly
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(PISContext).Assembly);
            modelBuilder.Entity<accountModel>(entity =>
            {
                entity.Property(e => e.accountID).HasColumnName("accountID");
                entity.Property(e => e.accountNumber).HasColumnName("accountNumber");
                entity.Property(e => e.accountDescription).HasColumnName("accountDescription");
                entity.Property(e => e.accountName).HasColumnName("accountName");
                entity.Property(e => e.accountStatus).HasColumnName("accountStatus").HasConversion<int>();
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                
                entity.HasMany(e => e.SubAccounts).WithOne(p => p.accountModel).HasForeignKey(e => e.accountID);
                entity.HasIndex(e => e.accountNumber).IsUnique();
                entity.HasIndex(e =>e.accountName).IsUnique();
            });

            modelBuilder.Entity<addressModel>(entity =>
            {
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.addressCountry).HasColumnName("addressCountry");
                entity.Property(e => e.addressRegion).HasColumnName("addressRegion");
                entity.Property(e => e.addressStatus).HasColumnName("addressStatus");
                entity.Property(e => e.addressTabya).HasColumnName("addressTabya");
                entity.Property(e => e.addressWoreda).HasColumnName("addressWoreda");
                entity.Property(e => e.addressZone).HasColumnName("addressZone");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.Persons).WithOne(p => p.addressModel).HasForeignKey(p => p.addressID);
                entity.HasMany(e => e.WorkSites).WithOne(p => p.addressModel).HasForeignKey(p => p.addressID);
                entity.HasMany(e => e.Companies).WithOne(p => p.addressModel).HasForeignKey(p => p.addressID);
                entity.HasIndex(e => new { e.addressCountry, e.addressRegion, e.addressZone, e.addressWoreda, e.addressTabya }).IsUnique();
            });

            modelBuilder.Entity<allowanceModel>(entity =>
            {
                entity.Property(e => e.allowanceID).HasColumnName("allowanceID");
                entity.Property(e => e.allowanceAmount)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("allowanceAmount");
                entity.Property(e => e.allowanceDescription).HasColumnName("allowanceDescription");
                entity.Property(e => e.allowanceName).HasColumnName("allowanceName");
                entity.Property(e => e.allowanceStatus).HasColumnName("allowanceStatus");
                entity.Property(e => e.allowanceTaxable).HasColumnName("allowanceTaxable");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.AllowanceAssignments).WithOne(p => p.allowanceModel).HasForeignKey(e => e.allowanceID);
                entity.HasIndex(e => e.allowanceName).IsUnique();
            });

            modelBuilder.Entity<allowanceAssignmentModel>(entity =>
            {
                entity.HasIndex(e => e.allowanceID, "IX_AllowanceAssignments_allowanceModelallowanceID");
                entity.Property(e => e.allowanceAssignmentID).HasColumnName("allowanceAssignmentID");
                entity.Property(e => e.allowanceID).HasColumnName("allowanceID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.allowanceAssignmentAmount).HasColumnName("allowanceAssignmentAmount");
                entity.Property(e => e.allowanceStatus).HasColumnName("allowanceStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.allowanceModel).WithMany(p => p.AllowanceAssignments).HasForeignKey(d => d.allowanceID);
                entity.HasMany(d => d.AllowanceAssignmentHistories).WithOne(p => p.allowanceAssignmentModel).HasForeignKey(p => p.allowanceAssignmentID);
                
                entity.HasIndex(d => new { d.allowanceID, d.employmentID }).IsUnique();
            });

            modelBuilder.Entity<allowanceAssignmentHistoryModel>(entity =>
            {
                entity.Property(e => e.allowanceAssignmentHistoryID).HasColumnName("allowanceAssignmentHistoryID");
                entity.Property(e => e.allowanceAssignmentID).HasColumnName("allowanceAssignmentID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.allowanceAssignmentHistoryStatus).HasColumnName("allowanceAssignmentHistoryStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e=>e.allowanceAssignmentModel).WithMany(d=>d.AllowanceAssignmentHistories).HasForeignKey(e => e.allowanceAssignmentID);

            });

            modelBuilder.Entity<bankInfoModel>(entity =>
            {
                entity.HasIndex(e => e.personID, "IX_BankInfos_personModelpersonID");

                entity.Property(e => e.bankInfoID).HasColumnName("bankInfoID");
                entity.Property(e => e.bankAccountNumber).HasColumnName("bankAccountNumber");
                entity.Property(e => e.bankBranch).HasColumnName("bankBranch");
                entity.Property(e => e.bankName).HasColumnName("bankName");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.personModel).WithMany(p => p.Banks).HasForeignKey(d => d.personID);
                entity.HasIndex(d => new { d.bankAccountNumber, d.personID, d.bankName }).IsUnique();
            });

            modelBuilder.Entity<breakModel>(entity =>
            {
                entity.HasIndex(e => e.shiftID, "IX_Breaks_shiftModelshiftID");

                entity.Property(e => e.breakID).HasColumnName("breakID");
                entity.Property(e => e.breakEnd).HasColumnName("breakEnd");
                entity.Property(e => e.breakName).HasColumnName("breakName");
                entity.Property(e => e.breakStart).HasColumnName("breakStart");
                entity.Property(e => e.breakStatus).HasColumnName("breakStatus");
                entity.Property(e => e.shiftID).HasColumnName("shiftID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.shiftModel).WithMany(p => p.Breaks).HasForeignKey(d => d.shiftID);
                entity.HasIndex(d => new { d.breakEnd, d.breakStart }).IsUnique();
            });

            modelBuilder.Entity<companyModel>(entity =>
            {
                
                entity.Property(e => e.companyID).HasColumnName("companyID");
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.companyName).HasColumnName("companyName");
                entity.Property(e => e.companyAlias).HasColumnName("companyShort");
                entity.Property(e => e.companyStatus).HasColumnName("companyStatus");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.HasMany(e => e.Departments).WithOne(p => p.companyModel).HasForeignKey(p => p.companyID);
                entity.HasOne(e => e.addressModel).WithMany(p => p.Companies).HasForeignKey(p => p.addressID);
                entity.HasIndex(e => e.companyName).IsUnique();
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                //entity.HasOne(e => e.employmentModel).WithOne(p => p.companyModel).HasForeignKey<companyModel>(p => p.employmentID).OnDelete(DeleteBehavior.ClientSetNull); ;
            });
            modelBuilder.Entity<contractModel>(entity =>
            {

                entity.Property(e => e.contractID).HasColumnName("contractID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.startDate).HasColumnName("startDate");
                entity.Property(e => e.endDate).HasColumnName("endDate");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(c => c.ContractHistories).WithOne(ch => ch.contractModel).HasForeignKey(p => p.contractID);
                entity.HasOne(c => c.employmentModel).WithOne(e => e.contractModel).HasForeignKey<contractModel>(p => p.employmentID);
                
                //entity.HasOne(e => e.employmentModel).WithOne(p => p.companyModel).HasForeignKey<companyModel>(p => p.employmentID).OnDelete(DeleteBehavior.ClientSetNull); ;
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

                entity.HasOne(ch => ch.contractModel).WithMany(c => c.ContractHistories).HasForeignKey(p => p.contractID);
                
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
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.companyModel).WithMany(p => p.Departments).HasForeignKey(d => d.companyID);

               // entity.HasOne(d => d.employmentModel).WithOne(p => p.departmentModel)
                //    .HasForeignKey<departmentModel>(d => d.employmentID)
                  //  .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.subAccountModel).WithMany(p => p.Departments).HasForeignKey(d => d.subAccountID);
                entity.HasMany(e =>e.JobPlacements).WithOne(p => p.departmentModel).HasForeignKey(d => d.departmentID);
                entity.HasMany(d => d.OvertimeRecords).WithOne(or => or.departmentModel).HasForeignKey(d => d.departmentID);
                entity.HasIndex(e => new { e.companyID, e.departmentName}).IsUnique();
            });

            modelBuilder.Entity<educationLevelModel>(entity =>
            {
                entity.HasIndex(e => e.educationLevelName, "IX_EducationLevels_personModelpersonID").IsUnique();

                entity.Property(e => e.educationLevelID).HasColumnName("educationLevelID");
                entity.Property(e => e.educationLevelCategory).HasColumnName("educationLevelCategory");
                entity.Property(e => e.educationLevelDescription).HasColumnName("educationLevelDescription");
                entity.Property(e => e.educationLevelGrade).HasColumnName("educationLevelGrade");
                entity.Property(e => e.educationLevelName).HasColumnName("educationLevelName");
                entity.Property(e => e.educationLevelStatus).HasColumnName("educationLevelStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(d => d.PersonEducationLevels).WithOne(p => p.educationLevelModel);
                entity.HasMany(d => d.Jobs).WithMany(p => p.EducationLevels);
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
                entity.Property(e => e.employmentRequestID).HasColumnName("employmentRequestID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.employmentPosition).HasColumnName("employmentPosition");

                entity.HasOne(d => d.personModel).WithMany(p => p.Employments).HasForeignKey(d => d.personID);
                entity.HasMany(d => d.EmploymentHistories).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasMany(d => d.OvertimeRecords).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasMany(d => d.JobPlacements).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasMany(d => d.Leaves).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasMany(d => d.AllowanceAssignments).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasMany(d => d.LoyaltyHistories).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasMany(d => d.WorkSites).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasOne(e => e.employmentMethodModel).WithMany(em => em.Employments).HasForeignKey(d => d.employmentID);
                entity.HasOne(e => e.employmentRequestModel).WithMany(er => er.Employments).HasForeignKey(d => d.employmentRequestID);
                

                entity.HasOne(d => d.employmentTypeModel).WithMany(e => e.Employments).HasForeignKey(d => d.employmentTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(d => new { d.personID, d.givenID }).HasFilter("[employmentStatus] = 1").IsUnique();
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

                entity.HasOne(e => e.employmentModel).WithMany(d => d.EmploymentHistories).HasForeignKey(p => p.employmentID);
                entity.HasOne(e => e.employmentTypeModel).WithMany(d => d.EmploymentHistories).HasForeignKey(p => p.employmentID);
            });
            modelBuilder.Entity<employmentMethodModel>(entity =>
            {
                entity.Property(e => e.employmentMethodID).HasColumnName("employmentMethodID");
                entity.Property(e => e.employmentMethodName).HasColumnName("employmentMethodName");
                entity.Property(e => e.employmentMethodDescription).HasColumnName("employmentMethodDescription");
                entity.Property(e => e.employmentMethodStatus).HasColumnName("employmentMethodStatus");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(em => em.Employments).WithOne(e => e.employmentMethodModel).HasForeignKey(p => p.employmentMethodID);
                entity.HasMany(em => em.EmploymentMethodHistories).WithOne(emh => emh.employmentMethodModel).HasForeignKey(p => p.employmentMethodID);
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

                entity.HasOne(emh => emh.employmentMethodModel).WithMany(em => em.EmploymentMethodHistories).HasForeignKey(p => p.employmentMethodHistoryID);
            });
            modelBuilder.Entity<employmentRequestModel>(entity =>
            {
                entity.Property(e => e.employmentRequestID).HasColumnName("employmentRequestID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.employmentRequestDate).HasColumnName("employmentRequestDate");
                entity.Property(e => e.employmentTypeID).HasColumnName("employmentTypeID");
                entity.Property(e => e.requiredNo).HasColumnName("requiredNo");
                entity.Property(e => e.requestStatus).HasColumnName("requestStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(er => er.employmentTypeModel).WithMany(et => et.EmploymentRequests).HasForeignKey(p => p.employmentTypeID);
                entity.HasOne(er => er.jobModel).WithMany(j => j.EmploymentRequests).HasForeignKey(p => p.jobID);
                entity.HasMany(er => er.Employments).WithOne(e => e.employmentRequestModel).HasForeignKey(p => p.employmentRequestID);
                entity.HasMany(er => er.EmploymentRequestHistories).WithOne(erh => erh.employmentRequestModel).HasForeignKey(p => p.employmentRequestID);
            });
            modelBuilder.Entity<employmentRequestHistoryModel>(entity =>
            {
                entity.Property(e => e.employmentRequestHistoryID).HasColumnName("employmentRequestHistoryID");
                entity.Property(e => e.employmentRequestID).HasColumnName("employmentRequestID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.employmentType).HasColumnName("employmentType");
                entity.Property(e => e.requestStatus).HasColumnName("requestStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(erh => erh.employmentRequestModel).WithMany(er => er.EmploymentRequestHistories).HasForeignKey(p => p.employmentRequestID);
                
            });
            modelBuilder.Entity<employmentTypeModel>(entity =>
            {
                entity.Property(e => e.employmentTypeID).HasColumnName("employmentTypeID");
                entity.Property(e => e.annualAccrualRate).HasColumnName("annualAccrualRate");
                entity.Property(e => e.employmentBaseLeave).HasColumnName("employmentBaseLeave");
                entity.Property(e => e.employmentTypeDescription).HasColumnName("employmentTypeDescription");
                entity.Property(e => e.employmentTypeName).HasColumnName("employmentTypeName");
                entity.Property(e => e.employmentTypeStatus).HasColumnName("employmentTypeStatus");
                entity.Property(e => e.isExprienceCount).HasColumnName("isExprienceCount");
                entity.Property(e => e.isLeaveCount).HasColumnName("isLeaveCount");
                entity.Property(e => e.isLoyalityAllowed).HasColumnName("isLoyalityAllowed");
                entity.Property(e => e.isSalaryAllowed).HasColumnName("isSalaryAllowed");
                entity.Property(e => e.isSeveranceAllowed).HasColumnName("isSeveranceAllowed");
                entity.Property(e => e.isCarrierAllowed).HasColumnName("isCarrierAllowed");
                entity.Property(e => e.isStepAllowed).HasColumnName("isStepAllowed");
                entity.Property(e => e.employmentMinAge).HasColumnName("employmentMinAge");
                entity.Property(e => e.employmentMaxAge).HasColumnName("employmentMaxAge");
                entity.Property(e => e.maxLeaveIncrement).HasColumnName("maxLeaveIncrement");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.EmploymentHistories).WithOne(p => p.employmentTypeModel).HasForeignKey(p => p.employmentTypeID);
                entity.HasMany(e => e.Employments).WithOne(p => p.employmentTypeModel).HasForeignKey(p => p.employmentTypeID);
                entity.HasMany(et => et.EmploymentRequests).WithOne(er => er.employmentTypeModel).HasForeignKey(p =>p.employmentTypeID);
                entity.HasIndex(e => e.employmentTypeName).IsUnique();
            });

            modelBuilder.Entity<holidayModel>(entity =>
            {
                entity.Property(e => e.holidayID).HasColumnName("holidayID");
                entity.Property(e => e.holidayEnd).HasColumnName("holidayEnd");
                entity.Property(e => e.holidayName).HasColumnName("holidayName");
                entity.Property(e => e.holidayCycle).HasColumnName("holidayRepition");
                entity.Property(e => e.holidayStart).HasColumnName("holidayStart");
                entity.Property(e => e.holidayStatus).HasColumnName("holidayStatus");
                entity.Property(e => e.holidayType).HasColumnName("holidayType");
                entity.HasIndex(e => e.holidayName).IsUnique();
            });

            modelBuilder.Entity<jobModel>(entity =>
            {

                entity.HasIndex(e => e.jobCategoryID, "IX_Jobs_jobCategoryModeljobCategoryID");

                entity.HasIndex(e => e.jobClassID, "IX_Jobs_jobClassModelJobClassId");

                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.jobCategoryID).HasColumnName("jobCategoryID");
                entity.Property(e => e.jobClassID).HasColumnName("jobClassID");
                entity.Property(e => e.jobDescription).HasColumnName("jobDescription");
                entity.Property(e => e.jobGradeID).HasColumnName("jobGrade");                
                entity.Property(e => e.jobStatus).HasColumnName("jobStatus");
                entity.Property(e => e.jobTitle).HasColumnName("jobTitle");
                entity.Property(e => e.jobCode).HasColumnName("jobCode");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.jobCategoryModel).WithMany(p => p.Jobs).HasForeignKey(p => p.jobCategoryID);
                entity.HasOne(d => d.jobClassModel).WithMany(p => p.Jobs).HasForeignKey(d => d.jobClassID);
                entity.HasMany(d => d.EducationLevels).WithMany(p => p.Jobs);
                entity.HasOne(d => d.jobGradeModel).WithMany(e => e.Jobs).HasForeignKey(p => p.jobGradeID);
                entity.HasMany(j => j.EmploymentRequests).WithOne(er => er.jobModel).HasForeignKey(p => p.jobID);
                entity.HasIndex(d => d.jobCode).IsUnique();
            });
            modelBuilder.Entity<jobGradeModel>(entity =>
            {
                entity.Property(e => e.jobGradeID).HasColumnName("jobGradeID");
                entity.Property(e => e.jobGradeName).HasColumnName("jobGradeName");
                entity.Property(e => e.jobGradeDescription).HasColumnName("jobGradeDescription");
                entity.Property(e => e.jobGradeBasicSalary).HasColumnName("jobGradeBasicSalary");
                entity.Property(e => e.jobGradeMaxSalary).HasColumnName("jobGradeMaxSalary"); 
                entity.Property(e => e.jobGradeStatus).HasColumnName("jobGradeStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.Jobs).WithOne(d => d.jobGradeModel).HasForeignKey(d => d.jobGradeID);
                entity.HasIndex(e => e.jobGradeName).IsUnique();
            });

            modelBuilder.Entity<jobCategoryModel>(entity =>
            {
                entity.Property(e => e.jobCategoryID).HasColumnName("jobCategoryID");
                entity.Property(e => e.jobCategoryDescription).HasColumnName("jobCategoryDescription");
                entity.Property(e => e.jobCategoryName).HasColumnName("jobCategoryName");
                entity.Property(e => e.jobCategoryStatus).HasColumnName("jobCategoryStatus");
                entity.HasMany(e => e.Jobs).WithOne(p => p.jobCategoryModel).HasForeignKey(p => p.jobCategoryID);
                entity.HasIndex(e => e.jobCategoryName).IsUnique();
            });

            modelBuilder.Entity<jobPlacementModel>(entity =>
            {
                entity.HasIndex(e => e.departmentID, "IX_JobPlacements_departmentModeldepartmentID");

                entity.HasIndex(e => e.employmentID, "IX_JobPlacements_employmentModelemploymentID");

                entity.HasIndex(e => e.jobID, "IX_JobPlacements_jobModeljobID");

                entity.Property(e => e.jobPlacementID).HasColumnName("jobPlacementID");
                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.employmentID).HasColumnName("employeeID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.jobPlacementDate).HasColumnName("jobPlacementDate");
                entity.Property(e => e.jobPlacementReference).HasColumnName("jobPlacementReference");
                entity.Property(e => e.jobPlacementSalary)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("jobPlacementSalary");
                entity.Property(e => e.jobPlacementStatus).HasColumnName("jobPlacementStatus");
                entity.Property(e => e.shiftID).HasColumnName("shiftID");
                entity.Property(e => e.workSiteID).HasColumnName("workSiteID");
                entity.Property(e => e.jobPlacementReason).HasColumnName("jobPlacementReason");
                entity.Property(e => e.jobPlacementStep).HasColumnName("jobPlacementStep");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.departmentModel).WithMany(p => p.JobPlacements)
                    .HasForeignKey(d => d.departmentID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.employmentModel).WithMany(p => p.JobPlacements)
                    .HasForeignKey(d => d.employmentID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.jobModel).WithMany(p => p.JobPlacements).HasForeignKey(d => d.jobID);
                entity.HasOne(d => d.shiftModel).WithMany(p => p.JobPlacements).HasForeignKey(d => d.shiftID);
                entity.HasOne(d => d.workSiteModel).WithMany(p => p.JobPlacements).HasForeignKey(d => d.workSiteID);
                entity.HasIndex(d => new { d.jobID, d.employmentID }).HasFilter("[jobPlacementStatus]=1").IsUnique();
                entity.HasIndex(d => d.employmentID).HasFilter("[jobPlacementStatus]=1").IsUnique();
            });
            modelBuilder.Entity<jobRequirementModel>(entity =>
            {
                entity.Property(e => e.jobRequirementID).HasColumnName("jobRequirementID");
                entity.Property(e => e.departmentID).HasColumnName("departmentID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.requiredNumber).HasColumnName("requiredNumber");
                entity.Property(e => e.jobRequirementStatus).HasColumnName("jobRequirementStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasMany(e => e.JobRequirementHistories).WithOne(p => p.JobRequirementModel).HasForeignKey(p => p.jobRequirementID);
                entity.HasOne(jr => jr.DepartmentModel).WithMany(d => d.JobRequirements).HasForeignKey(jr => jr.departmentID);
                entity.HasOne(jr => jr.JobModel).WithMany(j => j.JobRequirements).HasForeignKey(j => j.jobID);
            });
            modelBuilder.Entity<jobRequirementHistoryModel>(entity =>
            {
                entity.Property(e => e.jobRequirementHistoryID).HasColumnName("jobRequirementHistoryID");
                entity.Property(e => e.jobRequirementID).HasColumnName("jobRequirementID");                
                entity.Property(e => e.requiredNumber).HasColumnName("requiredNumber");
                entity.Property(e => e.jobRequirementStatus).HasColumnName("jobRequirementStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(jr => jr.JobRequirementModel).WithMany(jr => jr.JobRequirementHistories).HasForeignKey(jr => jr.jobRequirementID);
              
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
                entity.Property(e => e.leaveReaquestDate).HasColumnName("leaveReaquestDate");
                entity.Property(e => e.leaveStartDate).HasColumnName("leaveStartDate");
                entity.Property(e => e.leaveStatus).HasColumnName("leaveStatus");
                entity.Property(e => e.leaveTypeID).HasColumnName("leaveTypeID");
                entity.Property(e => e.oldBatchNbr).HasColumnName("oldBatchNbr");
                entity.Property(e => e.ratePerHour).HasColumnName("ratePerHour");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                
                entity.HasOne(d => d.employmentModel).WithMany(p => p.Leaves).HasForeignKey(d => d.employmentID);
                entity.HasOne(d => d.leaveTypeModel).WithMany(p => p.Leaves).HasForeignKey(d => d.leaveTypeID);
                entity.HasMany(d => d.LeaveHistories).WithOne(p => p.leaveModel).HasForeignKey(d => d.leaveID);
                entity.ToTable(tb => tb.UseSqlOutputClause(false));

                entity.HasCheckConstraint("CK_Leave_leaveEndDate",
                    "[leaveEndDate]>=[leaveStartDate]");

                entity.HasCheckConstraint("CK_Leave_NumberOfDays",
                    "leaveDays>0 AND leaveDays <= DATEDIFF(DAY, leaveStartDate, leaveEndDate)+1");
                entity.HasIndex(d => new { d.employmentID, d.leaveStartDate, d.leaveEndDate }).IsUnique();             

            });
            modelBuilder.Entity<leaveHistoryModel>(entity =>
            {
                entity.Property(e => e.leaveHistoryID).HasColumnName("leaveHistoryID");
                entity.Property(e => e.leaveID).HasColumnName("leaveID");
                entity.Property(e => e.leaveHistoryAction).HasColumnName("leaveHistoryAction");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.leaveModel).WithMany(p => p.LeaveHistories).HasForeignKey(e => e.leaveID);
            });

            modelBuilder.Entity<leaveTypeModel>(entity =>
            {
                entity.Property(e => e.leaveTypeID).HasColumnName("leaveTypeID");
                entity.Property(e => e.leaveTypeImpact).HasColumnName("leaveTypeImpact");
                entity.Property(e => e.leaveTypeName).HasColumnName("leaveTypeName");
                entity.Property(e => e.leaveTypeStatus).HasColumnName("leaveTypeStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.leaveAvailability).HasColumnName("leaveAvailability");

                entity.HasMany(p => p.Leaves).WithOne(e => e.leaveTypeModel).HasForeignKey(p => p.leaveTypeID);
                entity.HasIndex(e => e.leaveTypeName).IsUnique();
            });
            modelBuilder.Entity<loyaltyModel>(entity =>
            {
                entity.Property(e => e.loyaltyID).HasColumnName("loyaltyID");
                entity.Property(e => e.loyaltyName).HasColumnName("loyaltyName");
                entity.Property(e => e.loyaltyAmount).HasColumnName("loyaltyAmount");
                entity.Property(e => e.loyaltyCounter).HasColumnName("loyaltyCounter");
                entity.Property(e => e.loyaltyStatus).HasColumnName("loyaltyStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.LoyaltyHistories).WithOne(d => d.loyaltyModel).HasForeignKey(e => e.loyaltyID);
                entity.HasIndex(e =>e.loyaltyName).IsUnique();
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

                entity.HasOne(e => e.employmentModel).WithMany(d => d.LoyaltyHistories).HasForeignKey(e => e.employmentID);
                entity.HasOne(e => e.loyaltyModel).WithMany(p => p.LoyaltyHistories).HasForeignKey(e => e.loyaltyID);
            });

            modelBuilder.Entity<overtimeModel>(entity =>
            {
                entity.Property(e => e.overtimeID).HasColumnName("overtimeID");
                entity.Property(e => e.overtimeDescription).HasColumnName("overtimeDescription");
                entity.Property(e => e.overtimeName).HasColumnName("overtimeName");
                entity.Property(e => e.overtimeRate)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("overtimeRate");
                entity.Property(e => e.overtimeStatus).HasColumnName("overtimeStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
              
                entity.HasMany(e => e.OvertimeRecords).WithOne(p => p.overtimeModel).HasForeignKey(d => d.overtimeID);
                entity.HasIndex(e => e.overtimeName).IsUnique();
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
                entity.Property(e => e.oldBatchNbr).HasColumnName("oldBatchNbr");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.employmentModel).WithMany(p => p.OvertimeRecords).HasForeignKey(d => d.employmentID);
                entity.HasMany(e => e.OvertimeHistories).WithOne(p => p.overtimeRecordModel).HasForeignKey(d => d.overtimeRecordID);
                entity.HasOne(d => d.overtimeModel).WithMany(p => p.OvertimeRecords).HasForeignKey(d => d.overtimeID);
                entity.HasOne(or => or.departmentModel).WithMany(d => d.OvertimeRecords).HasForeignKey(d => d.departmentID);
                entity.HasIndex(d => new { d.employmentID, d.overtimeRecordDate, d.overtimeRecordStartTime, d.overtimeRecordEndTime, }).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<overtimeHistoryModel>(entity =>
            {
                entity.Property(e => e.overtimeHistoryID).HasColumnName("overtimeHistoryID");
                entity.Property(e => e.overtimeRecordID).HasColumnName("overtimeRecordID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.overtimeHistoryAction).HasColumnName("overtimeHistoryAction");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.overtimeRecordModel).WithMany(p => p.OvertimeHistories).HasForeignKey(d => d.overtimeRecordID);
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
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.addressModel).WithMany(p => p.Persons).HasForeignKey(d => d.addressID);
                entity.HasMany(d => d.Employments).WithOne(p => p.personModel).HasForeignKey(f => f.personID);
                entity.HasMany(d => d.PersonEducationLevels).WithOne(p => p.personModel).HasForeignKey(f => f.personID);
                entity.HasMany(e =>e.Banks).WithOne(p => p.personModel).HasForeignKey(d => d.personID);
                entity.HasIndex(e => new { e.personIDType, e.personIDNumber }).IsUnique();
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
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(d => d.personModel).WithMany(p => p.PersonHistories).HasForeignKey(d => d.personID);
                entity.HasOne(d => d.addressModel).WithMany(p => p.PersonHistories).HasForeignKey(d => d.addressID);
                entity.HasIndex(e => new { e.personIDType, e.personIDNumber }).IsUnique();
            });

            modelBuilder.Entity<personEducationLevelModel>(entity =>
            {
                entity.HasIndex(e => e.educationLevelID, "IX_PersonEducationLevels_educationLevelModeleducationLevelID");

                entity.HasIndex(e => e.personID, "IX_PersonEducationLevels_personModelpersonID");

                entity.Property(e => e.personEducationLevelID).HasColumnName("personEducationLevelID");
                entity.Property(e => e.educationLevelDate).HasColumnName("educationLevelDate");
                entity.Property(e => e.educationLevelID).HasColumnName("educationLevelID");
                entity.Property(e => e.educationLevelMark).HasColumnName("educationLevelMark");
                entity.Property(e => e.educationLevelNumber).HasColumnName("educationLevelNumber");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.educationLevelInstitutionName).HasColumnName("educationLevelInstitutionName");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.educationLevelModel).WithMany(p => p.PersonEducationLevels).HasForeignKey(d => d.educationLevelID);
                entity.HasOne(d => d.personModel).WithMany(e => e.PersonEducationLevels).HasForeignKey(d => d.personID);
                entity.HasIndex(d => new { d.educationLevelID, d.personID }).IsUnique();
            });

            modelBuilder.Entity<shiftModel>(entity =>
            {
                entity.Property(e => e.shiftID).HasColumnName("shiftID");
                entity.Property(e => e.shiftEnd).HasColumnName("shiftEnd");
                entity.Property(e => e.shiftName).HasColumnName("shiftName");
                entity.Property(e => e.shiftStart).HasColumnName("shiftStart");
                entity.Property(e => e.shiftStatus).HasColumnName("shiftStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.JobPlacements).WithOne(p => p.shiftModel).HasForeignKey(e => e.shiftID);
                entity.HasMany(e => e.Breaks).WithOne(p => p.shiftModel).HasForeignKey(e => e.shiftID);
                entity.HasIndex(e => e.shiftName).IsUnique();
            });

            modelBuilder.Entity<subAccountModel>(entity =>
            {
                entity.HasIndex(e => e.subAccountID, "IX_SubAccounts_accountModelaccountID");

                entity.Property(e => e.subAccountID).HasColumnName("subAccountID");
                entity.Property(e => e.accountID).HasColumnName("accountID");
                entity.Property(e => e.subAccountDescription).HasColumnName("subAccountDescription");
                entity.Property(e => e.subAccountName).HasColumnName("subAccountName");
                entity.Property(e => e.subAccountStatus).HasColumnName("subAccountStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.accountModel).WithMany(p => p.SubAccounts).HasForeignKey(d => d.accountID);
                entity.HasMany(e => e.Departments).WithOne(p => p.subAccountModel).HasForeignKey(d => d.subAccountID);
                entity.HasMany(e => e.WorkSites).WithOne(p => p.subAccountModel).HasForeignKey(d => d.subAccountID);
                entity.HasIndex(e => e.subAccountName).IsUnique();
            });
            modelBuilder.Entity<terminationModel>(entity =>
            {
                entity.Property(e => e.terminationID).HasColumnName("terminationID");
                entity.Property(e => e.terminationDate).HasColumnName("terminationDate");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.terminationReason).HasColumnName("terminationReason");
                entity.Property(e => e.terminationRemark).HasColumnName("terminationRemark");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(t => t.EmploymentModel).WithOne(e => e.TerminationModel).HasForeignKey<terminationModel>(d => d.employmentID);
                entity.HasIndex(e => e.employmentID).IsUnique();
            });

            modelBuilder.Entity<userModel>(entity =>
            {
                entity.Property(e => e.userID).HasColumnName("userID");
                entity.Property(e => e.userName).HasColumnName("userName");
                entity.Property(e => e.userStatus).HasColumnName("userStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.personModel).WithOne(p => p.userModel).HasForeignKey<userModel>(d => d.personID);
                entity.HasMany(e => e.UserHistories).WithOne(u => u.userModel).HasForeignKey(d => d.userID);
            });
            modelBuilder.Entity<userHistoryModel>(entity =>
            {
                entity.Property(e => e.userHistoryID).HasColumnName("userHistoryID");
                entity.Property(e => e.userID).HasColumnName("userID");
                entity.Property(e => e.userName).HasColumnName("userName");
                entity.Property(e => e.userStatus).HasColumnName("userStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.userModel).WithMany(u => u.UserHistories).HasForeignKey(d => d.userID);
            });
            modelBuilder.Entity<workSiteModel>(entity =>
            {
                entity.HasKey(e => e.workSiteID);

                entity.ToTable("workSiteModel");

                entity.HasIndex(e => e.addressID, "IX_workSiteModel_AddressModeladdressID");

                entity.Property(e => e.workSiteID).HasColumnName("workSiteID");
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.mapLink).HasColumnName("mapLink");
                entity.Property(e => e.workSiteEstablishDate).HasColumnName("workSiteEstablishDate");
                entity.Property(e => e.workSiteName).HasColumnName("workSiteName");
                entity.Property(e => e.workSiteNature).HasColumnName("workSiteNature");
                entity.Property(e => e.workSiteStatus).HasColumnName("worksiteStatus");
                entity.Property(e => e.workSiteCode).HasColumnName("workSiteCode");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.addressModel).WithMany(p => p.WorkSites).HasForeignKey(d => d.addressID);
                entity.HasMany(d => d.WorkSiteHistories).WithOne(p => p.workSiteModel).HasForeignKey(d => d.workSiteID);
                entity.HasOne(d => d.employmentModel).WithMany(e => e.WorkSites).HasForeignKey(d => d.employmentID);
                
                entity.HasIndex(d => d.workSiteName).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<workSiteHistoryModel>(entity =>
            {
                entity.Property(e => e.workSiteHistoryID).HasColumnName("workSiteHistoryID");
                entity.Property(e => e.workSiteID).HasColumnName("workSiteID");
                entity.Property(e => e.workSiteHistoryAction).HasColumnName("workSiteHistoryAction");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.worksiteName).HasColumnName("workSiteName");
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.workSiteModel).WithMany(p => p.WorkSiteHistories).HasForeignKey(d => d.workSiteID);
            });

        }
       
        public DbSet<accountModel> Accounts { get; set; }
        public DbSet<addressModel> Addresses { get; set; }
        public DbSet<allowanceAssignmentModel> AllowanceAssignments { get; set; }
        public DbSet<allowanceAssignmentHistoryModel> AllowanceAssignmentsHistories { get; set; }
        public DbSet<allowanceModel> Allowances { get; set; }
        public DbSet<bankInfoModel> BankInfos { get; set; }
        public DbSet<companyModel> Companies { get; set; }
        public DbSet<contractModel> Contracts { get; set; }
        public DbSet<contractHistoryModel> ContractHistories { get; set; }
        public DbSet<departmentModel> Departments { get; set; }
        public DbSet<educationLevelModel> EducationLevels { get; set; }
        public DbSet<employmentModel> Employments { get; set; }
        public DbSet<employmentHistoryModel> EmploymentHistories { get; set; }
        public DbSet<employmentMethodModel> EmploymentMethods { get; set; }
        public DbSet<employmentMethodHistoryModel> EmploymentMethodHistories { get; set; }
        public DbSet<employmentRequestModel> EmploymentRequests { get; set; }
        public DbSet<employmentRequestHistoryModel> EmploymentRequestHistories { get; set; }
        public DbSet<employmentTypeModel> EmploymentTypes { get; set; }
        public DbSet<experienceModel> Expriences { get; set; }
        public DbSet<holidayModel> Holidays { get; set; }
        public DbSet<jobModel> Jobs { get; set; }
        public DbSet<jobCategoryModel> JobCategories { get; set; }
        public DbSet<jobClassModel> JobClasses { get; set; }
        public DbSet<jobPlacementModel> JobPlacements { get; set; }
        public DbSet<jobGradeModel> JobGrades { get; set; }
        public DbSet<jobRequirementModel> JobRequirements { get; set; }
        public DbSet<jobRequirementHistoryModel> JobRequirementHistories { get; set; }
        public DbSet<leaveModel> Leaves { get; set; }
        public DbSet<leaveHistoryModel> LeaveHistories { get; set; }
        public DbSet<leaveTypeModel> LeaveTypes { get; set; }
        public DbSet<overtimeModel> Overtimes { get; set; }
        public DbSet<overtimeRecordModel> OvertimeRecords { get; set; }
        public DbSet<overtimeHistoryModel> OvertimeHistories { get; set; }
        public DbSet<personModel> Persons { get; set; }
        public DbSet<personHistoryModel> PersonHistories { get; set; }
        public DbSet<personEducationLevelModel> PersonEducationLevels { get; set; }
        public DbSet<shiftModel> Shifts { get; set; }
        public DbSet<subAccountModel> SubAccounts { get; set; }
        public DbSet<terminationModel> Terminations { get; set; }
        public DbSet<PIS2.Models.breakModel> Breaks { get; set; } = default!;
        public DbSet<PIS2.Models.userModel> Users { get; set; } = default!;
        public DbSet<PIS2.Models.userHistoryModel> UserHistories { get; set; } = default!;
        public DbSet<workSiteModel> WorkSites { get; set; }
        public DbSet<workSiteHistoryModel> WorkSitesHistories { get; set; }
        public DbSet<PIS2.Models.loyaltyModel> Loyalties { get; set; } = default!;
        public DbSet<PIS2.Models.loyaltyHistoryModel> LoyaltyHistories { get; set; } = default!;

    }
}
