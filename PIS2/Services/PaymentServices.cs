using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Services
{
    public class PaymentServices
    {
        private readonly PISContext _db;
        private readonly ILogger<PayrollService> _logger;
        private readonly Core _core;
        const decimal hoursPerDay = 8m;
        const string defaultSubAccName = "3000000000000";
        const string OtTypeCode = "OT";
        const string SalaryTypeCode = "SALARY";
        const string AllowanceTypeCode = "TALLOW";
        const string TaxTypeCode = "PAYE";
        const string EMPLOYEE_CREDIT_ACCOUNT_NAME = "Account Payables-Employees";
        const decimal avgWorkingHoursPerMonth = 208;

        public PaymentServices(PISContext db, ILogger<PayrollService> logger, Core core)
        {
            _db = db;
            _logger = logger;
            _core = core;
        }

        public async Task<IActionResult> PostPaymentAsync(int payID, string modifiedBy)
        {
            var payment = await _db.OtherPayments.FirstOrDefaultAsync(p => p.paymentID == payID);
            if (payment == null)
            {
                _logger.LogError("Error: Payment not found. {PaymentID} {User}", payment?.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment not found!" });
            }
            if (payment.paymentStatus != payrollStatus.PROCESSED)
            {
                _logger.LogError("Error: Payment is not processed yet. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment is not processed yet!" });
            }

            payment.paymentStatus = payrollStatus.POSTED;
            payment.modifiedBy = modifiedBy;
            try {await _db.SaveChangesAsync();
                _logger.LogWarning("MSG: Payment Posted Successfully!. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = true, message = "Payment Posted Successfully!" });
            } catch (Exception ex) 
            {
                _logger.LogError(ex.Message,"Error: Payment posting failed!. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment Posting Failed!" });
            }
            
        }
        public async Task<IActionResult> DiscardPaymentAsync(int payID, string modifiedBy)
        {
            var payment = await _db.OtherPayments.FirstOrDefaultAsync(p => p.paymentID == payID);
            if (payment == null)
            {
                _logger.LogError("Error: Payment not found. {PaymentID} {User}", payment?.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment not found!" });
            }
            if (payment.paymentStatus == payrollStatus.COMPLETED)
            {
                _logger.LogError("Error: Can not disccard payment. Payment is completed. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Can not disccard payment. Payment is completed yet!" });
            }

            payment.paymentStatus = payrollStatus.DISCARDED;
            payment.modifiedBy = modifiedBy;
            try
            {
                await _db.SaveChangesAsync();
                _logger.LogWarning("MSG: Payment Rejected Successfully!. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = true, message = "Payment Posted Successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error: Payment rejection failed!. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment Regecting Failed!" });
            }
        }
        public async Task<IActionResult> CompletePaymentAsync(int payID, string modifiedBy)
        {
            var payment = await _db.OtherPayments.Include(p => p.earningModel).FirstOrDefaultAsync(p => p.paymentID == payID);
            if (payment == null)
            {
                _logger.LogError("Error: Payment not found. {PaymentID} {User}", payment?.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment not found!" });
            }
            
            if (payment.paymentStatus != payrollStatus.POSTED)
            {
                _logger.LogError("Error: Payment is not posted. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment is not poseted yet!" });
            }

            if (_db.JournalEntries.Any(j => j.Reference == $"Payment-{payID}"))
            {
                _logger.LogError("Error: Payment is complete. {PaymentID} {User}", payment.paymentID, modifiedBy);
                return new JsonResult(new { success = false, message = "Payment is completed!" });
            }
            
            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {

                // mark payment completed
                payment.paymentStatus = payrollStatus.COMPLETED;
                payment.modifiedBy = modifiedBy;
               
                var empId = payment.earningModel?.employmentID;
                var dep = _db.JobPlacements.FirstOrDefault(j => j.employmentID == empId && j.jobPlacementStatus == mainStatus.Active).departmentModel;

                var journal = new JournalEntry
                {
                    EntryDate = DateTime.Now,
                    Reference = $"Payment-{payment.paymentID}",
                    Description = "Payment posting",
                    modifiedBy = modifiedBy,
                    modifiedDate = DateTime.Now,
                    Lines = new List<JournalEntryLine>()
                };
  
                // Debit Payment Expense
                journal.Lines.Add(new JournalEntryLine
                {
                    accountID = dep.subAccountModel.accountID,
                    subAccountID = payment.CreditAccountID,
                    Debit = payment.GrossPay,
                    Credit = 0
                });

                // Credit Net Pay (Cash/Payable)
                journal.Lines.Add(new JournalEntryLine
                {
                    accountID = dep.subAccountModel.accountID,
                    subAccountID = payment.CreditAccountID,
                    Debit = 0,
                    Credit = payment.NetPay
                });

                journal.Lines.Add(new JournalEntryLine
                {
                    accountID = dep.subAccountModel.accountID,
                    subAccountID = payment.CreditAccountID,
                    Debit = 0,
                    Credit = payment.GrossPay - payment.NetPay
                });

                var totalDebit = journal.Lines.Sum(l => l.Debit);
                var totalCredit = journal.Lines.Sum(l => l.Credit);

                if (totalDebit != totalCredit)
                    throw new Exception("Journal not balanced");

                _db.JournalEntries.Add(journal);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to complete payment");
                throw;
            }
            return new JsonResult(new { success =true, message="Payment completed!."});
        }

        public async Task<decimal> GetUnpaidSalaryAsync(employmentModel emp, DateTime? empLastPay, DateTime? endDate, jobPlacementModel jobp, List<leaveModel> absences, string userName)
        {
            var empType = emp.employmentTypeModel;
            // BASE SALARY: if salaried, pro-rate monthly salary by workedHours/totalPossibleHours
            decimal basePay = 0m;
            if (!empType.isSalaryAllowed)
            {//NB. REPLACE avgWorkingHoursPerMonth BY totalPossibleHours FOR STRICT  WORKING HOURS CALCULATION
                _logger.LogError("Error: Employment is not salary based! {EmployeeID} {UserName}", emp.employmentID, userName);
            }
            
            // load relevant records
            var start = empLastPay?.AddDays(1) ?? DateTime.Now;
            var end = endDate ?? DateTime.Now;
            
            if (emp.employmentDate > start)
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
            //GET LAST JOB PLACEMENT

            decimal baseSalary = jobp?.jobPlacementSalary ?? 0m; // assumes monthly

            decimal hourlyRate = baseSalary / avgWorkingHoursPerMonth;
            //
            //-----------------------------------------------------------
            // find total working days in payroll period (business days) 
            //-----------------------------------------------------------
            int workingDaysInPeriod = _core.GetWorkingDaysInPeriod(start, end);
            var daysInRange = (end.Date - start.Date).Days + 1;
            var isMonthly = daysInRange >= 28 && daysInRange <= 31 ? true : false;
            decimal totalPossibleHours = workingDaysInPeriod > 0 ? workingDaysInPeriod * hoursPerDay : 0;

            decimal absentHours = absences.Sum(a => a.leaveDays * hoursPerDay); // ensure AbsentHours exists
      
            decimal workedHours = Math.Max(0, totalPossibleHours - absentHours);

            basePay = workedHours * hourlyRate;

            return basePay;
        }
        public async Task<decimal> GetUnpaidOt(employmentModel emp, DateTime? empLastPay, DateTime? endDate, string userName) 
        {
            var start = empLastPay?.AddDays(1) ?? emp.employmentDate;
            var end = endDate ?? DateTime.Now;

            if (emp.employmentDate > start)
            {
                start = emp.employmentDate;
            }
            if (emp.employmentStatus == mainStatus.Inactive)
            {
                end = emp.employmentTerminationDate ?? end;
            }
            var overtimeRecords = await _db.OvertimeHistoryView.Where(o => o.employmentID == emp.employmentID && o.overtimeHistoryAction == overtimeStatus.Posted && o.overtimeRecordStatus == overtimeStatus.Posted).ToListAsync();
            if (overtimeRecords == null) 
            {
                _logger.LogError("Error: No Overtime Record! {EmployeeID} {UserName}", emp.givenID, userName);
                return 0;
            }

            decimal overtimeAmount = overtimeRecords.Sum(o => o.overtimeAmount);

            return overtimeAmount;

        }

       
    }
}
