using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System.ComponentModel;


namespace PIS2.Pages.EarningRecord
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _db;

        public CreateModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public earningModel Earning { get; set; }

        public List<earningType> EarningTypes { get; set; }

    

        public async Task OnGet(int? id)
        {
            Earning = new earningModel();

            EarningTypes = await _db.EarningTypes.Where(e => !new[] { "salary", "allowance","overtime" }.Contains(e.earningTypeName.ToLower())).ToListAsync();

            // If id is passed, auto-load employee
            if (id.HasValue)
            {
                // Fetch employee details
                var emp = await _db.Employments
                    .FirstOrDefaultAsync(e => e.employmentID == id && e.employmentStatus == mainStatus.Active);

                if (emp != null)
                {
                    // Pre-fill hidden employmentID
                    ViewData["GivenID"] = emp.givenID;

                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.IsInRole("MIE\\PMS_HRCLERCK") || User.IsInRole("MIE\\PMS_HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }
            ModelState.Clear();

            Earning.modifiedBy = User.Identity.Name;
            var earninType = await _db.EarningTypes.FirstOrDefaultAsync(e => e.earningTypeID == Earning.earningTypeID);
            if (!earninType.isPayroll)
            {
                Earning.IsPercentage = false;
                Earning.earningIteration = 1;
                Earning.earningBase = earningBase.NONE;
            }
            Earning.remainingIteration = Earning.earningIteration;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                //await OnGet();
                return Page();
            }

            Earning.modifiedBy = User.Identity?.Name ?? "System";

            _db.Earnings.Add(Earning);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
