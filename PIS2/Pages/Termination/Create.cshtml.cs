using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
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
    [Authorize(Roles = "HRMANAGER, HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly PayrollService _payrollService;
        private ILogger<CreateModel> _logger;
        private Global_S _global;

        public CreateModel(PISContext context, Core core, PayrollService payrollService, ILogger<CreateModel> logger, Global_S global)
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
        public decimal SeverancePay { get; set; }
        public AnnualLeaveSummary? LeaveSummary { get; set; }
        public decimal YearsOfService { get; set; }
        public jobPlacementModel? jobPlacement { get; set; }
        public payrollPay PayrollPay { get; set; }
        public EmployeeDetailView employeeDetail { get; set; }
        public DateTime lastPayTest { get; set; }
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
                _logger.LogError("Error: Termination request exists for this employment {UserName:}", User.Identity.Name);
                return new JsonResult(new { success = false, message = "Termination request exists for this employment!" });
            }
            var LastPay =await _payrollService.GetLastPayroll(employmentModel.employmentID);
            
            SeverancePay =await _core.GetSeverance(employmentModel.employmentID);
            LeaveSummary = await _context.AnnualLeaveSummary.FirstOrDefaultAsync(a => a.employmentID == employmentModel.employmentID);
            givenID = employmentModel.givenID;
            YearsOfService = (decimal) ((DateTime.Now - employmentModel.employmentDate).TotalDays)/365.25m;
            var pays = await _payrollService.GetExitPay(employmentModel);
            PayrollPay = pays.PayrollPays.FirstOrDefault();

            employeeDetail = await _context.EmployeeDetailViews.FirstOrDefaultAsync(e => e.EmploymentID == employmentModel.employmentID);
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
