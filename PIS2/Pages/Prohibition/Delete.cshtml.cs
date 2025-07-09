using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Prohibition
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public prohibitionModel prohibitionModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prohibitionmodel = await _context.Prohibitions.FirstOrDefaultAsync(m => m.prohibitionID == id);

            if (prohibitionmodel == null)
            {
                return NotFound();
            }
            else
            {
                prohibitionModel = prohibitionmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prohibitionmodel = await _context.Prohibitions.FindAsync(id);
            if (prohibitionmodel != null)
            {
                prohibitionModel = prohibitionmodel;
                _context.Prohibitions.Remove(prohibitionModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
