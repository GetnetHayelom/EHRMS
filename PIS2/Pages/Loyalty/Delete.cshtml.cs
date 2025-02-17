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
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltymodel = await _context.Loyalties.FindAsync(id);
            if (loyaltymodel != null)
            {
                loyaltyModel = loyaltymodel;
                _context.Loyalties.Remove(loyaltyModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
