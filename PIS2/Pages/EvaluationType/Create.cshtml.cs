using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
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
        public SelectList JobClasses { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var jobClasses = await _context.JobClasses.Where(j => j.jobClassStatus == mainStatus.Active).ToListAsync();
            JobClasses = new SelectList(jobClasses, "jobClassId", "jobClassName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EvaluationType.modifiedBy = User.Identity.Name;
            EvaluationType.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
                return Page();

            //decimal totalWeight =await _context.EvaluationTypes
            //    .Where(x => x.evaluationTypeStatus == mainStatus.Active)
            //    .Sum(x => x.evaluationTypeWeight);

            //if (totalWeight + EvaluationType.evaluationTypeWeight > 100)
            //{
            //    ModelState.AddModelError("", "Total evaluation weight cannot exceed 100%.");
            //    return Page();
            //}

            //if (EvaluationType.evaluationTypeName.Trim().ToUpper() == "HR")
            //{
            //    EvaluationType.isFixed = true;
            //}

            EvaluationType.modifiedBy = User.Identity.Name;
            EvaluationType.modifiedDate = DateTime.Now;

            _context.EvaluationTypes.Add(EvaluationType);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}


