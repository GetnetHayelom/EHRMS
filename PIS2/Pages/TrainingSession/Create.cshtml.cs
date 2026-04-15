using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models;
using System.Threading.Tasks;

namespace PIS2.Pages.TrainingSession
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        public CreateModel(PISContext context) => _context = context;

        [BindProperty]
        public trainingSessionModel Session { get; set; } = default!;

        public SelectList TrainingList { get; set; } = default!;
        public SelectList TrainerList { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            TrainingList = new SelectList(_context.Trainings, "trainingID", "trainingTitle", id);

            //var person = await _context.Persons()
            TrainerList = new SelectList(_context.Persons, "personID", "personFullName");
            //Session = new trainingSessionModel { startDate = DateTime.Now, endDate = DateTime.Now.AddHours(2) };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            Session.modifiedBy = User.Identity.Name;
            Session.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                TrainerList = new SelectList(_context.Persons, "personID", "personFullName");
                TrainingList = new SelectList(_context.Trainings, "trainingID", "trainingTitle");
                return Page();
            }

            _context.TrainingSessions.Add(Session);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Training/Details", new {id=Session.trainingID});
        }
    }
}