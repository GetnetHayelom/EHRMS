using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.DeductionRecord
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _db;

        public CreateModel(PISContext db)
        {
            _db = db;
        }
        [BindProperty]
        public deductionModel Deduction { get; set; }

        public SelectList DeductionTypes { get; set; }
        public SelectList Employees { get; set; }

        public async Task OnGet(int? id)
        {
            DeductionTypes = new SelectList(await _db.DeductionTypes.ToListAsync(), "deductionTypeID", "deductionName");
            Employees = new SelectList(await _db.Employments.ToListAsync(), "employmentID", "givenID");

            // If id is passed, auto-load employee
            if (id.HasValue)
            {
                // Fetch employee details
                var emp = await _db.Employments
                    .FirstOrDefaultAsync(e => e.employmentID == id);

                if (emp != null)
                {
                    // Pre-fill hidden employmentID
                    ViewData["GivenID"] = emp.givenID;
                   
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRCLERK")) { return RedirectToPage("/Shared/AccessDenied"); }
            ModelState.Clear();
            Deduction.modifiedBy = User.Identity?.Name ?? "System";
            Deduction.remainingIteration = Deduction.deductionIteration;
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

            

            _db.Deductions.Add(Deduction);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}

