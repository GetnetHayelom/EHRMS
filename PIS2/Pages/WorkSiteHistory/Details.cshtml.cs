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
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

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
    }
}
