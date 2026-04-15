using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Overtime
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public overtimeModel overtimeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimemodel = await _context.Overtimes.FirstOrDefaultAsync(m => m.overtimeID == id);
            if (overtimemodel == null)
            {
                return NotFound();
            }
            else
            {
                overtimeModel = overtimemodel;
            }
            return Page();
        }
    }
}
