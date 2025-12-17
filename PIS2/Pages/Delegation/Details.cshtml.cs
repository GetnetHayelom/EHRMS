using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.delegation
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public delegationModel delegationModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("MIE\\PMS_HRCLERCK") || User.IsInRole("MIE\\PMS_HRMANAGER") || User.IsInRole("MIE\\PMS_FINANCE"))) { return RedirectToPage("/Shared/AccessDenied"); }
            if (id == null)
            {
                return NotFound();
            }

            var delegationmodel = await _context.Delegations
                .Include(d => d.FromEmployment).ThenInclude(e => e.personModel)
                .Include(d => d.ToEmployment).ThenInclude(e => e.personModel).FirstOrDefaultAsync(m => m.delegationID == id);
            if (delegationmodel == null)
            {
                return NotFound();
            }
            else
            {
                delegationModel = delegationmodel;
            }
            return Page();
        }
    }
}
