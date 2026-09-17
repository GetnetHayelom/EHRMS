using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.Training
{
    [Authorize(Roles = "HRCLERK, HRMANAGER")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public trainingModel Training { get; set; } = default!;
        public List<trainingSessionModel> Sessions { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Trainings == null)
            {
                return NotFound();
            }

            var trainingmodel = await _context.Trainings
                .Include(t => t.TrainingSessions)
                .FirstOrDefaultAsync(m => m.trainingID == id);

            if (trainingmodel == null)
            {
                return NotFound();
            }
            else
            {
                var session = await _context.TrainingSessions.Include(s => s.PersonModel).Where(s => s.trainingID == id).ToListAsync();
                Sessions = session.Any() ? session : new List<trainingSessionModel>(); 
                Training = trainingmodel;
                
            }
            return Page();
        }
    }
}