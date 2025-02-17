using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Allowance
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public allowanceModel allowanceModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allowancemodel = await _context.Allowances.FirstOrDefaultAsync(m => m.allowanceID == id);

            if (allowancemodel == null)
            {
                return NotFound();
            }
            else
            {
                allowanceModel = allowancemodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allowancemodel = await _context.Allowances.FindAsync(id);
            if (allowancemodel != null)
            {
                allowanceModel = allowancemodel;
                _context.Allowances.Remove(allowanceModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
