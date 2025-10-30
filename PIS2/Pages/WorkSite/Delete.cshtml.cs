using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.WorkSite
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public workSiteModel workSiteModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worksitemodel = await _context.WorkSites.FirstOrDefaultAsync(m => m.workSiteID == id);

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worksitemodel = await _context.WorkSites.FindAsync(id);
            if (worksitemodel != null)
            {
                workSiteModel = worksitemodel;
                _context.WorkSites.Remove(workSiteModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
