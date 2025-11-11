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
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public prohibitionModel prohibitionModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prohibitionmodel = await _context.Prohibitions
                .Include(p=> p.employmentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.prohibitionID == id);
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
    }
}
