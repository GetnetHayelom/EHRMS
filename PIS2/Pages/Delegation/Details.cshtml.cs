using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.delegation
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public delegationModel delegationModel { get; set; } = default!;
        public List<delegationHistoryModel> DelegationHistory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("HRCLERCK") || User.IsInRole("HRMANAGER") || User.IsInRole("FINANCE"))) { return RedirectToPage("/Shared/AccessDenied"); }
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
                DelegationHistory = await _context.DelegationHistories.Where(d => d.delegationID == id).ToListAsync(); 
            }
            return Page();
        }
    }
}
