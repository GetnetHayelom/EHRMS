using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.TrainingCostAllocation
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        public EditModel(PISContext context) => _context = context;

        [BindProperty]
        public trainingCostAllocationModel Allocation { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Allocation = await _context.TrainingCostAllocations
                .Include(a => a.Attendance)
                .FirstOrDefaultAsync(m => m.trainingCostAllocationID == id);

            if (Allocation == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Allocation.modifiedBy = User.Identity.Name;
            Allocation.modifiedDate = DateTime.UtcNow;

            if (!ModelState.IsValid) return Page();

            _context.Attach(Allocation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    } 
}
