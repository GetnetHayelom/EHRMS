using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Address
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public addressModel addressModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("MIE\\PMS_HRMANAGER") || User.IsInRole("MIE\\PMS_HRADMIN")))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var addressmodel =  await _context.Addresses.FirstOrDefaultAsync(m => m.addressID == id);
            if (addressmodel == null)
            {
                return NotFound();
            }
            addressModel = addressmodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.IsInRole("MIE\\PMS_HRMANAGER") || User.IsInRole("MIE\\PMS_HRADMIN")))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Remove("addressModel.modifiedBy");
            addressModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return Page();
            }

            _context.Attach(addressModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Address updated successfully!";
            }
            catch (DbUpdateException ex)
            {
                if (!addressModelExists(addressModel.addressID))
                {
                    return NotFound();
                }
                else if(ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "This address already exists.");
                    return Page();
                }                
                    throw;                
            }
            return Page();
            //return RedirectToPage("./Index");
        }

        private bool addressModelExists(int id)
        {
            return _context.Addresses.Any(e => e.addressID == id);
        }
    }
}
