using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.TrainingSessionAttendance
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        public DetailsModel(PISContext context) => _context = context;

        public trainingAttendanceModel Attendance { get; set; } = default!;

        public List<trainingCostAllocationModel> Costs { get; set; } = default!;

        [BindProperty]
        public trainingCostAllocationModel NewAllocation { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Attendance = await _context.TrainingAttendances
                .Include(a => a.TrainingSession).ThenInclude(s => s.Training)
                .Include(a => a.EmploymentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.trainingAttendanceID == id);

            if (Attendance == null) return NotFound();

            Costs = await _context.TrainingCostAllocations.Where(t => t.trainingAttendanceID == id).ToListAsync() ?? new List<trainingCostAllocationModel>();
            return Page();
        }

        public async Task<IActionResult> OnPostAddAllocationAsync(int id)
        {

            // Clean up validation for navigation properties
            ModelState.Clear();
            NewAllocation.modifiedBy = User.Identity.Name;
            NewAllocation.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                await OnGetAsync(id);
                return Page();
            }

            _context.TrainingCostAllocations.Add(NewAllocation);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = NewAllocation.trainingAttendanceID });
        }
    }
}
