using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Guaranty
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public guarantyModel guarantyModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guarantymodel = await _context.Guaranties.Include(g => g.Employment).FirstOrDefaultAsync(m => m.guarantyID == id);
            if (guarantymodel == null)
            {
                return NotFound();
            }
            else
            {
                guarantyModel = guarantymodel;
            }
            return Page();
        }
    }
}
