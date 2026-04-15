using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;


namespace PIS2.Pages.EarningRecord
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PISContext _db;

        public EditModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public earningModel Earning { get; set; }

        public SelectList EarningTypes { get; set; }
        public SelectList Employees { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Earning = await _db.Earnings.Include(e => e.earningType).FirstOrDefaultAsync(e => e.earningID == id);

            if (Earning == null)
                return NotFound();

            EarningTypes = new SelectList(await _db.EarningTypes.ToListAsync(), "earningTypeID", "earningTypeName");
            Employees = new SelectList(await _db.Employments.ToListAsync(), "employmentID", "givenID");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.IsInRole("MIE\\PMS_HRCLERCK") || User.IsInRole("MIE\\PMS_HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied");}
            ModelState.Clear();
            Earning.modifiedBy = User.Identity?.Name;
            if (!Earning.earningType.isPayroll) 
            {
                
                Earning.IsPercentage = false;
                Earning.earningIteration = 1;
                Earning.earningBase = earningBase.NONE;
            }
            
            
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
                await OnGetAsync(Earning.earningID);
                return Page();
            }

            

            _db.Attach(Earning).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }

}
