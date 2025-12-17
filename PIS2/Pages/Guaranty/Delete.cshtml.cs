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
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public guarantyModel guarantyModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guarantymodel = await _context.Guaranties.FirstOrDefaultAsync(m => m.guarantyID == id);

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var guarantymodel = await _context.Guaranties.FindAsync(id);
            if (guarantymodel != null)
            {
                guarantyModel = guarantymodel;
                _context.Guaranties.Remove(guarantyModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
