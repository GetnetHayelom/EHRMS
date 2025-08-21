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
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public addressModel addressModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
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

            try
            {
                _context.Addresses.Add(addressModel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Address saved successfully!";
            }
            catch (DbUpdateException ex)
            {
                if(ex.InnerException !=null && ex.InnerException.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "This address already exists.");
                    return Page();
                }
                throw;
            }
           

            return RedirectToPage("./Index");
        }
    }
}
