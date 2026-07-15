using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace PIS2.Pages.Termination
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK,MIE\\PMS_MANAGEMENT")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly PayrollService _payrollService;
        private readonly Global_S _global;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(PISContext context, Core core, PayrollService payrollService, Global_S global, ILogger<DetailsModel> logger)
        {
            _context = context;
            _core = core;
            _payrollService = payrollService;
            _global = global;
            _logger = logger;
        }

        public terminationModel terminationModel { get; set; } = default!;
        public decimal SeverancePay { get; set; }
        public AnnualLeaveSummary LeaveSummary { get; set; } = new AnnualLeaveSummary();
        public decimal YearsOfService { get; set; }
        public payrollPay Payments { get; set; }
        public EmployeeDetailView employeeDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terminationmodel = await _context.Terminations
                .Include(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(e => e.EmploymentModel).ThenInclude(e => e.employmentTypeModel)
                .FirstOrDefaultAsync(m => m.terminationID == id);
            if (terminationmodel == null)
            {
                return NotFound();
            }
            else
            {
                terminationModel = terminationmodel;
                // Fetch Audit Logs for this specific record
                History = await _context.AuditLogs
                    .Where(a => a.TableName == "Terminations" && a.RecordID == id)
                    .OrderByDescending(a => a.ModifiedDate)
                    .ToListAsync();

                employeeDetail = await _context.EmployeeDetailViews.FirstOrDefaultAsync(e => e.EmploymentID == terminationModel.employmentID);
                SeverancePay = await _core.GetSeverance(terminationModel.employmentID);
                var leaveSum = await _context.AnnualLeaveSummary.FirstOrDefaultAsync(a => a.employmentID == terminationModel.employmentID);
                var leave = await _context.Leaves.OrderByDescending(l => l.leaveRequestDate).FirstOrDefaultAsync(l => l.employmentID == terminationModel.employmentID && (l.leaveStatus == leaveStatus.Posted || l.leaveStatus == leaveStatus.Completed));
                YearsOfService = (decimal)((terminationModel.terminationDate - terminationModel.EmploymentModel.employmentDate).TotalDays) / 365.25m;
                if (leaveSum?.adjustedLeaveBalance > 0)
                {
                    LeaveSummary = leaveSum;
                }
                else
                {
                    LeaveSummary = new AnnualLeaveSummary();
                    LeaveSummary.adjustedLeaveBalance = leave?.leaveDays ?? 0;
                    LeaveSummary.adjustedLeaveBalanceCost = leave?.leaveCost ?? 0;
                }

                if (terminationModel.terminationStatus == terminationStatus.Hold || terminationModel.terminationStatus == terminationStatus.Approved)
                {
                    var pays = await _payrollService.GetExitPay(terminationModel.EmploymentModel);
                    Payments = pays?.PayrollPays?.FirstOrDefault() ?? new payrollPay();
                }
                else
                {
                    var pays = await _context.Payrolls.FirstOrDefaultAsync(e => e.payrollName.Contains(employeeDetail.GivenID));
                    Payments = pays?.PayrollPays?.FirstOrDefault() ?? new payrollPay();
                }
            }
            return Page();
        }

        [BindProperty(SupportsGet =true)]
        public int terminationID { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!User.IsInRole("MIE\\PMS_HRCLERK") || !User.IsInRole("MIE\\PMS_HRMANAGER")) 
            {
                _logger.LogError("Error: Access denied! User not allowed posting a termination! {UseName}", User.Identity.Name);
                return RedirectToPage("/Shared/AccessDenied");
            }
            var termination = await _context.Terminations.FindAsync(terminationID);
            var employee = await _context.Employments.AsNoTracking()
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .FirstOrDefaultAsync(e => e.employmentID == termination.employmentID) ?? new employmentModel();
            if (termination == null)
            {
                _logger.LogError("Error: Termination Not Found! {TerminationID} {UseName}", terminationID, User.Identity.Name);
                return new JsonResult(new { success=false, message="Termination not found for given id!"});
            }
            if(termination.terminationStatus != terminationStatus.Approved)
            {
                _logger.LogError("Error: Termination must be approved before posting! {UserName}", User.Identity.Name);
                return new JsonResult(new { success = false, message = "Termination not Approved!" });
            }
            
            termination.modifiedBy = User.Identity.Name;
            termination.terminationStatus = terminationStatus.Posted;
                        
            var Earning = new List<earningModel>();
            var payroll = await _payrollService.GetExitPay(employee);
            var Pays = payroll.PayrollPays?.FirstOrDefault() ?? new payrollPay();
            Pays.modifiedBy = User.Identity.Name;
            payroll.IsPayroll = false;
            
            foreach (var p in Pays.EarningRecords) 
            {
                Earning.Add(new earningModel 
                {
                    earningTypeID = p.earningTypeID,
                    employmentID = termination.employmentID,
                    earningReference = termination.terminationID.ToString(),
                    earningStatus = mainStatus.Suspended,
                    earningIteration =1,
                    remainingIteration=1,
                    IsPercentage =false,
                    earningBase =earningBase.NONE,
                    earningAmount=p.earningAmount,
                    modifiedBy = User.Identity.Name
                });
            }
            var transact = await _context.Database.BeginTransactionAsync();
            
            try
            {
                _context.Attach(termination).State = EntityState.Modified;
                _context.Add(payroll);
                _context.AddRange(Earning);
                await _context.SaveChangesAsync();
                await transact.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message.ToString(), User.Identity.Name);
                return new JsonResult(new { success=false, message="An error occured while trying to post termination!"});
            }

            return RedirectToPage("./Details", new { id = terminationID });
        }

        public async Task<IActionResult> OnPostApprove()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                _logger.LogError("Error: Access denied! User not allowed approving a termination! UserName: {UseName}", User.Identity.Name);
                return RedirectToPage("/Shared/AccessDenied");
            }
            var termination = await _context.Terminations.FindAsync(terminationID);
            var employee = await _context.Employments.Include(e => e.personModel).FirstOrDefaultAsync(e => e.employmentID == termination.employmentID);
            if (termination == null)
            {
                _logger.LogError("Error: Termination Not Found! {TerminationID} {UseName}", terminationID, User.Identity.Name);
                return new JsonResult(new { success = false, message = "Termination not found for given id!" });
            }
            if (termination.terminationStatus != terminationStatus.Hold)
            {
                _logger.LogError("Error: Termination must be on hold before approving! {UserName}", User.Identity.Name);
                return new JsonResult(new { success = false, message = "Only termination on hold status can be approved!" });
            }

            termination.modifiedBy = User.Identity.Name;
            termination.terminationStatus = terminationStatus.Approved;
           
            try
            {
                _context.Attach(termination).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message.ToString(), User.Identity.Name);
                return new JsonResult(new { success = false, message = "An error occured while trying to approve termination!" });
            }

            return RedirectToPage("./Details", new { id = terminationID });
        }
        public List<Models.AuditLog> History { get; set; } = new();
        public string FormatAuditValue(string columnName, string value)
        {
            if (string.IsNullOrEmpty(value)) return "None";

            // Handle Booleans
            if (columnName.StartsWith("is"))
            {
                return value.ToLower() == "true" ? "Yes" : "No";
            }

            // Handle Enums (assuming mainStatus is 0=Active, 1=Inactive, etc.)
            if (columnName == "terminationStatus")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((terminationStatus)enumValue).ToString();
                }
                return value;
            }

            return value;
        }
        public string GetFriendlyColumnName(string columnName)
        {
            return columnName switch
            {
                "terminationDate" => "Termination Date",
                "terminationReason" => "Reason",
                "terminationRemark" => "Remark",
                "terminationStatus" => "Status",
                _ => columnName
            };
        }
    }
}
