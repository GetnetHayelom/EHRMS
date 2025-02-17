using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Break
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public breakModel breakModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var breakmodel = await _context.Breaks.Include(b=>b.shiftModel).FirstOrDefaultAsync(m => m.breakID == id);
            if (breakmodel == null)
            {
                return NotFound();
            }
            else
            {
                breakModel = breakmodel;
            }
            return Page();
        }
    }
}
