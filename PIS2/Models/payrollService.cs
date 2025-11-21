using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;
using System;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Models {



    public class PayrollService
    {
        private readonly PISContext _db;
        private readonly ILogger<PayrollService> _logger;

        public PayrollService(PISContext db, ILogger<PayrollService> logger)
        {
            _db = db;
            _logger = logger;
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
            // fetch employees for company or all if companyID null
            var employeesQuery = _db.Employments
                .Where(e => e.employmentStatus == mainStatus.Active
                || (e.employmentStatus == mainStatus.Inactive && e.employmentTerminationDate > payroll.StartDate && e.employmentTerminationDate <= payroll.StartDate)).AsQueryable();
            if (payroll.companyID.HasValue) employeesQuery = employeesQuery
                        .Where(e => e.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.companyID == payroll.companyID.Value);

            var employees = await employeesQuery.ToListAsync();

            // clear any existing payrollPay for this payroll (idempotent)
            var existing = _db.PayrollPays.Where(pp => pp.payrollID == payrollId);
            _db.PayrollPays.RemoveRange(existing);
            await _db.SaveChangesAsync();

            decimal totalGross = 0, totalNet = 0, totalTax = 0, totalPenEmp = 0, totalPenEmpr = 0;
            int totalEmployees = 0;
            var DeductionRecords = new List<deductionRecordModel>();
            var EarningRecords = new List<earningRecordModel>();


            foreach (var emp in employees)
            {
                var pay = await CalculateEmployeePayAsync(emp, payroll);
                pay.payrollID = payrollId;
                pay.modifiedBy = modifiedBy;

                _db.PayrollPays.Add(pay);

                ////
                //DeductionRecords.AddRange(pay.DeductionRecords);
                //EarningRecords.AddRange(pay.EarningRecords);

                totalGross += pay.GrossPay;
                totalNet += pay.NetPay;
                totalTax += pay.DeductionRecords.Where(d => d.DeductionType != null && d.DeductionType.deductionName.ToLower().Contains("tax")).Sum(d => d.deductionAmount ?? 0);
                totalPenEmp += pay.DeductionRecords.Where(d => d.DeductionType != null && d.DeductionType.deductionName.ToLower().Contains("pension") && d.DeductionType.isMandatory).Sum(d => d.deductionAmount ?? 0);
                // employer pension is not part of employee deductions; compute separately:
                totalPenEmpr += pay.GrossPay * 0.11M;

                totalEmployees++;
            }

            payroll.totalGross = totalGross;
            payroll.totalNet = totalNet;
            payroll.totalTax = totalTax;
            payroll.totalPensionEmployee = totalPenEmp;
            payroll.totalPensionEmployer = totalPenEmpr;
            payroll.totalEmployees = totalEmployees;
            payroll.modifiedBy = modifiedBy;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // your payroll generation logic here
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payroll generation failed");
                throw;
            }
            //
            ////Rese earning and deduction record IDs so they are treated like new entities
            //var earnRec = EarningRecords.Select(er => new earningRecordModel
            //{
            //    earningAmount = er.earningAmount,
            //    earningReference = er.earningReference,
            //    earningTypeId = er.earningTypeId,
            //    payrollPayID = er.payrollPayID,
            //    modifiedBy = modifiedBy
            //}).ToList();

            //var dedRec = DeductionRecords.Select(dr => new deductionRecordModel
            //{
            //    payrollPayID = dr.payrollPayID,
            //    deductionAmount = dr.deductionAmount,
            //    deductionReference =dr.deductionReference,
            //    deductionTypeID = dr.deductionTypeID,
            //    modifiedBy = modifiedBy

            //}).ToList();

            //// Add history using SQL Trigger
            //_db.EarningRecords.AddRange(earnRec);
            //await _db.SaveChangesAsync();

            //_db.DeductionRecords.AddRange(dedRec);
            //await _db.SaveChangesAsync();
        }
        //
        //??
        //
        //??
       // ??

        public async Task<payrollPay> CalculateEmployeePayAsync(employmentModel emp, payrollModel payroll)
        {
            // load relevant records
            var start = payroll.StartDate.Date;
            var end = payroll.EndDate.Date;
            var empType = _db.EmploymentTypes.FirstOrDefault(e => e.employmentTypeID == emp.employmentTypeID);

            if (emp.employmentDate > payroll.StartDate)
            {
                start = emp.employmentDate;
            }
            if (emp.employmentStatus > mainStatus.Inactive)
            {
                end = emp.employmentTerminationDate ?? end;
            }

            // Recurring earnings (salary + allowances)
            // Example: base salary from employment or earning records
            decimal baseSalary = _db.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == emp.employmentID)?.jobPlacementSalary ?? 0m; // assumes monthly
            decimal hourlyRate = baseSalary / 208;
            Console.WriteLine("#########BaseSalary-----------is--------" + baseSalary);
            // Work hour calculations
            // find total working days in payroll period (business days) - implement according to your calendar
            int workingDaysInPeriod = await GetWorkingDaysInPeriod(start, end);
            decimal hoursPerDay = 8m;
            decimal totalPossibleHours = workingDaysInPeriod * hoursPerDay;
            Console.WriteLine("#########B----WorkingDays-----------is--------" + workingDaysInPeriod);
            // absent hours
            var absences = await _db.Leaves
                .Where(l => l.employmentID == emp.employmentID && l.leaveStartDate >= start && l.leaveEndDate <= end && l.leaveStatus == leaveStatus.Posted && l.leaveTypeModel.leaveGroup == leaveGroup.Absentism)
                .ToListAsync();
            decimal absentHours = absences.Sum(a => a.leaveDays * hoursPerDay); // ensure AbsentHours exists
            Console.WriteLine("#########-------------Absent Hours-----------is--------" + absentHours);
            // worked hours
            decimal workedHours = Math.Max(0, totalPossibleHours - absentHours);

            //
            //
            var earnings = new List<earningRecordModel>();
            var earningType = await _db.EarningTypes.FirstOrDefaultAsync(e => e.earningTypeName.ToLower() == "Unknown");
            if(earningType == null)
            {
                _db.EarningTypes.Add(new earningType
                {
                    earningTypeName = "Unknown",
                    isRecurring = false,
                    isTaxable = true,
                    earningTypeStatus = mainStatus.Active,
                    modifiedBy = "Sysytem"
                });
                _db.SaveChangesAsync();
            }
            //
            // overtime within period
            var overtimeRecords = await _db.OvertimeRecords
                .Where(o => o.employmentID == emp.employmentID && o.overtimeRecordDate >= start && o.overtimeRecordDate <= end && o.overtimeRecordStatus == overtimeStatus.Posted)
                .ToListAsync();

            decimal overtimeAmount = overtimeRecords.Sum(o => o.GetOtCost);

            if (overtimeAmount > 0)
            {
                earningType = await _db.EarningTypes.FirstOrDefaultAsync(d => d.earningTypeName.ToLower().Contains("overtime"));
                earnings.Add(new earningRecordModel { earningAmount = overtimeAmount, earningTypeId = earningType.earningTypeID, earningReference = 0, modifiedBy = "system" });
            }
            // Allowances (earnings)
            var allowances = await _db.AllowanceAssignments
                .Where(a => a.employmentID == emp.employmentID && a.allowanceStatus == mainStatus.Active)
                .ToListAsync();
            decimal allowancesSum = allowances.Sum(a => a.allowanceAssignmentAmount);
            if (allowancesSum > 0)
            {
                earningType = await _db.EarningTypes.FirstOrDefaultAsync(d => d.earningTypeName.ToLower().Contains("allowance"));
                earnings.Add(new earningRecordModel { earningAmount = allowancesSum, earningTypeId = earningType.earningTypeID, earningReference = 0, modifiedBy = "system" });
            }
            
            // compute base pay: if salaried, pro-rate monthly salary by workedHours/totalPossibleHours
            decimal basePay = 0m;
            if (empType.isSalaryAllowed)
            {
                basePay =totalPossibleHours == 0? 0: baseSalary * (workedHours / totalPossibleHours);
            }
            if (basePay > 0)
            {
                earningType = await _db.EarningTypes.FirstOrDefaultAsync(d => d.earningTypeName.ToLower().Contains("salary"));
                earnings.Add(new earningRecordModel { earningAmount = basePay, earningTypeId = earningType.earningTypeID, earningReference = 0, modifiedBy = "system" });
            }
            // other earnings (Bonus)

            var empEarnings = await _db.Earnings.Where(e => e.employmentID == emp.employmentID && e.earningStatus == mainStatus.Active).ToListAsync();

            // add other earnings
            foreach (var od in empEarnings)
            {
                // copy to new earning record attached to payroll computation
                earnings.Add(new earningRecordModel
                {
                    earningTypeId = od.earningTypeId,
                    earningAmount = od.earningAmount,
                    modifiedBy = "system"
                });
            }

            decimal gross = earnings.Sum(e => e.earningAmount);
            decimal pensionEmployee = 0;
            decimal pensionEmployer = 0;
            // Compute pension employee (deduct before tax)
            if (empType.isPensionAllowed)
            {
                pensionEmployee = Math.Round(baseSalary * 0.07m, 2);//OR FROM BASE PAY##############################
                pensionEmployer = Math.Round(baseSalary * 0.11m, 2);//######################################
            }
            pensionEmployee = Math.Round(baseSalary * 0.07m, 2);
            // Taxable income = gross - pensionEmployee
            //############################################################################################
            decimal taxable = gross - pensionEmployee;

            // tax lookup
            var taxRate = await _db.TaxRates
                .Where(t => t.from <= taxable && (t.ceiling == 0 || t.ceiling >= taxable))
                .OrderBy(t => t.from)
                .FirstOrDefaultAsync();

            decimal tax = 0m;
            if (taxRate != null)
            {
                tax = Math.Max(0, Math.Round((taxable * taxRate.taxRate/100) - taxRate.deduction, 2));
                if (tax < 0) tax = 0;
            }

            //
            //
            // Build payroll deductions list (pension + tax + any mandatory deductions)
            var deductions = new List<deductionRecordModel>();

            // pension
            var pensionDedType = await _db.DeductionTypes.FirstOrDefaultAsync(d => d.deductionName.ToLower().Contains("pension"));
            deductions.Add(new deductionRecordModel
            {
                deductionTypeID = pensionDedType?.deductionTypeID ?? 0,
                deductionAmount = pensionEmployee,
                modifiedBy = "system"
            });

            // tax
            var taxDedType = await _db.DeductionTypes.FirstOrDefaultAsync(d => d.deductionName.ToLower().Contains("tax"));
            deductions.Add(new deductionRecordModel
            {
                deductionTypeID = taxDedType?.deductionTypeID ?? 0,
                deductionAmount = tax,
                modifiedBy = "system"
            });

            // other deductions (loans, penalties)
            var otherDeds = await _db.Deductions
                .Include(d => d.DeductionType)
                .Where(d => d.employmentID == emp.employmentID && d.deductionStatus == mainStatus.Active)
                .ToListAsync();

            // add other deductions (respect priority)
            foreach (var od in otherDeds.OrderBy(d => d.DeductionType?.dedcutionPriority))
            {
                // copy to new deduction record attached to payroll computation
                deductions.Add(new deductionRecordModel
                {
                    deductionTypeID = od.deductionTypeID,
                    deductionAmount = od.deductionAmount,
                    modifiedBy = "system"
                });
            }

            decimal totalDeductions = deductions.Sum(d => d.deductionAmount ?? 0);
            decimal net = gross - totalDeductions;

            var payrollPay = new payrollPay
            {
                employmentID = emp.employmentID,
                EmploymentModel = emp,
                EarningRecords = earnings,
                DeductionRecords = deductions,
                GrossPay = gross,
                NetPay = net
            };

            return payrollPay;
        }
        //########################################
        //REPLACE WITH WORKINDAY FROM CORE
        //########################################
        private async Task<int> GetWorkingDaysInPeriod(DateTime start, DateTime end)
        {
            // implement business calendar logic (exclude weekends, public holidays from a table if you have)
            int days = (end.Date - start.Date).Days + 1;
            int workingDays = 0;
            for (int i = 0; i < days; i++)
            {
                var day = start.AddDays(i);
                if (day.DayOfWeek == DayOfWeek.Sunday) continue; // exclude Sundays per your rule
                workingDays++;
            }
            return workingDays;
        }

        public async Task PostPayrollAsync(int payrollId, string modifiedBy)
        {
            // mark payroll as POSTED, and on COMPLETE mark overtime/leave done when finalizing
            var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == payrollId);
            if (payroll == null) throw new ArgumentException("Payroll not found");
            if (payroll.payrollStatus == payrollStatus.PENDING) throw new ArgumentException("Payroll not APPROVED");
            if (payroll.payrollStatus == payrollStatus.COMPLETED) throw new ArgumentException("Payroll is COMPLETED!");

            payroll.payrollStatus = payrollStatus.POSTED;
            payroll.modifiedBy = modifiedBy;

            await _db.SaveChangesAsync();
        }

        public async Task CompletePayrollAsync(int payrollId, string modifiedBy)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var payroll = await _db.Payrolls.FirstOrDefaultAsync(p => p.payrollID == payrollId);
                if (payroll == null) throw new ArgumentException("Payroll not found");
                if (payroll.payrollStatus != payrollStatus.POSTED) throw new ArgumentException("Payroll not not posted");
                // mark payroll completed
                payroll.payrollStatus = payrollStatus.COMPLETED;
                payroll.modifiedBy = modifiedBy;

                // mark overtime & leaves in the period as completed (so they won't be used again)
                var pays = await _db.PayrollPays.Where(pp => pp.payrollID == payrollId).ToListAsync();
                var empIds = pays.Select(p => p.employmentID).Distinct();
                var overtimeRecords = await _db.OvertimeRecords.Where(o => empIds.Contains(o.employmentID) && o.overtimeRecordDate >= payroll.StartDate && o.overtimeRecordDate <= payroll.EndDate && o.overtimeRecordStatus == overtimeStatus.Posted).ToListAsync();
                overtimeRecords.ForEach(o => o.overtimeRecordStatus = overtimeStatus.Completed);
                _db.OvertimeRecords.UpdateRange(overtimeRecords);

                var leaves = await _db.Leaves.Where(l => empIds.Contains(l.employmentID) && l.leaveStartDate >= payroll.StartDate && l.leaveEndDate <= payroll.EndDate && l.leaveStatus == leaveStatus.Posted && l.leaveTypeModel.leaveGroup == leaveGroup.Absentism).ToListAsync();
                leaves.ForEach(l => l.leaveStatus = leaveStatus.Completed);
                _db.Leaves.UpdateRange(leaves);

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
    }
}
