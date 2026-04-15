using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Training
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<trainingModel> Trainings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // We include TrainingSessions in case you want to show "Session Count" in the table
            Trainings = await _context.Trainings
                .Include(t => t.TrainingSessions)
                .OrderByDescending(t => t.trainingID)
                .ToListAsync();
        }
    }
}