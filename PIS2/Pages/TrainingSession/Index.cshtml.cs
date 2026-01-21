using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.TrainingSession
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        public IndexModel(PISContext context) => _context = context;

        public IList<trainingSessionModel> Sessions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Sessions = await _context.TrainingSessions
                .Include(s => s.Training)
                .OrderByDescending(s => s.startDate)
                .ToListAsync();
        }
    }
}