using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.WorkSiteHistory
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public workSiteHistoryModel workSiteHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worksitehistorymodel = await _context.WorkSitesHistories.FirstOrDefaultAsync(m => m.workSiteHistoryID == id);

            if (worksitehistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                workSiteHistoryModel = worksitehistorymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worksitehistorymodel = await _context.WorkSitesHistories.FindAsync(id);
            if (worksitehistorymodel != null)
            {
                workSiteHistoryModel = worksitehistorymodel;
                _context.WorkSitesHistories.Remove(workSiteHistoryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
