using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Holiday
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK, MIE\\PMS_HRADMIN")]
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!(User.IsInRole("MIE\\PMS_HRADMIN") || User.IsInRole("MIE\\PMS_HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }
            if (id == null)
            {
                return NotFound();
            }

            var holidaymodel = await _context.Holidays.FindAsync(id);
            if (holidaymodel != null)
            {
                holidayModel = holidaymodel;
                _context.Holidays.Remove(holidayModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
