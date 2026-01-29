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

                
                totalGross += pay.GrossPay;
                totalNet += pay.NetPay;
                totalTax += pay.DeductionRecords.Where(d => d.DeductionType != null && d.DeductionType.deductionName.ToLower().Contains("tax")).Sum(d => d.deductionAmount ?? 0);
                totalPenEmp += pay.DeductionRecords.Where(d => d.DeductionType != null && d.DeductionType.deductionName.ToLower().Contains("pension")).Sum(d => d.deductionAmount ?? 0);
                // employer pension is not part of employee deductions; compute separately:
                totalPenEmpr += _db.JobPlacements.First(j => j.employmentID == emp.employmentID).jobPlacementSalary  * 0.11M;

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
            
        }
        //
      

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
            if (emp.employmentStatus == mainStatus.Inactive)
            {
                end = emp.employmentTerminationDate ?? end;
            }
            //-----------------------------------------------------------
            // CALCULATE SALARY EARNING
            //-----------------------------------------------------------
            decimal baseSalary = _db.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == emp.employmentID)?.jobPlacementSalary ?? 0m; // assumes monthly
            decimal hourlyRate = baseSalary / 208;
            //
            //-----------------------------------------------------------
            // Work hour calculations
            // find total working days in payroll period (business days) - implement according to your calendar
            //-----------------------------------------------------------
            int workingDaysInPeriod = await GetWorkingDaysInPeriod(start, end);
            decimal hoursPerDay = 8m;
            decimal totalPossibleHours = workingDaysInPeriod > 0? workingDaysInPeriod * hoursPerDay :0;
            //
            //-----------------------------------------------------------
            // GET ABSENT DAYS
            //-----------------------------------------------------------
            var absences = await _db.Leaves
                .Where(l => l.employmentID == emp.employmentID && l.leaveStartDate >= start && l.leaveEndDate <= end && l.leaveStatus == leaveStatus.Posted && l.leaveTypeModel.leaveGroup == leaveGroup.Absentism)
                .ToListAsync();
            decimal absentHours = absences.Sum(a => a.leaveDays * hoursPerDay); // ensure AbsentHours exists
            //
            //-----------------------------------------------------------
            // GET WORKED HOURS
            //-----------------------------------------------------------
            decimal workedHours = Math.Max(0, totalPossibleHours - absentHours);

            //
            //
            var earnings = new List<earningRecordModel>();
            //-----------------------------------------------------------
            //ADD UNKNOWN EARNING TYPE IF IT DOES NOT EXIST
            //-----------------------------------------------------------
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
            //-----------------------------------------------------------
            // OVERTIME WITHIN PERIOD
            //-----------------------------------------------------------
            var overtimeRecords = await _db.OvertimeRecords
                .Where(o => o.employmentID == emp.employmentID && o.overtimeRecordDate >= start && o.overtimeRecordDate <= end && o.overtimeRecordStatus == overtimeStatus.Posted)
                .ToListAsync();

            decimal overtimeAmount = overtimeRecords.Sum(o => o.GetOtCost);

            if (overtimeAmount > 0)
            {
                earningType = new earningType();
                earningType = await _db.EarningTypes.FirstOrDefaultAsync(d => d.earningTypeName.ToLower().Contains("overtime"));
                earnings.Add(new earningRecordModel { earningAmount = overtimeAmount, earningTypeID = earningType.earningTypeID, earningReference = "overtime", modifiedBy = "system" });
            }
            //-----------------------------------------------------------
            // ALLOWANCES (earnings)
            //------------------------------------------------------------
            var allowances = await _db.AllowanceAssignments
                .Include(a => a.allowanceModel)
                .Where(a => a.employmentID == emp.employmentID && a.allowanceStatus == mainStatus.Active)
                .ToListAsync();

            var taxableAllowances = allowances.Where(a => a.allowanceModel?.allowanceTaxable == true).ToList();
            decimal allowancesSum = allowances.Sum(a => a.allowanceAssignmentAmount);
            if (allowancesSum > 0)
            {
                earningType = new earningType();
                earningType = await _db.EarningTypes.FirstOrDefaultAsync(d => d.earningTypeName.ToLower().Contains("allowance"));
                earnings.Add(new earningRecordModel { earningAmount = allowancesSum, earningTypeID = earningType.earningTypeID, earningReference = "allowance", modifiedBy = "system" });
            }
            
            // BASE SALARY: if salaried, pro-rate monthly salary by workedHours/totalPossibleHours
            decimal basePay = 0m;
            if (empType.isSalaryAllowed)
            {
                basePay =totalPossibleHours == 0? 0: baseSalary * (workedHours / totalPossibleHours);
            }
            if (basePay > 0)
            {
                earningType = new earningType();
                earningType = await _db.EarningTypes.FirstOrDefaultAsync(d => d.earningTypeName.ToLower().Contains("salary"));
                earnings.Add(new earningRecordModel { earningAmount = basePay, earningTypeID = earningType.earningTypeID, earningReference = "salary", modifiedBy = "system" });
            }

            // GET OTHER EARNINGS (Bonus)

            var empEarnings = await _db.Earnings.Include(e => e.earningType).Where(e => e.employmentID == emp.employmentID && e.earningStatus == mainStatus.Active && e.earningType.isPayroll ==true).ToListAsync();

            // ADD OTHER EARNINGS
            foreach (var od in empEarnings)
            {
                // copy to new earning record attached to payroll computation
                earnings.Add(new earningRecordModel
                {
                    earningTypeID = od.earningTypeID,
                    earningAmount = CalculateEarningValue(od, baseSalary, allowancesSum),
                    earningReference = od.earningID.ToString(),
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

            // PENSION FROM BASE SALARY
            decimal pensionEmployee = 0;
            decimal pensionEmployer = 0;
            if (empType.isPensionAllowed)
            {
                pensionEmployee = Math.Round(baseSalary * 0.07m, 2);//OR FROM BASE PAY##############################
                pensionEmployer = Math.Round(baseSalary * 0.11m, 2);//######################################
               
            }

            // ADD PENSION TO DEDUCTIONS
            var pensionDedType = await _db.DeductionTypes.FirstOrDefaultAsync(d => d.deductionName.ToLower().Contains("pension"));
            deductions.Add(new deductionRecordModel
            {
                deductionTypeID = pensionDedType?.deductionTypeID ?? 0,
                deductionAmount = pensionEmployee,
                deductionReference = "pension",
                modifiedBy = "system"
            });
            //
            //TAX
            //TAXABLE EARNINGS WITHOUT SALARY, OVERTIME,AND ALLOWANCE
            decimal taxableEarnings = empEarnings.Where(e => e.earningType?.isTaxable == true)
                .Sum(e => CalculateEarningValue(e, baseSalary, allowancesSum));

            foreach(var e in empEarnings.Where(e => e.earningType?.isTaxable == true)){ Console.WriteLine("******************--- Taxable Earnings" + e.earningAmount); }

            //
            //ALL TAXABLE EARNING
            decimal taxable = basePay + overtimeAmount + taxableEarnings + allowances.Where(a => a.allowanceModel?.allowanceTaxable == true).Sum(a => a.allowanceAssignmentAmount);
            Console.WriteLine("#######-taxable======" + taxable);

            // TAX RATE LOOKUP
            var taxRate = await _db.TaxRates
                .Where(t => t.from <= taxable && (t.ceiling == 0 || t.ceiling >= taxable) && t.taxStatus == mainStatus.Active)
                .OrderBy(t => t.from)
                .FirstOrDefaultAsync() ?? new taxRateModel { taxRate = 0, deduction = 0 }; ;

            // GET TAX AMOUNT
            decimal tax = 0m;
            if (taxRate != null)
            {
                tax = Math.Max(0, Math.Round((taxable * taxRate.taxRate/100) - taxRate.deduction, 2));
                if (tax < 0) tax = 0;
            }
            Console.WriteLine("+=======================================================================================================================================+");
            Console.WriteLine("EMP ID = "+emp.givenID + " BASE= "+basePay + " WORKH = " + workedHours + " RATE = " + empEarnings + " TAXABLE = " + taxableEarnings + " DEDUCTABLE = " + taxRate.deduction + "**** TAX = " + tax);
            Console.WriteLine("+=======================================================================================================================================+");
            //
            // ADD TAX DEDUCTION TO DEDUCTIONS
            var taxDedType = await _db.DeductionTypes.FirstOrDefaultAsync(d => d.deductionName.ToLower().Contains("tax"));
            deductions.Add(new deductionRecordModel
            {
                deductionTypeID = taxDedType?.deductionTypeID ?? 0,
                deductionAmount = tax,
                modifiedBy = "system"
            });
            //
            //GET NET PAY AFTER TAX AND PENSION
            decimal netPay = gross - tax - pensionEmployee;
            //
            //
            // OTHER DEDUCTIONS (loans, penalties)
            var otherDeds = await _db.Deductions
                .Include(d => d.DeductionType)
                .Where(d => d.employmentID == emp.employmentID && d.deductionStatus == mainStatus.Active)
                .ToListAsync();

            // ADD OTHER DEDUCTIONS TO DEDUCTION HOLDER (respect priority)
            foreach (var od in otherDeds.OrderBy(d => d.DeductionType?.dedcutionPriority))
            {
                // copy to new deduction record attached to payroll computation
                deductions.Add(new deductionRecordModel
                {
                    deductionTypeID = od.deductionTypeID,
                    deductionAmount = CalculateDeductionValue(od, baseSalary, allowancesSum, gross, netPay),
                    modifiedBy = "system"
                });
            }

            //TOTAL DEDUCTIONS AMOUNT
            decimal totalDeductions = deductions.Sum(d => d.deductionAmount ?? 0);

            //TOTAL NET PAYABLE
            decimal net = gross - totalDeductions;

            //CREATE A PAYROLL ROW FOR THE EMPLOYEE
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

            if(start > end) { return 0; }

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

                var earnings = await _db.Earnings.Where(e => e.earningStatus == mainStatus.Active && empIds.Contains(e.employmentID)).ToListAsync();
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
                return (d.deductionAmount / 100m) * baseValue;

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
                return (e.earningAmount / 100m) * baseValue;

            return e.earningAmount; // Fixed amount
        }

    }
}
