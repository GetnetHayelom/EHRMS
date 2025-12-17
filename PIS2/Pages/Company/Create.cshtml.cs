using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.Company
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
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            ViewData["employmentID"] = new SelectList(
                _context.Employments.OrderBy(e => e.personModel.personFirstName).ThenBy(e => e.personModel.personFatherName).ThenBy(e => e.personModel.personLastName)
                .Select(e => new { e.employmentID, FullName = e.personModel.personFullName }),
                "employmentID",
                "FullName"
            );
            ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");
            return Page();
        }

        [BindProperty]
        public companyModel companyModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            ModelState.Remove("companyModel.modifiedBy");
            companyModel.modifiedBy = User.Identity.Name;

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
            _context.Companies.Add(companyModel);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Company created successfuly!";
            return RedirectToPage("./Create");
        }
    }
}
