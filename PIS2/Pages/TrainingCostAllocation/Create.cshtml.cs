using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.TrainingCostAllocation
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        public CreateModel(PISContext context) => _context = context;

        [BindProperty]
        public trainingCostAllocationModel Allocation { get; set; } = default!;
        public SelectList AttendanceList { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Creates a display string like "John Doe - Fire Safety (Jan 2024)"
            var list = _context.TrainingAttendances
                .Include(a => a.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(a => a.TrainingSession)
                    .ThenInclude(s => s.Training)
                .Select(a => new {
                    ID = a.trainingAttendanceID,
                    DisplayName = $"{a.EmploymentModel.personModel.personFullName} - {a.TrainingSession.Training.trainingTitle}"
                });

            AttendanceList = new SelectList(list, "ID", "DisplayName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Allocation.modifiedBy = User.Identity.Name;
            Allocation.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid) return Page();

            _context.TrainingCostAllocations.Add(Allocation);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
