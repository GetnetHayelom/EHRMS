using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.JobReqCost
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public jobReqCost JobReqCost { get; set; } = new();

        public SelectList VacancyList { get; set; }

        public void OnGet(int? id)
        {
            VacancyList = new SelectList(
                _context.Vacancies,
                "VacancyID",
                "VacancyTitle", id
            );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("JobReqCost.modifiedBy");
            JobReqCost.modifiedBy = User?.Identity?.Name ?? "System";
            JobReqCost.modifiedDate = DateTime.Now;
            JobReqCost.jobReqStatus = jobReqStatus.Hold;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                 OnGet(null);
                return Page();
            }

            JobReqCost.modifiedBy = User?.Identity?.Name ?? "System";
            JobReqCost.modifiedDate = DateTime.Now;

            _context.JobReqCosts.Add(JobReqCost);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
