using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Break
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public breakModel breakModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("MIE\\PMS_HRMANAGER") || User.IsInRole("MIE\\PMS_HRCLERK") || User.IsInRole("MIE||PMS_HRADMIN")))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
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
