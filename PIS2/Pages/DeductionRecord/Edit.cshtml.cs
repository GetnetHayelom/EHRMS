using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.DeductionRecord
{
    public class EditModel : PageModel
    {
        private readonly PISContext _db;

        public EditModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public deductionModel Deduction { get; set; }

        public SelectList DeductionTypes { get; set; }
        public SelectList Employees { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!(User.IsInRole("MIE\\PMS_HRCLERK") || User.IsInRole("MIE\\PMS_HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }
            Deduction = await _db.Deductions.FindAsync(id);

            if (Deduction == null)
                return NotFound();

            DeductionTypes = new SelectList(await _db.DeductionTypes.ToListAsync(), "deductionTypeID", "deductionName");
            Employees = new SelectList(await _db.Employments.ToListAsync(), "employmentID", "givenID");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.IsInRole("MIE\\PMS_HRCLERK") || User.IsInRole("MIE\\PMS_HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }
            ModelState.Clear();
            Deduction.modifiedBy = User.Identity?.Name ?? "System";

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
                await OnGetAsync(Deduction.deductionID);
                return Page();
            }

            

            _db.Attach(Deduction).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
