using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.SiteAssignment
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public siteAssignmentModel siteAssignmentModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteassignmentmodel = await _context.SiteAssignments.FirstOrDefaultAsync(m => m.siteAssignmentID == id);
            if (siteassignmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                siteAssignmentModel = siteassignmentmodel;
            }
            return Page();
        }
    }
}
