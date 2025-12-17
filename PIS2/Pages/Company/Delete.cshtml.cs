using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Company
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public companyModel companyModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var companymodel = await _context.Companies.FirstOrDefaultAsync(m => m.companyID == id);

            if (companymodel == null)
            {
                return NotFound();
            }
            else
            {
                companyModel = companymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var companymodel = await _context.Companies.FindAsync(id);
            if (companymodel != null)
            {
                companyModel = companymodel;
                _context.Companies.Remove(companyModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
