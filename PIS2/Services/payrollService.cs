using Azure.Identity;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.Finance;
using PIS2.Models.HR;
using PIS2.Pages.EmployeeService;
using PIS2.Views;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Services {

    public class PayrollService
    {
        private readonly PISContext _db;
        private readonly ILogger<PayrollService> _logger;
        private readonly Core _core;
        private readonly Global_S _global;

        const decimal pensionEmployee = 0.07m;
        const decimal pensionEmployer = 0.11m;
        const decimal avgWorkingHoursPerMonth = 208;
        const decimal hoursPerDay = 8m;
        const string defaultSubAccName = "3000000000000";
        const string pensionTypeCode = "PENSIN";
        const string OtTypeCode = "OT";
        const string SalaryTypeCode = "SALARY";
        const string AllowanceTypeCode = "TALLOW";
        const string TaxTypeCode = "PAYE";
        const string EMPLOYEE_CREDIT_ACCOUNT_NAME = "Account Payables-Employees";
        const decimal AVG_DAYS_PER_MONTH = 26;

        public PayrollService(PISContext db, ILogger<PayrollService> logger, Core core, Global_S global)
        {
            _db = db;
            _logger = logger;
            _core = core;
            _global = global;
        }

        public async Task<DateTime> GetLastPayroll(int emp) 
        {
            var lastPay = _db.PayrollPays.Include(p => p.payrollModel)
                .OrderByDescending(p => p.payrollModel.EndDate)
                .FirstOrDefault(p => p.employmentID == emp && p.payrollModel.payrollStatus == payrollStatus.COMPLETED);
            var employee = await _db.Employments.FirstOrDefaultAsync(e => e.employmentID == emp);
            if (employee == null) 
            { 
                _logger.LogError("Employee Not Found. {Service} {UserName}", "Get Last Payroll");
                ArgumentNullException.ThrowIfNull(employee);
            }
            return lastPay?.payrollModel?.EndDate ?? employee.employmentDate;
        }
        public async Task GeneratePayrollAsync(int payrollId, string modifiedBy)
        {
            var payroll = await _db.Payrolls
                .Include(p => p.companyModel)
                .FirstOrDefaultAsync(p => p.payrollID == payrollId);

            if (payroll == null) throw new ArgumentException("Payroll not found");
            if (payroll.payrollStatus == payrollStatus.PENDING)
                throw new InvalidOperationException("Payroll must be APPROVED to generate.");
            if (payroll.payrollStatus == payrollStatus.COMPLETED)
                throw new InvalidOperationException("Payroll is COMPLETED, you can not re-generate.");
            if (payroll.payrollStatus == payrollStatus.POSTED)
                throw new InvalidOperationException("Payroll is POSTED, you can not re-generate.");
            if (payroll.payrollStatus == payrollStatus.DISCARDED)
                throw new InvalidOperationException("Payroll is DISCARDED, you can not re-generate.");
            // fetch employees for company or all if companyID null
            var employeesQuery = _db.Employments.Include(e => e.employmentTypeModel)
                .Where(e => e.employmentStatus == mainStatus.Active)
                .AsQueryable();

            //filter by companyif exisits
            if (payroll.companyID.HasValue)
            {
                employeesQuery = employeesQuery
                        .Where(e => e.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.companyID == payroll.companyID.Value);
            }

            var defaultDebit = _db.SubAccounts.FirstOrDefault(s => s.subAccountName.ToLower()== defaultSubAccName.ToLower()) ?? new subAccountModel();
            if (defaultDebit.accountID == 0) throw new InvalidOperationException("Default Account Not Found!.");
            var empls = await employeesQuery.ToListAsync();
            var employees = empls;
            var pensionDedType = _db.DeductionTypes.AsNoTracking().FirstOrDefault(d => d.deductionCode.ToLower() == pensionTypeCode.ToLower()).deductionTypeID;
            if (pensionDedType == 0) throw new InvalidOperationException("Pension Deduction Type Not Found!.");
            var prevPays = _db.PayrollPays.Include(p=> p.payrollModel).Where(p => p.payrollID != payrollId).GroupBy(p => p.employmentID).ToDictionary(g => g.Key, g => g.Max(p => p.payrollModel?.EndDate));
            var jobPs = _db.JobPlacements
                .Include(j => j.departmentModel).ThenInclude(d => d.subAccountModel).Where(j => j.jobPlacementStatus == mainStatus.Active).ToList();

            var allAbsences = await _db.LeaveHistoryView.AsNoTracking()
                .Where(l => l.leaveStatus == leaveStatus.Posted && l.leaveGroup == leaveGroup.Absenteeism && l.leaveHistoryAction == leaveStatus.Posted
                && l.modifiedDate > payroll.StartDate && l.modifiedDate <= payroll.EndDate)
                .ToListAsync();

            var allOvertimeRecords = await _db.OvertimeHistoryView.AsNoTracking()
                .Where(o => o.overtimeRecordStatus == overtimeStatus.Posted && o.overtimeHistoryAction == overtimeStatus.Posted
                && o.modifiedDate > payroll.StartDate && o.modifiedDate <= payroll.EndDate)
                .ToListAsync();

            var OtEarningType = _db.EarningTypes.AsNoTracking().FirstOrDefault(d => d.earningTypeCode.ToLower() == OtTypeCode.ToLower()).earningTypeID;
            if (OtEarningType == 0) throw new InvalidOperationException("Overtime Earning Type Not Found!.");
            var allEmpEarnings = await _db.Earnings.AsNoTracking().Include(e => e.earningType).Where(e => e.earningStatus == mainStatus.Active && e.earningType.isPayroll == true).ToListAsync();
            var allAllowances = await _db.AllowanceAssignments.AsNoTracking()
                .Include(a => a.allowanceModel)
                .Where(a => a.allowanceStatus == mainStatus.Active)
                .ToListAsync();
            var AllowanceEarningType = _db.EarningTypes.AsNoTracking().FirstOrDefault(d => d.earningTypeCode.ToLower()==AllowanceTypeCode).earningTypeID;
            if (AllowanceEarningType == 0) throw new InvalidOperationException("Total Allowance Earning Type Not Found!.");
            var SalaryEarningType = _db.EarningTypes.AsNoTracking().FirstOrDefault(d => d.earningTypeCode.ToLower() == SalaryTypeCode.ToLower()).earningTypeID;
            if (SalaryEarningType == 0) throw new InvalidOperationException("Salary Earning Type Not Found!.");
            var TaxDeductionType = _db.DeductionTypes.FirstOrDefault(d => d.deductionCode.ToLower()== TaxTypeCode.ToLower()).deductionTypeID;
            if (TaxDeductionType == 0) throw new InvalidOperationException("Tax Deduction Type Not Found!.");
            var taxRates = _db.TaxRates.AsNoTracking().Where(t => t.taxStatus == mainStatus.Active).OrderBy(t => t.amount).ToList();
            if (taxRates == null) throw new InvalidOperationException("Tax Rates Not Found!.");
            var AllOtherDeds = await _db.Deductions.AsNoTracking().Include(d => d.DeductionType).Where(d => d.deductionStatus == mainStatus.Active).ToListAsync();
            var AllCredit =await _db.Persons.AsNoTracking().ToListAsync();
            // clear any existing payrollPay for this payroll (idempotent)
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var existing = _db.PayrollPays.Where(pp => pp.payrollID == payrollId);
                _db.PayrollPays.RemoveRange(existing);
                await _db.SaveChangesAsync();

                decimal totalGross = 0, totalNet = 0, totalTax = 0, totalPenEmp = 0, totalPenEmpr = 0;
                int totalEmployees = 0;
                var DeductionRecords = new List<deductionRecordModel>();
                var EarningRecords = new List<earningRecordModel>();

                var absencesByEmp = allAbsences
                    .GroupBy(a => a.employmentID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var overtimeByEmp = allOvertimeRecords
                    .GroupBy(o => o.employmentID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var earningsByEmp = allEmpEarnings
                    .GroupBy(e => e.employmentID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var allowancesByEmp = allAllowances
                    .GroupBy(a => a.employmentID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var deductionsByEmp = AllOtherDeds
                    .GroupBy(d => d.employmentID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var jobPlacements = jobPs.ToDictionary(j => j.employmentID);
                var empAccounts = AllCredit.ToDictionary(g => g.personID, g => g.subAccountID);

                foreach (var emp in employees)
                {
                    absencesByEmp.TryGetValue(emp.employmentID, out var absences);
                    overtimeByEmp.TryGetValue(emp.employmentID, out var overtimeRecords);
                    earningsByEmp.TryGetValue(emp.employmentID, out var empEarnings);
                    allowancesByEmp.TryGetValue(emp.employmentID, out var allowances);
                    deductionsByEmp.TryGetValue(emp.employmentID, out var otherDeds);
                    jobPlacements.TryGetValue(emp.employmentID, out var jobp);
                    prevPays.TryGetValue(emp.employmentID,out var empLastPay);
                    empAccounts.TryGetValue(emp.personID, out var empAccount);

                    absences ??= new List<LeaveHistoryView>();
                    overtimeRecords ??= new List<OvertimeHistoryView>();
                    empEarnings ??= new List<earningModel>();
                    allowances ??= new List<allowanceAssignmentModel>();
                    otherDeds ??= new List<deductionModel>();

                    jobp ??= new jobPlacementModel();
                    var credit = empAccount ?? defaultDebit?.subAccountID;
                    var debit =0;
                    debit = jobp.departmentModel?.subAccountID > 0 ? jobp.departmentModel.subAccountID ?? 0: defaultDebit?.subAccountID ?? 0;

                    if (debit == 0) throw new InvalidOperationException("Debit Sub Account Not Found!.");
                    var pay = await CalculateEmployeePayAsync(
                        emp, payroll, pensionDedType, empLastPay, jobp, absences.Sum(a => a.leaveDays), overtimeRecords.Sum(o => o.overtimeAmount), OtEarningType,
                        empEarnings, allowances, AllowanceEarningType, SalaryEarningType, TaxDeductionType,
                        taxRates, otherDeds, credit ?? 0, debit, false
                        );
                    pay.departmentID = jobp.departmentID;
                    pay.jobPlacementID = jobp.jobPlacementID;
                    pay.payrollID = payrollId;
                    pay.modifiedBy = modifiedBy;

                    _db.PayrollPays.Add(pay);

                    totalGross += pay.GrossPay;
                    totalNet += pay.NetPay;
                    totalTax += pay.DeductionRecords.Where(d => d.DeductionType != null && d.DeductionType.deductionName.ToLower().Contains("tax")).Sum(d => d.deductionAmount ?? 0);
                
                    var pension = pay.DeductionRecords.Where(d => d.deductionTypeID == pensionDedType).Sum(d => d.deductionAmount ?? 0);

                    totalPenEmp += pay.DeductionRecords.Where(d => d.deductionTypeID == pensionDedType).Sum(d => d.deductionAmount ?? 0); ;
                    // employer pension is not part of employee deductions; compute separately:
                    var pensione = pension * Global_C.PENSION_EMPLOYER / Global_C.PENSION_EMPLOYEE;
                    totalPenEmpr += pensione;

                    totalEmployees++;
                }

                payroll.totalGross = totalGross;
                payroll.totalNet = totalNet;
                payroll.totalTax = totalTax;
                payroll.totalPensionEmployee = totalPenEmp;
                payroll.totalPensionEmployer = totalPenEmpr;
                payroll.totalEmployees = totalEmployees;
                payroll.modifiedBy = modifiedBy;
                payroll.payrollStatus = payrollStatus.PROCESSED;

            
                // your payroll generation logic here
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payroll pay generation failed");
                throw;
            }
            
        }
        //
  
        public async Task<payrollPay> CalculateEmployeePayAsync(
            employmentModel emp, payrollModel payroll, int? pensionDedType, DateTime? empLastPay,
            jobPlacementModel jobp, decimal absences, decimal overtimeAmount, int OtEarningType,
            List<earningModel> empEarnings, List<allowanceAssignmentModel> allowances, int AllowanceEarningType,
            int SalaryEarningType, int TaxDeductionType, List<taxRateModel> taxRates, List<deductionModel> otherDeds,
            int credit, int debit, bool IsStrict)
        {
            // LOAD RELEVANT DATA
            var start = empLastPay?.AddDays(1) ?? payroll.StartDate;
            var end = payroll.EndDate.Date;
            var empType = emp.employmentTypeModel;
            if (emp.employmentDate > payroll.StartDate)
            {
                start = emp.employmentDate;
            }
            if (emp.employmentStatus == mainStatus.Inactive)
            {
                end = emp.employmentTerminationDate ?? end;
                start =new DateTime(end.Year, end.Month, 1);
            }
            //-----------------------------------------------------------
            // CALCULATE SALARY EARNING
            //-----------------------------------------------------------
            //GET LAST JOB PLACEMENT
  
            decimal baseSalary = jobp?.jobPlacementSalary ?? 0m; // assumes monthly
            
            decimal hourlyRate = baseSalary / avgWorkingHoursPerMonth;
            //
            //-----------------------------------------------------------
            // find total working days in payroll period (business days) 
            //-----------------------------------------------------------
            int workingDaysInPeriod = GetWorkingDaysInPeriod(start, end);
            var daysInRange = (end.Date - start.Date).Days + 1;
            var isMonthly = daysInRange >= 28 && daysInRange <= 31? true: false;
            decimal totalPossibleHours = workingDaysInPeriod > 0? workingDaysInPeriod * hoursPerDay :0;

            //
            //-----------------------------------------------------------
            // GET ABSENT DAYS
            //-----------------------------------------------------------
            
            decimal absentHours = absences * hoursPerDay; // ensure AbsentHours exists
            //
            //-----------------------------------------------------------
            // GET WORKED HOURS NB. REPLACE avgWorkingHoursPerMonth BY totalPossibleHours FOR STRICT  WORKING HOURS CALCULATION
            //-----------------------------------------------------------
            decimal workedHours = IsStrict ? Math.Max(0, totalPossibleHours - absentHours) : isMonthly? Math.Max(0, avgWorkingHoursPerMonth - absentHours): Math.Max(0, (daysInRange*hoursPerDay) - absentHours);

            //
            //
            var earnings = new List<earningRecordModel>();
            
            //-----------------------------------------------------------
            // OVERTIME WITHIN PERIOD
            //-----------------------------------------------------------
         
            if (overtimeAmount > 0)
            {                
                earnings.Add(new earningRecordModel { earningAmount = overtimeAmount, earningTypeID = OtEarningType, earningReference = "overtime", modifiedBy = "system" });
            }
            //-----------------------------------------------------------
            // ALLOWANCES (earnings)
            //------------------------------------------------------------
            var taxableAllowances = allowances.Where(a => a.allowanceModel?.allowanceTaxable == true).ToList();
            decimal allowancesSum = allowances.Sum(a => a.allowanceAssignmentAmount);
            if (IsStrict) 
            {
                var starDate = GetClosestFirstDate(start);
                var dateDif = (starDate - end).Days;
                allowancesSum = (allowancesSum / AVG_DAYS_PER_MONTH) * dateDif;
            }
            if (allowancesSum > 0)
            {
                earnings.Add(new earningRecordModel { earningAmount = allowancesSum, earningTypeID = AllowanceEarningType, earningReference = "allowance", modifiedBy = "system" });
            }
            // BASE SALARY: if salaried, pro-rate monthly salary by workedHours/totalPossibleHours
            decimal basePay = 0m;
            if (empType.isSalaryAllowed)
            {
                //NB. REPLACE avgWorkingHoursPerMonth BY totalPossibleHours FOR STRICT  WORKING HOURS CALCULATION
                basePay = workedHours <= 0? 0: baseSalary * (workedHours / avgWorkingHoursPerMonth);
            }
            if (basePay > 0)
            {
                earnings.Add(new earningRecordModel { earningAmount = basePay, earningTypeID = SalaryEarningType, earningReference = "salary", modifiedBy = "system" });
            }

            // GET OTHER EARNINGS (Bonus)

            // ADD OTHER EARNINGS
            foreach (var oe in empEarnings)
            {
                // copy to new earning record attached to payroll computation
                earnings.Add(new earningRecordModel
                {
                    earningTypeID = oe.earningTypeID,
                    earningAmount = CalculateEarningValue(oe, baseSalary, allowancesSum),
                    earningReference = oe.earningType?.earningTypeName ?? oe.earningID.ToString(),
                    modifiedBy = "system"
                });
            }

            //GROSS EARNING
            decimal gross = earnings.Sum(e => e.earningAmount);

            //
            //*******************************
            //DEDUCTIONS
            //*******************************
            //PREPARE LIST FOR HOLDING DEDUCTIONS
            var deductions = new List<deductionRecordModel>();

            // ADD PENSION TO DEDUCTIONS
            decimal pensionAmount = 0; 
            if (empType.isPensionAllowed && pensionDedType != null)
            {
                pensionAmount = Math.Round(baseSalary * pensionEmployee, 2);
                deductions.Add(new deductionRecordModel
                {
                    deductionTypeID = pensionDedType ?? 0,
                    deductionAmount = pensionAmount,
                    deductionReference = "pension",
                    modifiedBy = "system"
                });
            }
            //
            //TAX
            //TAXABLE EARNINGS WITHOUT SALARY, OVERTIME,AND ALLOWANCE
            decimal taxableEarnings = empEarnings.Where(e => e.earningType?.isTaxable == true)
                .Sum(e => CalculateEarningValue(e, baseSalary, allowancesSum));

            //ALL TAXABLE EARNING
            decimal taxable = basePay + overtimeAmount + taxableEarnings + taxableAllowances.Sum(a => a.allowanceAssignmentAmount);

            //TAX RATE LOOKUP
            var taxRate = taxRates.OrderByDescending(t => t.amount)
                .Where(t => t.amount <= taxable).FirstOrDefault() ?? new taxRateModel { taxRate = 0, deduction = 0 };

            //GET TAX AMOUNT
            decimal tax = 0m;
            if (taxRate != null)
            {
                tax = Math.Max(0, Math.Round(taxable * taxRate.taxRate/100 - taxRate.deduction, 2));
                if (tax < 0) tax = 0;
            }
            
            //
            //ADD TAX DEDUCTION TO DEDUCTIONS
            deductions.Add(new deductionRecordModel
            {
                deductionTypeID = TaxDeductionType,
                deductionAmount = tax,
                deductionReference = "Income-tax",
                modifiedBy = "system"
            });
            //
            //GET NET PAY AFTER TAX AND PENSION
            decimal netPay = gross - tax - pensionAmount;
            //
            //
            // OTHER DEDUCTIONS (loans, penalties)
            

            // ADD OTHER DEDUCTIONS TO DEDUCTION HOLDER (respect priority)
            foreach (var od in otherDeds.OrderBy(d => d.DeductionType?.deductionPriority))
            {
                // copy to new deduction record attached to payroll computation
                deductions.Add(new deductionRecordModel
                {
                    deductionTypeID = od.deductionTypeID,
                    deductionAmount = CalculateDeductionValue(od, baseSalary, allowancesSum, gross, netPay),
                    deductionReference = od.DeductionType?.deductionName ?? "other",
                    modifiedBy = "system"
                });
            }

            //TOTAL DEDUCTIONS AMOUNT
            decimal totalDeductions = deductions.Sum(d => d.deductionAmount ?? 0);

            //TOTAL NET PAYABLE
            decimal net = gross - totalDeductions;
            if (debit == 0)
            {
                throw new InvalidOperationException($"Debit Sub AccountID ={debit}!.");
            }
            //CREATE A PAYROLL ROW FOR THE EMPLOYEE
            var payrollPay = new payrollPay
            {
                employmentID = emp.employmentID,
                workedHours = workedHours,
                hourlyRate = hourlyRate,
                EarningRecords = earnings,
                DeductionRecords = deductions,
                CreditAccountID = credit,
                DebitAccountID = debit,
                GrossPay = gross,
                NetPay = net,
                departmentID = jobp?.departmentID,
                jobPlacementID = jobp?.jobPlacementID,
                modifiedBy ="System"
            };

            return payrollPay;
        }
        //########################################
        //REPLACE WITH WORKINDAY FROM CORE
        //########################################
        public int GetWorkingDaysInPeriod(DateTime start, DateTime end)
        {
            // implement business calendar logic (exclude weekends)
            int days = (end.Date - start.Date).Days + 1;

            if(start > end) { return 0; }

            int workingDays = 0;
            for (int i = 0; i < days; i++)
            {
                var day = start.AddDays(i);
                if (day.DayOfWeek == DayOfWeek.Sunday) continue; // exclude Sundays 
                workingDays++;
            }
            return workingDays;
        }

        public async Task ProcessPayrollAsync(int payrollId, string modifiedBy)
        {
            // mark payroll as POSTED, and on COMPLETE mark overtime/leave done when finalizing
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == payrollId);
            if (payroll == null) throw new ArgumentException("Payroll not found");
            if (payroll.payrollStatus != payrollStatus.APPROVED || payroll.payrollStatus != payrollStatus.PROCESSED) throw new ArgumentException("Payroll not APPROVED");

            payroll.payrollStatus = payrollStatus.PROCESSED;
            payroll.modifiedBy = modifiedBy;

            await _db.SaveChangesAsync();
        }
        public async Task PostPayrollAsync(int payrollId, string modifiedBy)
        {
            // mark payroll as POSTED, 
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == payrollId);
            if (payroll == null) throw new ArgumentException("Payroll not found");
            if (payroll.payrollStatus != payrollStatus.PROCESSED) throw new ArgumentException("Payroll not PROCESSED");
            
            payroll.payrollStatus = payrollStatus.POSTED;
            payroll.modifiedBy = modifiedBy;

            await _db.SaveChangesAsync();
        }
        public async Task DiscardPayrollAsync(int payrollId, string modifiedBy)
        {
            // mark payroll as POSTED, and on COMPLETE mark overtime/leave done when finalizing
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == payrollId);
            if (payroll == null) throw new ArgumentException("Payroll not found");
            if (payroll.payrollStatus == payrollStatus.COMPLETED) throw new ArgumentException("Payroll is COMPLETED!");
            payroll.payrollStatus = payrollStatus.DISCARDED;
            payroll.modifiedBy = modifiedBy;

            await _db.SaveChangesAsync();
        }

        public async Task CompletePayrollAsync(int payrollId, string modifiedBy)
        {
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == payrollId);
            if (payroll == null) { throw new ArgumentException("Payroll not found"); }
            if (payroll.payrollStatus != payrollStatus.POSTED) throw new ArgumentException("Payroll not posted");
            if (_db.JournalEntries.AsNoTracking().Any(j => j.Reference == $"Payroll-{payrollId}"))
                throw new InvalidOperationException("Payroll committed");

            if (payroll == null) throw new ArgumentException("Payroll not found");
            if (payroll.payrollStatus != payrollStatus.POSTED) throw new ArgumentException("Payroll not not POSTED");
            // mark payroll completed
            payroll.payrollStatus = payrollStatus.COMPLETED;
            payroll.modifiedBy = modifiedBy;

            // mark overtime & leaves in the period as completed (so they won't be used again)
            var pays = await _db.PayrollPays.AsNoTracking()
                .Include(p => p.DeductionRecords).ThenInclude(d => d.DeductionType)
                .Include(p => p.EarningRecords).ThenInclude(d => d.earningType)
                .Where(pp => pp.payrollID == payrollId).ToListAsync();

            var empIds = pays.Select(p => p.employmentID).Distinct();

            var absRange = _db.LeaveHistories.AsNoTracking()
                .Where(l => l.leaveHistoryAction == leaveStatus.Posted && l.modifiedDate >= payroll.StartDate && l.modifiedDate <= payroll.EndDate)
                .OrderByDescending(l => l.modifiedDate).GroupBy(l => l.leaveID)
                .Select(l => l.First().leaveID).ToList();

            var allAbsences = await _db.Leaves.AsNoTracking()
                .Where(l => absRange.Contains(l.leaveID) && l.leaveStatus == leaveStatus.Posted && l.leaveTypeModel.leaveGroup == leaveGroup.Absenteeism && empIds.Contains(l.employmentID))
                .ToListAsync();

            var otRange = _db.OvertimeHistories.AsNoTracking()
                .Where(l => l.overtimeHistoryAction == overtimeStatus.Posted && l.modifiedDate >= payroll.StartDate && l.modifiedDate <= payroll.EndDate)
                .OrderByDescending(l => l.modifiedDate).GroupBy(l => l.overtimeRecordID)
                .Select(l => l.First().overtimeRecordID).ToList();

            var allOvertimeRecords = await _db.OvertimeRecords.AsNoTracking()
                .Where(o => otRange.Contains(o.overtimeRecordID) && o.overtimeRecordStatus == overtimeStatus.Posted && empIds.Contains(o.employmentID))
                .ToListAsync();

            var overtimeRecords = allOvertimeRecords;
                overtimeRecords.ForEach(o => o.overtimeRecordStatus = overtimeStatus.Completed);

            var leaves = allAbsences;
                leaves.ForEach(l => l.leaveStatus = leaveStatus.Completed);
            using var tx = await _db.Database.BeginTransactionAsync();

           
            try
            {
                _db.OvertimeRecords.UpdateRange(overtimeRecords);
                
                _db.Leaves.UpdateRange(leaves);

                var earnings = await _db.Earnings.Where(e => e.earningStatus == mainStatus.Active && empIds.Contains(e.employmentID)).ToListAsync();
                var subAccounts = pays.Select(p => p.DebitAccountID).ToList();
                var debitAccounts = _db.SubAccounts.AsNoTracking().Include(s => s.accountModel).Where(p => subAccounts.Contains(p.subAccountID))
                    .ToDictionary(s=>s.subAccountID, s => s.accountModel.accountID);
                var empCreditAccount = _db.Accounts.AsNoTracking().FirstOrDefault(a => a.accountName == EMPLOYEE_CREDIT_ACCOUNT_NAME);
                if (empCreditAccount == null) 
                {
                    _logger.LogError("Employee credit account not found!"); 
                    throw new Exception($"Employee cash account is not set. Create a liability type account named {EMPLOYEE_CREDIT_ACCOUNT_NAME} to complete the payment.");
                    
                }
                foreach (var e in earnings)
                {
                    // Decrement remainingIteration
                    e.remainingIteration -= 1;

                    // If remainingIteration reaches 0, mark as inactive
                    if (e.remainingIteration <= 0)
                    {
                        e.earningStatus = mainStatus.Inactive;
                        e.remainingIteration = 0; // optional: prevent negative
                    }
                    
                }

                var deductions = await _db.Deductions.Where(e => e.deductionStatus == mainStatus.Active && empIds.Contains(e.employmentID)).ToListAsync();
                var defaultSubAccount = await _db.SubAccounts.AsNoTracking().FirstOrDefaultAsync(s => s.subAccountName.ToLower() == defaultSubAccName.ToLower());
                var defaultAccount = await _db.Accounts.AsNoTracking().FirstOrDefaultAsync();
                foreach (var e in deductions)
                {
                    // Decrement remainingIteration
                    e.remainingIteration -= 1;

                    // If remainingIteration reaches 0, mark as inactive
                    if (e.remainingIteration <= 0)
                    {
                        e.deductionStatus = mainStatus.Inactive;
                        e.remainingIteration = 0; // optional: prevent negative
                    }
                }

 
                var journal = new JournalEntry
                {
                    EntryDate = payroll.EndDate,
                    Reference = $"Payroll-{payroll.payrollID}",
                    Description = "Payroll posting",
                    modifiedBy = modifiedBy,
                    modifiedDate = DateTime.Now,
                    Lines = new List<JournalEntryLine>()
                };

                foreach (var p in pays)
                {
                    debitAccounts.TryGetValue(p.DebitAccountID, out var debitAccount);
                    // Debit Salary Expense
                    foreach (var e in p.EarningRecords)
                    {
                        journal.Lines.Add(new JournalEntryLine
                        {
                            accountID = e.earningType?.accountID ?? debitAccount,
                            subAccountID = p.DebitAccountID,
                            Debit = e.earningAmount,
                            Credit = 0
                        });
                    }
                   
                    // Credit Net Pay (Cash/Payable)
                    journal.Lines.Add(new JournalEntryLine
                    {
                        accountID = empCreditAccount?.accountID ?? debitAccount,
                        subAccountID = p.CreditAccountID,
                        Debit = 0,
                        Credit = p.NetPay
                    });
                    //if(!p.DeductionRecords.Any()) throw new Exception("No deduction records!.");
                    foreach (var d in p.DeductionRecords ?? new List<deductionRecordModel>())
                    {
                        if (d.DeductionType.accountID == null || d.DeductionType.accountID == 0) throw new Exception("No account ID for deduction record.");
                        journal.Lines.Add(new JournalEntryLine
                        {
                            accountID = d.DeductionType?.accountID ?? debitAccount,
                            subAccountID = p.CreditAccountID,
                            Debit = 0,
                            Credit = d.deductionAmount ?? 0
                        });
                    }

                }

                var totalDebit = journal.Lines.Sum(l => l.Debit);
                var totalCredit = journal.Lines.Sum(l => l.Credit);

                //if (totalDebit != totalCredit)
                //    throw new Exception("Journal not balanced");

                _db.JournalEntries.Add(journal);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to complete payroll");
                throw;
            }
        }

        private decimal CalculateDeductionValue(deductionModel d, decimal baseSalary, decimal allowancesSum, decimal gross, decimal netPay)
        {
            decimal baseValue = d.deductionBase switch
            {
                deductionBase.NONE => 100,         // Fixed percentage base
                deductionBase.SALARY => baseSalary,
                deductionBase.ALLOWANCE => allowancesSum,
                deductionBase.GROSS => gross,
                deductionBase.NET => netPay,
                _ => 0
            };

            if (d.IsPercentage)
                return d.deductionAmount / 100m * baseValue;

            return d.deductionAmount; // Fixed amount
        }

        private decimal CalculateEarningValue(earningModel e, decimal baseSalary, decimal allowancesSum)
        {
            decimal baseValue = e.earningBase switch
            {
                earningBase.NONE => 100,         // Fixed percentage base
                earningBase.SALARY => baseSalary,
                earningBase.ALLOWANCE => allowancesSum,
                _ => 0
            };

            if (e.IsPercentage)
                return e.earningAmount / 100m * baseValue;

            return e.earningAmount; // Fixed amount
        }

        public async Task<payrollModel> GetExitPay(employmentModel employmentModel)
        {
            var LastPay = await GetLastPayroll(employmentModel.employmentID);

            var SeverancePay = await _core.GetSeverance(employmentModel.employmentID);
            var leaveSum = await _db.AnnualLeaveSummary.AsNoTracking().FirstOrDefaultAsync(a => a.employmentID == employmentModel.employmentID);
            var LeaveSummary = leaveSum?.adjustedLeaveBalanceCost ?? 0;
            var Termination = await _db.Terminations.FirstOrDefaultAsync(t => t.employmentID == employmentModel.employmentID);

            if (LeaveSummary == 0)
            {
                var PaidLeave = _db.Leaves.AsNoTracking().OrderByDescending(l => l.leaveRequestDate).FirstOrDefault(l => l.employmentID == employmentModel.employmentID && l.leaveTypeID == _global.PaidLeaveID);
                LeaveSummary = PaidLeave?.leaveCost ??0;
            }

            var absences = await _db.LeaveHistoryView.AsNoTracking()
                .Where(l => l.employmentID == employmentModel.employmentID && l.leaveGroup == leaveGroup.Absenteeism
                && l.leaveStatus == leaveStatus.Posted && l.leaveHistoryAction == leaveStatus.Posted && l.modifiedDate > LastPay)
                .SumAsync(l => l.leaveDays);

            var OTs = await _db.OvertimeHistoryView.AsNoTracking()
                .Where(o => o.employmentID == employmentModel.employmentID && o.overtimeRecordStatus == overtimeStatus.Posted
                && o.overtimeHistoryAction == overtimeStatus.Posted && o.modifiedDate > LastPay)
                .ToListAsync();
            var OT = OTs.Sum(o => o.overtimeAmount);

            var earning = await _db.Earnings.AsNoTracking()
                .Include(e => e.earningType)
                .Where(e => e.employmentID == employmentModel.employmentID && e.earningStatus == mainStatus.Active)
                .ToListAsync();

            earning.Add(new earningModel
            {
                employmentID = employmentModel.employmentID,
                earningTypeID = _global.SeveranceType,
                earningType = _db.EarningTypes.FirstOrDefault(d => d.earningTypeCode == Global_C.SEVERANCE_PAY_CODE),
                earningReference = "Severance",
                earningAmount = SeverancePay,
                earningStatus = mainStatus.Active,
                earningIteration = 1,
                remainingIteration = 1,
                earningBase = earningBase.NONE,
                modifiedBy = "System"
            });

            earning.Add(new earningModel
            {
                employmentID = employmentModel.employmentID,
                earningTypeID = _global.LeavePayType,
                earningType = _db.EarningTypes.FirstOrDefault(d => d.earningTypeCode == Global_C.LEAVE_PAY_CODE),
                earningReference = "Leavepayment",
                earningAmount = LeaveSummary,
                earningStatus = mainStatus.Active,
                earningIteration = 1,
                remainingIteration = 1,
                earningBase = earningBase.NONE,
                modifiedBy = "System"
            });

            var allowances = await _db.AllowanceAssignments.AsNoTracking()
                .Where(a => a.employmentID == employmentModel.employmentID && a.allowanceStatus == mainStatus.Active)
                .ToListAsync();
            var OtherDeds = await _db.Deductions.AsNoTracking().Include(d => d.DeductionType)
                .Where(d => d.employmentID == employmentModel.employmentID && d.deductionStatus == mainStatus.Active)
                .ToListAsync();

            var jobPlacement = await _db.JobPlacements.AsNoTracking()
                .Include(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Include(jp => jp.jobModel)
                .OrderByDescending(j => j.jobPlacementStatus == mainStatus.Active).ThenByDescending(j => j.jobPlacementDate)
                .FirstOrDefaultAsync(j => j.employmentID == employmentModel.employmentID);

            var payroll = new payrollModel
            {
                payrollMonth = DateTime.Now.Month.ToString(),
                payrollName = $"ExitPay for {employmentModel?.personModel?.personFullName} / {employmentModel?.givenID}",
                payrollStatus = payrollStatus.PROCESSED,
                IsPayroll = true,
                StartDate = LastPay.AddDays(1),
                EndDate = Termination?.terminationDate ?? employmentModel?.employmentTerminationDate ?? DateTime.Now,
                modifiedBy = "System",
                modifiedDate = DateTime.Now,
                PayrollPays = new List<payrollPay>()
            };

            var Pays = await CalculateEmployeePayAsync(employmentModel,
               payroll,null, LastPay, jobPlacement ?? new jobPlacementModel(), absences, OT, _global.OtEarningType,
               earning ?? new List<earningModel>(), allowances ?? new List<allowanceAssignmentModel>(), _global.AllowanceEarningType, _global.SalaryEarningType, _global.TaxDeductionType,
               _global.TaxRates, OtherDeds ?? new List<deductionModel>(), employmentModel?.personModel?.subAccountID ?? _global.DefaultSubAccount, jobPlacement?.departmentModel?.subAccountID ?? _global.DefaultSubAccount, true);

            payroll.PayrollPays.Add(Pays);
            payroll.totalGross = Pays.EarningRecords.Sum(e => e.earningAmount);
            payroll.totalNet = payroll.totalGross - Pays.DeductionRecords.Sum(e => e.deductionAmount);
            return payroll;
        }

        public async Task<List<earningModel>> PostExitEarnings(employmentModel employmentModel)
        {
            var LastPay = await GetLastPayroll(employmentModel.employmentID);

            var SeverancePay = await _core.GetSeverance(employmentModel.employmentID);
            var leaveSum = await _db.AnnualLeaveSummary.AsNoTracking().FirstOrDefaultAsync(a => a.employmentID == employmentModel.employmentID);
            var LeaveSummary = leaveSum?.adjustedLeaveBalanceCost ?? 0;
            var Termination = await _db.Terminations.FirstOrDefaultAsync(t => t.employmentID == employmentModel.employmentID);

            if (LeaveSummary == 0)
            {
                var PaidLeave = _db.Leaves.AsNoTracking().OrderByDescending(l => l.leaveRequestDate).FirstOrDefault(l => l.employmentID == employmentModel.employmentID && l.leaveTypeID == _global.PaidLeaveID);
                LeaveSummary = PaidLeave?.leaveCost ?? 0;
            }

            var earning = new List<earningModel>();

            var jobPlacement = await _db.JobPlacements.AsNoTracking()
                .OrderByDescending(j => j.jobPlacementStatus == mainStatus.Active).ThenByDescending(j => j.jobPlacementDate)
                .FirstOrDefaultAsync(j => j.employmentID == employmentModel.employmentID);
  
            earning.Add(new earningModel
            {
                employmentID = employmentModel.employmentID,
                earningTypeID = _global.SeveranceType,
                earningReference = "Severance",
                earningAmount = SeverancePay,
                modifiedBy = ""
            });

            earning.Add(new earningModel
            {
                employmentID = employmentModel.employmentID,
                earningTypeID = _global.LeavePayType,
                earningReference = "Leavepayment",
                earningAmount = LeaveSummary,
                modifiedBy = "System"
            });

            return earning;
        }
        DateTime GetClosestFirstDate(DateTime input)
        {
            if (input.Day < 15)
            {
                return new DateTime(input.Year, input.Month, 1);
            }

            var nextMonth = input.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 1);
        }

        public payrollModel ProccessEarning(earningModel earning)
        {
            var payroll = new payrollModel();

            var pay = new payrollPay();
            
            pay.EarningRecords.Add(new earningRecordModel 
            {
                earningAmount=earning.earningAmount,
                earningTypeID = earning.earningTypeID,
                earningReference =earning.earningType.earningTypeName,
                earningRecordID = earning.earningID,
                modifiedBy = "system",
            });

            if (earning.earningType.isTaxable)
            {
                var taxable = earning.earningAmount;
                var taxRate = _global.TaxRates.FirstOrDefault(t => t.amount > taxable);
                var tax = (taxable * taxRate.taxRate / 100) - taxRate.amount;
                pay.DeductionRecords.Add(new deductionRecordModel
                {
                    deductionAmount = taxable - tax,
                    DeductionType = _db.DeductionTypes.FirstOrDefault(d => d.deductionCode == Global_C.TAX_PAY_CODE)
                });
            }
            payroll.PayrollPays.Add(pay);
            return payroll;
        }

        public decimal GetTaxAmount(decimal taxable)
        {
            var taxRate = _global.TaxRates.OrderBy(t => t.amount).FirstOrDefault(t => t.amount > taxable);
            var tax = (taxable * taxRate.taxRate / 100) - taxRate.deduction;
            return tax;
        }
    }
}
