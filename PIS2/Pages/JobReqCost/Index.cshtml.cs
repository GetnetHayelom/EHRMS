using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.JobReqCost
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<jobReqCost> JobReqCosts { get; set; } = new List<jobReqCost>();

        public async Task OnGetAsync()
        {
            JobReqCosts = await _context.JobReqCosts
                .Include(j => j.VacancyModel)
                .OrderByDescending(j => j.modifiedDate)
                .ToListAsync();
        }
    }
}
