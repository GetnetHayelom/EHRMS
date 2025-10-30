using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PIS2.Models;
using PIS2.Views;

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
            modelBuilder.Entity<accessModel>(entity =>
            {
                entity.HasKey(a => a.accessID);

                entity.HasOne(a => a.userModel).WithMany(u => u.Accesses).HasForeignKey(a => a.userID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.CompanyModel).WithMany(c => c.Accesses).HasForeignKey(a => a.companyID).OnDelete(DeleteBehavior.SetNull);
                entity.Property(a => a.modifiedBy).IsRequired().HasMaxLength(100);

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });

            // ------------------------------
            // accessHistoryModel configuration
            // ------------------------------
            modelBuilder.Entity<accessHistoryModel>(entity =>
            {
                entity.HasKey(h => h.accessHistoryID);

                entity.HasOne(h => h.accessModel).WithMany(a => a.AccessHistories).HasForeignKey(h => h.accessID).OnDelete(DeleteBehavior.Cascade);             
                entity.Property(h => h.modifiedBy).IsRequired().HasMaxLength(100);
;
            });


            modelBuilder.Entity<accountModel>(entity =>
            {
                entity.Property(e => e.accountID).HasColumnName("accountID");
                entity.Property(e => e.accountNumber).HasColumnName("accountNumber");
                entity.Property(e => e.accountDescription).HasColumnName("accountDescription");
                entity.Property(e => e.accountName).HasColumnName("accountName");
                entity.Property(e => e.accountStatus).HasColumnName("accountStatus").HasConversion<int>();
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                
                entity.HasMany(e => e.SubAccounts).WithOne(p => p.accountModel).HasForeignKey(e => e.accountID).OnDelete(DeleteBehavior.Cascade);

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
                entity.Property(e => e.allowanceDuration).HasColumnName("allowanceDuration");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.AllowanceAssignments).WithOne(p => p.allowanceModel).HasForeignKey(e => e.allowanceID).OnDelete(DeleteBehavior.Cascade);

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
                entity.Property(e => e.allowanceAssignmentEndDate).HasColumnName("allowanceAssignmentEndDate").IsRequired(false);
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.allowanceModel).WithMany(p => p.AllowanceAssignments).HasForeignKey(d => d.allowanceID);
                entity.HasMany(d => d.AllowanceAssignmentHistories).WithOne(p => p.allowanceAssignmentModel).HasForeignKey(p => p.allowanceAssignmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.employmentModel).WithMany(e => e.AllowanceAssignments).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);


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

                entity.HasOne(e=>e.allowanceAssignmentModel).WithMany(d=>d.AllowanceAssignmentHistories).HasForeignKey(e => e.allowanceAssignmentID);

            });
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.AuditID).HasColumnName("AuditID");
                entity.Property(e => e.TableName).HasColumnName("TableName");
                entity.Property(e => e.RecordID).HasColumnName("RecordID");
                entity.Property(e => e.ColumnName).HasColumnName("ColumnName");
                entity.Property(e => e.OldValue).HasColumnName("OldValue");
                entity.Property(e => e.NewValue).HasColumnName("NewValue");
                entity.Property(e => e.ModifiedBy).HasColumnName("ModifiedBy");
                entity.Property(e => e.ModifiedDate).HasColumnName("ModifiedDate");

            });


            modelBuilder.Entity<bankInfoModel>(entity =>
            {
                entity.HasIndex(e => e.personID, "IX_BankInfos_personModelpersonID");

                entity.Property(e => e.bankInfoID).HasColumnName("bankInfoID");
                entity.Property(e => e.bankAccountNumber).HasColumnName("bankAccountNumber");
                entity.Property(e => e.bankBranch).HasColumnName("bankBranch");
                entity.Property(e => e.bankName).HasColumnName("bankName");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.bankInfoStatus).HasColumnName("bankInfoStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.personModel).WithMany(p => p.Banks).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.Cascade);
                
                //entity.HasIndex(d => new { d.personID, d.bankName, d.bankInfoStatus}).IsUnique().HasFilter("[bankInfoStatus] = 1");
                //entity.HasIndex(d => new { d.bankAccountNumber, d.personID, d.bankName, d.bankInfoStatus }).IsUnique();
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

                entity.HasOne(d => d.shiftModel).WithMany(p => p.Breaks).HasForeignKey(d => d.shiftID).OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(d => new { d.breakEnd, d.breakStart, d.shiftID }).IsUnique();
            });

            modelBuilder.Entity<businessUnitModel>(entity =>
            {

                entity.Property(e => e.businessUnitID).HasColumnName("businessUnitID");
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.businessUnitName).HasColumnName("businessUnitName");
                entity.Property(e => e.businessUnitAlias).HasColumnName("businessUnitAlias");
                entity.Property(e => e.businessUnitStatus).HasColumnName("businessUnitStatus");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.Departments).WithOne(p => p.businessUnitModel).HasForeignKey(p => p.businessUnitID).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.companyModel).WithMany(p => p.BusinessUnits).HasForeignKey(p => p.companyID).OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(e => e.businessUnitName).IsUnique();
                entity.HasIndex(e => e.businessUnitAlias).IsUnique();

                //entity.HasOne(e => e.employmentModel).WithOne(p => p.companyModel).HasForeignKey<companyModel>(p => p.employmentID).OnDelete(DeleteBehavior.ClientSetNull); ;
            });
            modelBuilder.Entity<companyModel>(entity =>
            {
                
                entity.Property(e => e.companyID).HasColumnName("companyID");
                entity.Property(e => e.addressID).HasColumnName("addressID");
                entity.Property(e => e.companyName).HasColumnName("companyName");
                entity.Property(e => e.companyAlias).HasColumnName("companyAlias");
                entity.Property(e => e.companyStatus).HasColumnName("companyStatus");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.Departments).WithOne(p => p.companyModel).HasForeignKey(p => p.companyID).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.BusinessUnits).WithOne(p => p.companyModel).HasForeignKey(p => p.companyID);
                entity.HasOne(e => e.addressModel).WithMany(p => p.Companies).HasForeignKey(p => p.addressID);

                entity.HasIndex(e => e.companyName).IsUnique();
                
                //entity.HasOne(e => e.employmentModel).WithOne(p => p.companyModel).HasForeignKey<companyModel>(p => p.employmentID).OnDelete(DeleteBehavior.ClientSetNull); ;
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

                entity.HasMany(c => c.ContractHistories).WithOne(ch => ch.contractModel).HasForeignKey(p => p.contractID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.employmentModel).WithOne(e => e.contractModel).HasForeignKey<contractModel>(p => p.employmentID).OnDelete(DeleteBehavior.Cascade);

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

                entity.HasOne(ch => ch.contractModel).WithMany(c => c.ContractHistories).HasForeignKey(p => p.contractID);
                
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

                entity.HasOne(e => e.FromEmployment).WithMany(e => e.delegationsFrom).HasForeignKey(e => e.delegationFrom).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ToEmployment).WithMany(e => e.delegationsTo).HasForeignKey(e => e.delegationTo).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(d => d.DelegationHistories).WithOne(d => d.delegationModel).HasForeignKey(d => d.delegationID);

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

                entity.HasOne(e => e.delegationModel).WithMany(e => e.DelegationHistories).HasForeignKey(e => e.delegationID);
               
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

                entity.HasOne(d => d.companyModel).WithMany(p => p.Departments).HasForeignKey(d => d.companyID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.businessUnitModel).WithMany(p => p.Departments).HasForeignKey(d => d.businessUnitID);

                // entity.HasOne(d => d.employmentModel).WithOne(p => p.departmentModel)
                //    .HasForeignKey<departmentModel>(d => d.employmentID)
                //  .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.subAccountModel).WithMany(p => p.Departments).HasForeignKey(d => d.subAccountID);
                entity.HasMany(e =>e.JobPlacements).WithOne(p => p.departmentModel).HasForeignKey(d => d.departmentID);
                entity.HasMany(d => d.OvertimeRecords).WithOne(or => or.departmentModel).HasForeignKey(d => d.departmentID);
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

                entity.HasOne(e => e.departmentModel).WithMany(e => e.DepartmentHistories).HasForeignKey(d => d.departmentID);
                
            });

            modelBuilder.Entity<disciplineModel>(entity =>
            {
                entity.ToTable("Disciplines");
                entity.Property(e => e.disciplineName)
                      .IsRequired()
                      .HasMaxLength(100);
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

                entity.HasMany(d => d.PersonEducationLevels).WithOne(p => p.educationLevelModel).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.Jobs).WithMany(p => p.EducationLevels);

                entity.HasIndex(d => d.educationLevelName).IsUnique();
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
                entity.HasMany(d => d.EmploymentHistories).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.OvertimeRecords).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.JobPlacements).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.Leaves).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.AllowanceAssignments).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.LoyaltyHistories).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.WorkSites).WithOne(e => e.employmentModel).HasForeignKey(d => d.employmentID);
                entity.HasOne(e => e.employmentMethodModel).WithMany(em => em.Employments).HasForeignKey(d => d.employmentID);
                entity.HasOne(e => e.employmentRequestModel).WithMany(er => er.Employments).HasForeignKey(d => d.employmentRequestID);
                

                entity.HasOne(d => d.employmentTypeModel).WithMany(e => e.Employments).HasForeignKey(d => d.employmentTypeID).OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(d => d.personID).HasFilter("[employmentStatus] = 1").IsUnique();
                //entity.HasIndex(d => d.givenID).IsUnique();

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

                entity.HasOne(e => e.employmentModel).WithMany(d => d.EmploymentHistories).HasForeignKey(p => p.employmentID).OnDelete(DeleteBehavior.Cascade);
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

                entity.HasOne(emh => emh.employmentMethodModel).WithMany(em => em.EmploymentMethodHistories).HasForeignKey(p => p.employmentMethodID);
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

                entity.ToTable(t => t.UseSqlOutputClause(false));
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

                entity.HasOne(erh => erh.employmentRequestModel).WithMany(er => er.EmploymentRequestHistories).HasForeignKey(p => p.employmentRequestID).OnDelete(DeleteBehavior.Cascade);
                
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
                entity.Property(e => e.isPensionAllowed).HasColumnName("isPensionAllowed");
                entity.Property(e => e.employmentMinAge).HasColumnName("employmentMinAge");
                entity.Property(e => e.employmentMaxAge).HasColumnName("employmentMaxAge");
                entity.Property(e => e.maxLeaveIncrement).HasColumnName("maxLeaveIncrement");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.EmploymentHistories).WithOne(p => p.employmentTypeModel).HasForeignKey(p => p.employmentTypeID);
                entity.HasMany(e => e.Employments).WithOne(p => p.employmentTypeModel).HasForeignKey(p => p.employmentTypeID);
                entity.HasMany(et => et.EmploymentRequests).WithOne(er => er.employmentTypeModel).HasForeignKey(p =>p.employmentTypeID);

                entity.HasIndex(e => e.employmentTypeName).IsUnique();
            });

            modelBuilder.Entity<experienceModel>(e =>
            {
                e.Property(e => e.experienceID).HasColumnName("exprienceID");
                e.Property(e => e.personID).HasColumnName("personID");
                e.Property(e => e.experienceStartDate).HasColumnName("experienceStartDate");
                e.Property(e => e.experienceEndDate).HasColumnName("experienceEndDate");
                e.Property(e => e.jobTitle).HasColumnName("jobTitle");
                e.Property(e => e.jobGrade).HasColumnName("jobGrade");
                e.Property(e => e.jobStep).HasColumnName("jobStep");
                e.Property(e => e.jobSalary).HasColumnName("jobSalary");
                e.Property(e => e.jobDepartment).HasColumnName("jobDepartment");
                e.Property(e => e.experienceType).HasColumnName("experienceType");
                e.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                e.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

            });
            modelBuilder.Entity<familyModel>(entity =>
            {
                entity.Property(e => e.familyID).HasColumnName("familyID");
                entity.Property(e => e.personID2).HasColumnName("personID2");
                entity.Property(e => e.personID).HasColumnName("personID");
                entity.Property(e => e.relation).HasColumnName("relation");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                //entity.HasOne(e => e.personModel2).WithMany(e => e.Families).HasForeignKey(e => e.personID2);
                entity.HasOne(e => e.personModel).WithMany(e => e.Families).HasForeignKey(e => e.personID);

                entity.HasIndex(e => new { e.personID, e.personID2 }).IsUnique();
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

                entity.HasMany(e => e.GuarantyHistories).WithOne(g => g.guarantyModel).HasForeignKey(g => g.guarantyID);
                entity.HasOne(g => g.Employment).WithMany(e => e.Guaranties).HasForeignKey(g => g.employmentID);

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

                entity.HasOne(g => g.guarantyModel).WithMany(e => e.GuarantyHistories).HasForeignKey(g => g.guarantyID);
            });

            modelBuilder.Entity<jobModel>(entity =>
            {

                entity.HasIndex(e => e.jobCategoryID, "IX_Jobs_jobCategoryModeljobCategoryID");

                entity.HasIndex(e => e.jobClassID, "IX_Jobs_jobClassModelJobClassId");

                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.jobCategoryID).HasColumnName("jobCategoryID");
                entity.Property(e => e.jobClassID).HasColumnName("jobClassID");
                entity.Property(e => e.jobDescription).HasColumnName("jobDescription");
                entity.Property(e => e.jobGradeID).HasColumnName("jobGradeID");                
                entity.Property(e => e.jobStatus).HasColumnName("jobStatus");
                entity.Property(e => e.jobTitle).HasColumnName("jobTitle");
                entity.Property(e => e.jobCode).HasColumnName("jobCode");
                entity.Property(e => e.jobQualifications).HasColumnName("jobQualifications");
                entity.Property(e => e.jobExperience).HasColumnName("jobExperience");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.jobCategoryModel).WithMany(p => p.Jobs).HasForeignKey(p => p.jobCategoryID);
                entity.HasOne(d => d.jobClassModel).WithMany(p => p.Jobs).HasForeignKey(d => d.jobClassID);
                entity.HasMany(d => d.EducationLevels).WithMany(p => p.Jobs);
                entity.HasOne(d => d.jobGradeModel).WithMany(e => e.Jobs).HasForeignKey(p => p.jobGradeID);
                entity.HasMany(j => j.EmploymentRequests).WithOne(er => er.jobModel).HasForeignKey(p => p.jobID);

                //entity.HasIndex(d => d.jobTitle).HasFilter("jobStatus =1").IsUnique();
                entity.HasIndex(d => d.jobCode).IsUnique().HasFilter("[jobCode] IS NOT NULL");
            });
            modelBuilder.Entity<jobGradeModel>(entity =>
            {
                entity.Property(e => e.jobGradeID).HasColumnName("jobGradeID");
                entity.Property(e => e.jobGradeName).HasColumnName("jobGradeName");
                entity.Property(e => e.jobGradeDescription).HasColumnName("jobGradeDescription");
                entity.Property(e => e.jobGradeBasicSalary).HasColumnName("jobGradeBasicSalary");
                entity.Property(e => e.jobGradeMidSalary).HasColumnName("jobGradeMidSalary");
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
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.jobID).HasColumnName("jobID");
                entity.Property(e => e.jobPlacementDate).HasColumnName("jobPlacementDate");
                entity.Property(e => e.jobPlacementReference).HasColumnName("jobPlacementReference");
                entity.Property(e => e.jobPlacementSalary)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("jobPlacementSalary");
                entity.Property(e => e.jobPlacementStatus).HasColumnName("jobPlacementStatus");
                entity.Property(e => e.jobPlacementReason).HasColumnName("jobPlacementReason");
                entity.Property(e => e.jobStepID).HasColumnName("jobStepID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.departmentModel).WithMany(p => p.JobPlacements)
                    .HasForeignKey(d => d.departmentID);

                entity.HasOne(d => d.employmentModel).WithMany(p => p.JobPlacements)
                    .HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.NoAction);
                    

                entity.HasOne(d => d.jobModel).WithMany(p => p.JobPlacements).HasForeignKey(d => d.jobID);
                //entity.HasOne(e => e.jobStepModel).WithMany(e => e.JobPlacements).HasForeignKey(e => e.jobStepID).OnDelete(DeleteBehavior.NoAction);
                
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

                entity.HasOne(e => e.jobPlacementModel).WithMany(e => e.JobPlacementHistories).HasForeignKey(e => e.jobPlacementID);
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

                entity.HasMany(e => e.JobRequirementHistories).WithOne(p => p.JobRequirementModel).HasForeignKey(p => p.jobRequirementID);
                entity.HasOne(jr => jr.DepartmentModel).WithMany(d => d.JobRequirements).HasForeignKey(jr => jr.departmentID);
                entity.HasOne(jr => jr.JobModel).WithMany(j => j.JobRequirements).HasForeignKey(j => j.jobID);

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

                entity.HasOne(jr => jr.JobRequirementModel).WithMany(jr => jr.JobRequirementHistories).HasForeignKey(jr => jr.jobRequirementID).OnDelete(DeleteBehavior.Cascade);
                
            });

      

            modelBuilder.Entity<jobReqCost>(entity =>
            {
                entity.Property(e => e.jobReqCostID).HasColumnName("jobReqCostID");
                entity.Property(e => e.jobRequirementID).HasColumnName("jobRequiremenetID");
                entity.Property(e => e.jobReqStatus).HasColumnName("jobReqStatus");
                entity.Property(e => e.jobReqCostReason).HasColumnName("jobReqCostReason");
                entity.Property(e => e.jobReqCostEstimate).HasColumnName("jobReqCostEstimate");
                entity.Property(e => e.jobReqCostActual).HasColumnName("jobReqCostActual");
                entity.Property(e => e.jobReqCostReference).HasColumnName("jobRequirementStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(jr => jr.JobRequirementModel).WithMany(jr => jr.JobReqCosts).HasForeignKey(jr => jr.jobRequirementID);
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

                entity.HasMany(e => e.JobStepHistories).WithOne(e => e.jobStepModel).HasForeignKey(e => e.jobStepID);
                entity.HasOne(e => e.jobGradeModel).WithMany(e => e.JobSteps).HasForeignKey(e => e.jobGradeID);

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

                entity.HasOne(e => e.jobStepModel).WithMany(e => e.JobStepHistories).HasForeignKey(e => e.jobStepID);

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
                
                entity.HasOne(d => d.employmentModel).WithMany(p => p.Leaves).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.leaveTypeModel).WithMany(p => p.Leaves).HasForeignKey(d => d.leaveTypeID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.LeaveHistories).WithOne(p => p.leaveModel).HasForeignKey(d => d.leaveID).OnDelete(DeleteBehavior.Cascade);
                

                //entity.HasCheckConstraint("CK_Leave_leaveEndDate",
                //    "[leaveEndDate]>=[leaveStartDate]");

                //entity.HasCheckConstraint("CK_Leave_NumberOfDays",
                //    "leaveDays>0 AND leaveDays <= DATEDIFF(DAY, leaveStartDate, leaveEndDate)+1");

                //entity.HasIndex(d => new { d.employmentID, d.leaveStartDate, d.leaveEndDate}).IsUnique();

                entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<leaveHistoryModel>(entity =>
            {
                entity.Property(e => e.leaveHistoryID).HasColumnName("leaveHistoryID");
                entity.Property(e => e.leaveID).HasColumnName("leaveID");
                entity.Property(e => e.leaveHistoryAction).HasColumnName("leaveHistoryAction");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.leaveModel).WithMany(p => p.LeaveHistories).HasForeignKey(e => e.leaveID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<leaveTypeModel>(entity =>
            {
                entity.Property(e => e.leaveTypeID).HasColumnName("leaveTypeID");
                entity.Property(e => e.leaveTypeImpact).HasColumnName("leaveTypeImpact");
                entity.Property(e => e.leaveTypeName).HasColumnName("leaveTypeName");
                entity.Property(e => e.leaveTypeStatus).HasColumnName("leaveTypeStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.leaveAvailability).HasColumnName("leaveAvailability");
                entity.Property(e => e.leaveGroup).HasColumnName("leaveGroup");
                entity.Property(e => e.leaveJob).HasColumnName("leaveJob");
                entity.Property(e => e.leaveLegality).HasColumnName("leaveLegality");

                entity.HasMany(p => p.Leaves).WithOne(e => e.leaveTypeModel).HasForeignKey(p => p.leaveTypeID).OnDelete(DeleteBehavior.Cascade);

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

                entity.HasMany(e => e.LoyaltyHistories).WithOne(d => d.loyaltyModel).HasForeignKey(e => e.loyaltyID).OnDelete(DeleteBehavior.Cascade);

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

                entity.HasOne(e => e.employmentModel).WithMany(d => d.LoyaltyHistories).HasForeignKey(e => e.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.loyaltyModel).WithMany(p => p.LoyaltyHistories).HasForeignKey(e => e.loyaltyID).OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(d => new {d.loyaltyID, d.employmentID}).IsUnique();
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

                entity.Property(e => e.noticePostedBy)
                    .HasMaxLength(100)
                    .HasColumnName("noticePostedBy");

                entity.Property(e => e.noticeApprovedBy)
                    .HasMaxLength(100)
                    .HasColumnName("noticeApprovedBy");

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

                entity.Property(e => e.AttachmentPath)
                    .HasMaxLength(255)
                    .HasColumnName("AttachmentPath");

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
              
                entity.HasMany(e => e.OvertimeRecords).WithOne(p => p.overtimeModel).HasForeignKey(d => d.overtimeID).OnDelete(DeleteBehavior.Cascade);

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
                entity.Property(e => e.overtimeRecordEmploymentRate).HasColumnName("overtimeRecordEmploymentRate");
                entity.Property(e => e.overtimeRate).HasColumnName("overtimeRate");
                entity.Property(e => e.oldBatchNbr).HasColumnName("oldBatchNbr");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.employmentModel).WithMany(p => p.OvertimeRecords).HasForeignKey(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.OvertimeHistories).WithOne(p => p.overtimeRecordModel).HasForeignKey(d => d.overtimeRecordID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.overtimeModel).WithMany(p => p.OvertimeRecords).HasForeignKey(d => d.overtimeID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(or => or.departmentModel).WithMany(d => d.OvertimeRecords).HasForeignKey(d => d.departmentID).OnDelete(DeleteBehavior.Cascade);

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

                entity.HasOne(e => e.overtimeRecordModel).WithMany(p => p.OvertimeHistories).HasForeignKey(d => d.overtimeRecordID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<penaltyModel>(entity => {
                entity.Property(e => e.penaltyID).HasColumnName("penaltyID");
                entity.Property(e => e.penaltyTypeID).HasColumnName("penaltyTypeID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.penaltyIssueDate).HasColumnName("penaltyIssueDate");
                entity.Property(e => e.penaltyStartDate).HasColumnName("penaltyStartDate");
                entity.Property(e => e.penaltyEndDate).HasColumnName("penaltyEndDate");
                entity.Property(e => e.penaltyReference).HasColumnName("penaltyReference");
                entity.Property(e => e.penaltyReason).HasColumnName("penaltyReason");
                entity.Property(e => e.penaltyStatus).HasColumnName("penaltyStatus");
                entity.Property(e => e.penaltyAmount).HasColumnName("penaltyAmount");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.employmentModel).WithMany(e => e.Penalties).HasForeignKey(e => e.employmentID);
                entity.HasOne(e => e.penaltyTypeModel).WithMany(e => e.Penalties).HasForeignKey(e => e.penaltyTypeID);
                entity.HasMany(e => e.PenaltyHistories).WithOne(e => e.penaltyModel).HasForeignKey(e => e.penaltyID);

                entity.ToTable(t => t.UseSqlOutputClause(false));
            });
            modelBuilder.Entity<penaltyHistoryModel>(entity => {
                entity.Property(e => e.penaltyHistoryID).HasColumnName("penaltyHistoryID");
                entity.Property(e => e.penaltyID).HasColumnName("penaltyID");
                entity.Property(e => e.penaltyStatus).HasColumnName("penaltyStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.penaltyModel).WithMany(e => e.PenaltyHistories).HasForeignKey(e => e.penaltyID);
                
            });
            modelBuilder.Entity<penaltyTypeModel>(entity => {
                entity.Property(e => e.penaltyTypeID).HasColumnName("penaltyTypeID");
                entity.Property(e => e.penaltyName).HasColumnName("penaltyName");
                entity.Property(e => e.penaltyTypeStatus).HasColumnName("penaltyTypeStatus");
                entity.Property(e => e.penaltyMethod).HasColumnName("penaltyMethod");
                entity.Property(e => e.penaltyRate).HasColumnName("penaltyRate");
                entity.Property(e => e.penaltyValidity).HasColumnName("penaltyValidity");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasMany(p => p.Penalties).WithOne(e => e.penaltyTypeModel).HasForeignKey(e => e.penaltyTypeID);
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
                entity.HasMany(d => d.Employments).WithOne(p => p.personModel).HasForeignKey(f => f.personID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(d => d.PersonEducationLevels).WithOne(p => p.personModel).HasForeignKey(f => f.personID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e =>e.Banks).WithOne(p => p.personModel).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.Cascade);

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

                entity.HasOne(d => d.personModel).WithMany(p => p.PersonHistories).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.addressModel).WithMany(p => p.PersonHistories).HasForeignKey(d => d.addressID);
                
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
                entity.Property(e => e.educationField).HasColumnName("educationField");
                entity.Property(e => e.educationDomain).HasColumnName("educationDomain");
                entity.Property(e => e.educationDiscipline).HasColumnName("educationDiscipline");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(d => d.educationLevelModel).WithMany(p => p.PersonEducationLevels).HasForeignKey(d => d.educationLevelID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.personModel).WithMany(e => e.PersonEducationLevels).HasForeignKey(d => d.personID).OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(d => new { d.educationLevelID, d.personID, d.educationField}).IsUnique();
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

                e.HasMany(p => p.ProhibitionHistories).WithOne(p => p.prohibitionModel).HasForeignKey(p => p.prohibitionID);
                e.HasOne(p => p.employmentModel).WithMany(p => p.Prohibitions).HasForeignKey(p => p.employmentID);

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

                e.HasOne(p => p.prohibitionModel).WithMany(p => p.ProhibitionHistories).HasForeignKey(p => p.prohibitionID);               

            });

            modelBuilder.Entity<serviceRequestModel>(entity =>
            {
                entity.Property(e => e.serviceRequestID).HasColumnName("serviceRequestID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.serviceRequestDate).HasColumnName("serviceRequestDate");
                entity.Property(e => e.requestedService).HasColumnName("requestedService");
                entity.Property(e => e.serviceRequestStatus).HasColumnName("serviceRequestStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.Employment).WithMany(e => e.ServiceRequests).HasForeignKey(e => e.employmentID);
                entity.HasMany(e => e.ServiceRequestHistoies).WithOne(e => e.ServiceRequest).HasForeignKey(e => e.serviceRequestID);

                entity.HasIndex(d => new { d.employmentID, d.requestedService }).HasFilter("serviceRequestStatus =1");

                entity.ToTable(t => t.UseSqlOutputClause(false));

            });
            modelBuilder.Entity<serviceRequestHistoryModel>(entity =>
            {
                entity.Property(e => e.serviceRequestHistoryID).HasColumnName("serviceRequestHistoryID");
                entity.Property(e => e.serviceRequestID).HasColumnName("serviceRequestID");
                entity.Property(e => e.serviceRequestStatus).HasColumnName("serviceRequestStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.ServiceRequest).WithMany(e => e.ServiceRequestHistoies).HasForeignKey(e => e.serviceRequestID);

            });

            modelBuilder.Entity<shiftModel>(entity =>
            {
                entity.Property(e => e.shiftID).HasColumnName("shiftID");
                entity.Property(e => e.shiftEnd).HasColumnName("shiftEnd");
                entity.Property(e => e.shiftName).HasColumnName("shiftName");
                entity.Property(e => e.shiftStart).HasColumnName("shiftStart");
                entity.Property(e => e.shiftStatus).HasColumnName("shiftStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                
                entity.HasMany(e => e.Breaks).WithOne(p => p.shiftModel).HasForeignKey(e => e.shiftID).OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.shiftName).IsUnique();
                entity.HasIndex(e => new { e.shiftStart, e.shiftEnd }).IsUnique();
            });

            modelBuilder.Entity<shiftAssignmentModel>(entity =>
            {
                entity.Property(e => e.shiftAssignmentID).HasColumnName("shiftAssignmentID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.shiftID).HasColumnName("shiftID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.shiftModel).WithMany(e => e.ShiftAssignments).HasForeignKey(e => e.shiftID);
                entity.HasOne(e => e.EmploymentModel).WithMany(e => e.ShiftAssignments).HasForeignKey(e => e.employmentID);


            });
            modelBuilder.Entity<siteAssignmentModel>(entity =>
            {
                entity.Property(e => e.siteAssignmentID).HasColumnName("siteAssignmentID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.workSiteID).HasColumnName("workSiteID");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.workSiteModel).WithMany(e => e.SiteAssignments).HasForeignKey(e => e.workSiteID);
                entity.HasOne(e => e.employmentModel).WithMany(e => e.SiteAssignments).HasForeignKey(e => e.employmentID);

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

                entity.HasMany(e => e.StructureHistories).WithOne(e => e.structureModel).HasForeignKey(e => e.structureID);
                entity.HasOne(e => e.departmentModel).WithMany(e => e.Structures).HasForeignKey(e => e.departmentID);
                entity.HasOne(e => e.jobModel).WithMany(e => e.Structures).HasForeignKey(e => e.jobID);
                
                entity.HasOne(s => s.ReportsTo).WithMany(s => s.Subordinates).HasForeignKey(s => s.reportsTo).OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t => t.UseSqlOutputClause(false));

            });

            modelBuilder.Entity<structureHistoryModel>(entity =>
            {
                entity.Property(e => e.structureHistoryID).HasColumnName("structureHistoryID");
                entity.Property(e => e.structureID).HasColumnName("structureID");
                entity.Property(e => e.structureStatus).HasColumnName("structureStatus");
                entity.Property(e => e.requiredNumber).HasColumnName("requiredNumber");
                entity.Property(e => e.reportsTo).HasColumnName("reportsTo");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");
                entity.Property(e => e.modifiedDate).HasColumnName("modifiedDate");

                entity.HasOne(e => e.structureModel).WithMany(e => e.StructureHistories).HasForeignKey(e => e.structureID);
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

                entity.HasOne(d => d.accountModel).WithMany(p => p.SubAccounts).HasForeignKey(d => d.accountID).OnDelete(DeleteBehavior.Cascade);
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
                entity.Property(e => e.terminationStatus).HasColumnName("terminationStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(t => t.EmploymentModel).WithOne(e => e.TerminationModel).HasForeignKey<terminationModel>(d => d.employmentID).OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.employmentID).IsUnique();
                entity.ToTable(tb => tb.UseSqlOutputClause(false));
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

                entity.HasOne(e => e.personModel).WithOne(p => p.userModel).HasForeignKey<userModel>(d => d.personID).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.UserHistories).WithOne(u => u.userModel).HasForeignKey(d => d.userID);

                entity.HasIndex(d => d.userName).IsUnique();

                entity.ToTable(t => t.UseSqlOutputClause(false));
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

                entity.HasOne(e => e.userModel).WithMany(u => u.UserHistories).HasForeignKey(d => d.userID).OnDelete(DeleteBehavior.Cascade);
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
                entity.HasMany(d => d.WorkSiteHistories).WithOne(p => p.workSiteModel).HasForeignKey(d => d.workSiteID).OnDelete(DeleteBehavior.Cascade);
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

                entity.HasOne(e => e.workSiteModel).WithMany(p => p.WorkSiteHistories).HasForeignKey(d => d.workSiteID).OnDelete(DeleteBehavior.Cascade);
            });
            //
            /// <summary>
            /// Payroll related
            /// </summary>
            // payrollModel
            modelBuilder.Entity<payrollModel>(entity =>
            {
                entity.Property(e => e.payrollID).HasColumnName("payrollID");
                entity.Property(e => e.payrollName).HasColumnName("payrollName");
                entity.Property(e => e.StartDate).HasColumnName("StartDate");
                entity.Property(e => e.EndDate).HasColumnName("EndDate");
                entity.Property(e => e.payrollStatus).HasColumnName("payrollStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasMany(e => e.payrollHistories)
                      .WithOne(p => p.payrollModel)
                      .HasForeignKey(p => p.payrollID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // payrollHistory
            modelBuilder.Entity<payrollHistory>(entity =>
            {
                entity.Property(e => e.payrollHistoryID).HasColumnName("payrollHistoryID");
                entity.Property(e => e.payrollID).HasColumnName("payrollID");
                entity.Property(e => e.payrollStatus).HasColumnName("payrollStatus");
                entity.Property(e => e.modfiedDate).HasColumnName("modfiedDate");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.payrollModel)
                      .WithMany(p => p.payrollHistories)
                      .HasForeignKey(e => e.payrollID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // payrollPay
            modelBuilder.Entity<payrollPay>(entity =>
            {
                entity.Property(e => e.payrollPayID).HasColumnName("payrollPayID");
                entity.Property(e => e.payrollID).HasColumnName("payrollID");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.GrossPay).HasColumnName("GrossPay");
                entity.Property(e => e.NetPay).HasColumnName("NetPay");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.payrollModel)
                      .WithMany()
                      .HasForeignKey(e => e.payrollID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.EmploymentModel)
                      .WithMany()
                      .HasForeignKey(e => e.employmentID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // earningModel
            modelBuilder.Entity<earningModel>(entity =>
            {
                entity.Property(e => e.earningID).HasColumnName("earningID");
                entity.Property(e => e.EarningTypeId).HasColumnName("EarningTypeId");
                entity.Property(e => e.earningReference).HasColumnName("earningReference");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.earningAmount).HasColumnName("earningAmount");
                entity.Property(e => e.payrollID).HasColumnName("payrollID");
                entity.Property(e => e.earningStatus).HasColumnName("earningStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.EarningType)
                      .WithMany(p => p.Earnings)
                      .HasForeignKey(e => e.EarningTypeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.EmploymentModel)
                      .WithMany()
                      .HasForeignKey(e => e.employmentID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PayrollModel)
                      .WithMany()
                      .HasForeignKey(e => e.payrollID)
                      .OnDelete(DeleteBehavior.SetNull); // since payrollID is nullable
            });

            // earningType
            modelBuilder.Entity<earningType>(entity =>
            {
                entity.Property(e => e.earningTypeID).HasColumnName("earningTypeID");
                entity.Property(e => e.earningName).HasColumnName("earningName");
                entity.Property(e => e.isRecurring).HasColumnName("isRecurring");
                entity.Property(e => e.isTaxable).HasColumnName("isTaxable");
                entity.Property(e => e.Status).HasColumnName("Status");
            });

            // deductionModel
            modelBuilder.Entity<deductionModel>(entity =>
            {
                entity.HasKey(e => e.deductionID);

                entity.Property(e => e.deductionID).HasColumnName("deductionID");
                entity.Property(e => e.deductionTypeID).HasColumnName("deductionTypeID");
                entity.Property(e => e.deductionReference).HasColumnName("deductionReference");
                entity.Property(e => e.employmentID).HasColumnName("employmentID");
                entity.Property(e => e.deductionAmount).HasColumnName("deductionAmount");
                entity.Property(e => e.payrollID).HasColumnName("payrollID");
                entity.Property(e => e.deductionStatus).HasColumnName("deductionStatus");
                entity.Property(e => e.modifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(e => e.DeductionType)
                      .WithMany(p => p.Deductions)
                      .HasForeignKey(e => e.deductionTypeID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.EmploymentModel)
                      .WithMany()
                      .HasForeignKey(e => e.employmentID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.payrollModel)
                      .WithMany()
                      .HasForeignKey(e => e.payrollID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // deductionType
            modelBuilder.Entity<deductionType>(entity =>
            {
                entity.Property(e => e.deductionTypeID).HasColumnName("deductionTypeID");
                entity.Property(e => e.deductionName).HasColumnName("deductionName");
                entity.Property(e => e.isRecurring).HasColumnName("isRecurring");
                entity.Property(e => e.Status).HasColumnName("Status");
            });



            /// <summary>
            /// Views
            ///</summary>
            // Leave Report View
            modelBuilder.Entity<LeaveReportView>()
            .HasNoKey()
            .ToView("vw_LeaveReport");

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

        }

        public DbSet<accessModel> Accesses { get; set; }
        public DbSet<accessHistoryModel> AccessHistories { get; set; }
        public DbSet<accountModel> Accounts { get; set; }
        public DbSet<addressModel> Addresses { get; set; }
        public DbSet<allowanceAssignmentModel> AllowanceAssignments { get; set; }
        public DbSet<allowanceAssignmentHistoryModel> AllowanceAssignmentsHistories { get; set; }
        public DbSet<allowanceModel> Allowances { get; set; }
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
        public DbSet<employmentRequestModel> EmploymentRequests { get; set; }
        public DbSet<employmentRequestHistoryModel> EmploymentRequestHistories { get; set; }
        public DbSet<employmentTypeModel> EmploymentTypes { get; set; }
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
        public DbSet<PIS2.Models.siteAssignmentModel> SiteAssignments { get; set; }
        public DbSet<shiftModel> Shifts { get; set; }
        public DbSet<shiftAssignmentModel> ShiftAssignments { get; set; }
        public DbSet<structureModel> Structures { get; set; }
        public DbSet<structureHistoryModel> StructureHistories { get; set; }
        public DbSet<subAccountModel> SubAccounts { get; set; }
        public DbSet<terminationModel> Terminations { get; set; }
        public DbSet<PIS2.Models.breakModel> Breaks { get; set; } = default!;
        public DbSet<PIS2.Models.userModel> Users { get; set; } = default!;
        public DbSet<PIS2.Models.userHistoryModel> UserHistories { get; set; } = default!;
        public DbSet<workSiteModel> WorkSites { get; set; }       
        public DbSet<workSiteHistoryModel> WorkSitesHistories { get; set; }
        public DbSet<PIS2.Models.loyaltyModel> Loyalties { get; set; } = default!;
        public DbSet<PIS2.Models.loyaltyHistoryModel> LoyaltyHistories { get; set; } = default!;

        ///<summary>
        /// Payroll related
        ///</summary>
        public DbSet<payrollModel> Payrolls { get; set; }
        public DbSet<payrollHistory> PayrollHistories { get; set; }
        public DbSet<payrollPay> PayrollPays { get; set; }
        public DbSet<earningModel> Earnings { get; set; }
        public DbSet<earningType> EarningTypes { get; set; }
        public DbSet<deductionModel> Deductions { get; set; }
        public DbSet<deductionType> DeductionTypes { get; set; }




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
    }
    
}
