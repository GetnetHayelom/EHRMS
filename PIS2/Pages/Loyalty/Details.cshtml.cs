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

namespace PIS2.Pages.Loyalty
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public loyaltyModel loyaltyModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltymodel = await _context.Loyalties.FirstOrDefaultAsync(m => m.loyaltyID == id);
            if (loyaltymodel == null)
            {
                return NotFound();
            }
            else
            {
                loyaltyModel = loyaltymodel;
            }
            return Page();
        }
    }
}
