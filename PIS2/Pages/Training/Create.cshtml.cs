using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Training
{
    [Authorize(Roles="HRPERSONNEL")]
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
            ModelState.Clear();
            Training.modifiedBy = User.Identity.Name;
            Training.modifiedDate = DateTime.Now;
            Training.trainingStatus = Enums.trainingStatus.Planned;

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