using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Holiday
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public holidayModel holidayModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidaymodel = await _context.Holidays.FirstOrDefaultAsync(m => m.holidayID == id);
            if (holidaymodel == null)
            {
                return NotFound();
            }
            else
            {
                holidayModel = holidaymodel;
            }
            return Page();
        }
    }
}
