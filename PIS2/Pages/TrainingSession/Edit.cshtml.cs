using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.TrainingSession
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        public EditModel(PISContext context) => _context = context;

        [BindProperty]
        public trainingSessionModel Session { get; set; } = default!;
        public SelectList TrainingList { get; set; } = default!;
        public SelectList TrainerList { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            
            Session = await _context.TrainingSessions.FirstOrDefaultAsync(m => m.trainingSessionID == id);
            if (Session == null) return NotFound();

            TrainerList = new SelectList(_context.Persons, "personID", "personFullName", Session.personID);
            TrainingList = new SelectList(_context.Trainings, "trainingID", "trainingTitle");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Session.modifiedBy = User.Identity.Name;
            Session.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                TrainingList = new SelectList(_context.Trainings, "trainingID", "trainingTitle");
                return Page();
            }

            _context.Attach(Session).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}