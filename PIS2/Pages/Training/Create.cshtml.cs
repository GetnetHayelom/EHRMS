using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;

namespace PIS2.Pages.Training
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public trainingModel Training { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Trainings == null || Training == null)
            {
                return Page();
            }

            _context.Trainings.Add(Training);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new {id =Training.trainingID});
        }
    }
}