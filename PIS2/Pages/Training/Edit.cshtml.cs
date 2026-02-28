using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Training
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public trainingModel Training { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Trainings == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings.FirstOrDefaultAsync(m => m.trainingID == id);
            if (training == null)
            {
                return NotFound();
            }
            Training = training;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Training.modifiedBy = User.Identity.Name;
            Training.modifiedDate = DateTime.Now;
            ModelState.Remove("Training.modifiedBy");
            ModelState.Remove("Training.modifiedDate");


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

                return Page();
            }

            _context.Attach(Training).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainingExists(Training.trainingID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TrainingExists(int id)
        {
            return (_context.Trainings?.Any(e => e.trainingID == id)).GetValueOrDefault();
        }
    }
}