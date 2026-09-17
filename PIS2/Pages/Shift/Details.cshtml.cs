using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.Shift
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public shiftModel shiftModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftmodel = await _context.Shifts.FirstOrDefaultAsync(m => m.shiftID == id);
            if (shiftmodel == null)
            {
                return NotFound();
            }
            else
            {
                shiftModel = shiftmodel;
            }
            return Page();
        }
    }
}
