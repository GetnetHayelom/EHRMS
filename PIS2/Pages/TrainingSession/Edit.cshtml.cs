using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

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
            
            Session = await _context.TrainingSessions.Include(s => s.PersonModel).FirstOrDefaultAsync(m => m.trainingSessionID == id);
            if (Session == null) return NotFound();

            TrainerList = new SelectList(_context.Persons, "personID", "personFullName", Session.personID);
            TrainingList = new SelectList(_context.Trainings, "trainingID", "trainingTitle");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Session.ModifiedBy");
            ModelState.Remove("Session.ModifiedDate");
            
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                }
                
                TrainerList = new SelectList(_context.Persons, "personID", "personFullName", Session.personID);
                TrainingList = new SelectList(_context.Trainings, "trainingID", "trainingTitle", Session.trainingID);
                return Page();
            }

            var existing = await _context.TrainingSessions.FirstOrDefaultAsync(t => t.trainingSessionID == Session.trainingSessionID);

            if (existing == null)
                return NotFound();

            existing.trainingID = Session.trainingID;
            existing.personID = Session.personID;
            existing.startDate = Session.startDate;
            existing.endDate = Session.endDate;
            existing.modifiedBy = User.Identity.Name;
            existing.modifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
           
            return RedirectToPage("./Details", new {id=Session.trainingSessionID});
        }
    }
}