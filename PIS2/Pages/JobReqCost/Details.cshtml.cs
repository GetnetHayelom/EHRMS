using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobReqCost
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public jobReqCost JobReqCost { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            JobReqCost = await _context.JobReqCosts
                .Include(j => j.VacancyModel)
                .FirstOrDefaultAsync(j => j.jobReqCostID == id);

            if (JobReqCost == null) return NotFound();
            return Page();
        }
    }
}
