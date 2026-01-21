using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.EvaluationType
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public evaluationTypeModel EvaluationType { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            decimal totalWeight = _context.EvaluationTypes
                .Where(x => x.evaluationTypeStatus == mainStatus.Active)
                .Sum(x => x.evaluationTypeWeight);

            if (totalWeight + EvaluationType.evaluationTypeWeight > 100)
            {
                ModelState.AddModelError("", "Total evaluation weight cannot exceed 100%.");
                return Page();
            }

            if (EvaluationType.evaluationTypeName.Trim().ToUpper() == "HR")
            {
                EvaluationType.isFixed = true;
            }

            EvaluationType.modifiedBy = User.Identity.Name;
            EvaluationType.modifiedDate = DateTime.Now;

            _context.EvaluationTypes.Add(EvaluationType);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}


