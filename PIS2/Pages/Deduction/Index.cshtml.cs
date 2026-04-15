using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Deductions
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<deductionType> DeductionTypes { get; set; } = new List<deductionType>();

        public async Task OnGetAsync()
        {
            DeductionTypes = await _context.DeductionTypes.Include(d => d.Account)
                .OrderBy(d => d.deductionPriority)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var deduction = await _context.DeductionTypes.FindAsync(id);
            if (deduction != null)
            {
                _context.DeductionTypes.Remove(deduction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Deduction Type deleted successfully!";
            }
            return RedirectToPage();
        }
    }
}
