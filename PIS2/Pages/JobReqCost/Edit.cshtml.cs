using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.JobReqCost
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobReqCost JobReqCost { get; set; }

        public SelectList VacancyList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            JobReqCost = await _context.JobReqCosts.FindAsync(id);
            if (JobReqCost == null) return NotFound();

            VacancyList = new SelectList(_context.Vacancies, "VacancyID", "VacancyTitle", JobReqCost.VacancyID);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("JobReqCost.modifiedBy");
            JobReqCost.modifiedBy = User?.Identity?.Name ?? "System";
            JobReqCost.modifiedDate = DateTime.Now;
            JobReqCost.jobReqStatus = jobReqStatus.Hold;
            
            if (!ModelState.IsValid)
                return Page();

            JobReqCost.modifiedBy = User?.Identity?.Name ?? "System";
            JobReqCost.modifiedDate = DateTime.Now;

            _context.Attach(JobReqCost).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
