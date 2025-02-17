using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.EmploymentHistory
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public employmentHistoryModel employmentHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employmenthistorymodel = await _context.EmploymentHistories.FirstOrDefaultAsync(m => m.employmentHistoryID == id);

            if (employmenthistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                employmentHistoryModel = employmenthistorymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employmenthistorymodel = await _context.EmploymentHistories.FindAsync(id);
            if (employmenthistorymodel != null)
            {
                employmentHistoryModel = employmenthistorymodel;
                _context.EmploymentHistories.Remove(employmentHistoryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
