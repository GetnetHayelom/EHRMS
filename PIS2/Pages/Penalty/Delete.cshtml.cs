using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Penalty
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public penaltyModel penaltyModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltymodel = await _context.Penalties.FirstOrDefaultAsync(m => m.penaltyID == id);

            if (penaltymodel == null)
            {
                return NotFound();
            }
            else
            {
                penaltyModel = penaltymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltymodel = await _context.Penalties.FindAsync(id);
            if (penaltymodel != null)
            {
                penaltyModel = penaltymodel;
                _context.Penalties.Remove(penaltyModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
