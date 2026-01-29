using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
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
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public CreateModel(PIS2.Models.PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string ErrorMessage { get; set; }

        int? EmpID { get; set; }
        public decimal SeverancePay { get; set; }
        public AnnualLeaveSummary? LeaveSummary { get; set; }
        public decimal YearsOfService { get; set; }
        public jobPlacementModel? jobPlacement { get; set; }
        public async Task<IActionResult> OnGet(int? id)
        {
            if (string.IsNullOrEmpty(givenID) && id == null)
            {
                return Page();
            }
            
            if (!string.IsNullOrEmpty(givenID))
            {
                employmentModel = await _context.Employments.Include(e => e.personModel)
                    .Where(e => e.employmentStatus == mainStatus.Active)
                    .FirstOrDefaultAsync(e => e.givenID == givenID);
                              
            }

            if (id != null && id != 0)
            {
                employmentModel = await _context.Employments.Include(e => e.personModel)
                    .Where(e => e.employmentStatus == mainStatus.Active)
                    .FirstOrDefaultAsync(e => e.employmentID == id);
            }

            if (employmentModel == null)
            {
                //TempData["message"] = ("Error", $"Employee Record Not Found!");
                return NotFound();
            }

            if (employmentModel.employmentStatus != mainStatus.Active)
            {
                //TempData["message"] = ("Error", $"Employee is not active.");
                return NotFound();
            }

            var exiTermination = await _context.Terminations.Where(t => t.employmentID == employmentModel.employmentID).ToListAsync();
            
            if (exiTermination.Any())
            {
                //TempData["message"] = ("Error", $"Termination exists for this employment!");
                return RedirectToPage("Details", new { id = exiTermination.First().terminationID });
            }

            SeverancePay =await _core.GetSeverance(employmentModel.employmentID);
            LeaveSummary = await _context.AnnualLeaveSummary.FirstOrDefaultAsync(a => a.employmentID == employmentModel.employmentID);
            givenID = employmentModel.givenID;
            YearsOfService = (decimal) ((DateTime.Now - employmentModel.employmentDate).TotalDays)/365.25m;
                

            jobPlacement = await _context.JobPlacements
                .Include(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Include(jp => jp.jobModel)
                .FirstOrDefaultAsync(j => j.employmentID == employmentModel.employmentID && j.jobPlacementStatus == mainStatus.Active);
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
