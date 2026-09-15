using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.SiteAssignment
{
    [Authorize(Roles = "HRPERSONNEL")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
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
