using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.WorkSite
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public workSiteModel workSiteModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worksitemodel = await _context.WorkSites
                .Include(w=> w.WorkSiteHistories).ThenInclude(e => e.employmentModel).ThenInclude(e => e.personModel)
                .Include(ws => ws.WorkSiteHistories).ThenInclude( wsh => wsh.addressModel)
                .Include(e => e.employmentModel).ThenInclude(e => e.personModel)
                .Include(ws => ws.addressModel)
                .FirstOrDefaultAsync(m => m.workSiteID == id);
            if (worksitemodel == null)
            {
                return NotFound();
            }
            else
            {
                workSiteModel = worksitemodel;
            }
            return Page();
        }
    }
}
