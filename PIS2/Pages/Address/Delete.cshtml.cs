using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Address
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public addressModel addressModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("MIE\\PMS_HRMANAGER")|| User.IsInRole("MIE\\PMS_HRADMIN")))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var addressmodel = await _context.Addresses.FirstOrDefaultAsync(m => m.addressID == id);

            if (addressmodel == null)
            {
                return NotFound();
            }
            else
            {
                addressModel = addressmodel;
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

            var addressmodel = await _context.Addresses.FindAsync(id);
            if (addressmodel != null)
            {
                addressModel = addressmodel;
                _context.Addresses.Remove(addressModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
