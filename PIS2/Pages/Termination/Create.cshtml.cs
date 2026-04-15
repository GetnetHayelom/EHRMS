using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Pages.EmployeeService;
using PIS2.Services;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Termination
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly PayrollService _payrollService;
        private ILogger<CreateModel> _logger;
        private Globals _global;

        public CreateModel(PISContext context, Core core, PayrollService payrollService, ILogger<CreateModel> logger, Globals global)
        {
            _context = context;
            _core = core;
            _payrollService = payrollService;
            _logger = logger;
            _global = global;
        }
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string ErrorMessage { get; set; }
        int? EmpID { get; set; }
        public decimal SeverancePay { get; set; }
        public AnnualLeaveSummary? LeaveSummary { get; set; }
        public decimal YearsOfService { get; set; }
        public jobPlacementModel? jobPlacement { get; set; }
        public payrollPay PayrollPay { get; set; }
        public decimal OT { get; set; }
        public decimal Allowances { get; set; }
        public async Task<IActionResult> OnGet(int? id)
        {
            if (string.IsNullOrEmpty(givenID) && id == null)
            {
                return Page();
            }
            
            if (!string.IsNullOrEmpty(givenID))
            {
                employmentModel = await _context.Employments.Include(e => e.personModel)
                    .Include(e => e.employmentTypeModel)
                    .Where(e => e.employmentStatus == mainStatus.Active)
                    .FirstOrDefaultAsync(e => e.givenID == givenID);                              
            }

            if (id != null && id != 0)
            {
                employmentModel = await _context.Employments.Include(e => e.personModel)
                    .Include(e => e.employmentTypeModel)
                    .Where(e => e.employmentStatus == mainStatus.Active)
                    .FirstOrDefaultAsync(e => e.employmentID == id);
            }

            if (employmentModel == null)
            {
                _logger.LogError("Error: Employee Not Found {UserName:}", User.Identity.Name);
                return new JsonResult(new { success=false, message = "Employee Record Not Found!"});
            }

            if (employmentModel.employmentStatus != mainStatus.Active)
            {
                _logger.LogError("Error: Employee is not active {UserName:}", User.Identity.Name);
                return new JsonResult(new { success = false, message = "Employee is not active!" });
            }

            var exiTermination = await _context.Terminations.Where(t => t.employmentID == employmentModel.employmentID).ToListAsync();
            
            if (exiTermination.Any())
            {
                //TempData["message"] = ("Error", $"Termination exists for this employment!");
                //return RedirectToPage("Details", new { id = exiTermination.First().terminationID });
                _logger.LogError("Error: Termination request exists for this employment {UserName:}", User.Identity.Name);
                return new JsonResult(new { success = false, message = "Termination request exists for this employment!" });
            }
            var LastPay =await _payrollService.GetLastPayroll(employmentModel.employmentID);

            SeverancePay =await _core.GetSeverance(employmentModel.employmentID);
            LeaveSummary = await _context.AnnualLeaveSummary.FirstOrDefaultAsync(a => a.employmentID == employmentModel.employmentID);
            givenID = employmentModel.givenID;
            YearsOfService = (decimal) ((DateTime.Now - employmentModel.employmentDate).TotalDays)/365.25m;

            var absences = await _context.LeaveHistoryView
                .Where(l => l.employmentID == employmentModel.employmentID && l.leaveGroup == leaveGroup.Absentism
                && l.leaveStatus == leaveStatus.Posted && l.leaveHistoryAction == leaveStatus.Posted && l.modifiedDate > LastPay)
                .SumAsync(l => l.leaveDays);

            var OTs = await _context.OvertimeHistoryView
                .Where(o => o.employmentID == employmentModel.employmentID && o.overtimeRecordStatus == overtimeStatus.Posted
                && o.overtimeHistoryAction == overtimeStatus.Posted && o.modifiedDate > LastPay)
                .ToListAsync();
            var OT = OTs.Sum(o => o.overtimeAmount);

            var earning = await _context.Earnings.Where(e => e.employmentID == employmentModel.employmentID && e.earningStatus == mainStatus.Active).ToListAsync();
            var allowances = await _context.AllowanceAssignments.Where(a => a.employmentID == employmentModel.employmentID && a.allowanceStatus == mainStatus.Active).ToListAsync();
            var taxRates = await _context.TaxRates.Where(t => t.taxStatus == mainStatus.Active).ToListAsync();
            var OtherDeds = await _context.Deductions.AsNoTracking().Include(d => d.DeductionType).Where(d => d.deductionStatus == mainStatus.Active).ToListAsync();
            
            jobPlacement = await _context.JobPlacements
                .Include(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Include(jp => jp.jobModel)
                .FirstOrDefaultAsync(j => j.employmentID == employmentModel.employmentID && j.jobPlacementStatus == mainStatus.Active);
            
            var payroll = new payrollModel();
             var Pays = await _payrollService.CalculateEmployeePayAsync(employmentModel,
                payroll, _global.pensionDedType,LastPay, jobPlacement, absences, OT, _global.OtEarningType, 
                earning, allowances, _global.AllowanceEarningType, _global.SalaryEarningType, _global.TaxDeductionType,  
                taxRates, OtherDeds, employmentModel.personModel.subAccountID ?? 0, jobPlacement?.departmentModel?.subAccountID ??0);
            PayrollPay = Pays;
            return Page();
        }

        [BindProperty]
        public terminationModel terminationModel { get; set; } = default!;
        public employmentModel employmentModel { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("terminationModel.modifiedBy");
            ModelState.Remove("modifiedBy");
            ModelState.Remove("givenID");
            terminationModel.modifiedBy = User.Identity.Name;
            terminationModel.terminationStatus = terminationStatus.Hold;
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        
                    }
                }
                TempData["message"] = ("Error", $"Missing field!");
                
                return Page();
            }

  
            _context.Terminations.Add(terminationModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id =terminationModel.terminationID});
        }
    }
}
