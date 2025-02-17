using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.LoyaltyHistory
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public loyaltyHistoryModel loyaltyHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyhistorymodel = await _context.LoyaltyHistories.FirstOrDefaultAsync(m => m.loyaltyHistoryID == id);
            if (loyaltyhistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                loyaltyHistoryModel = loyaltyhistorymodel;
            }
            return Page();
        }
    }
}
