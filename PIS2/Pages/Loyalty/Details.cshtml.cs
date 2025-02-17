using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Loyalty
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
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
