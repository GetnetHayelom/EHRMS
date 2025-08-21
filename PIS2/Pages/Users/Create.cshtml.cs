using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;




namespace PIS2.Pages.Users
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
        ViewData["personID"] = new SelectList(_context.Persons
            .OrderBy(p => p.personFirstName).ThenBy(p => p.personFatherName).ThenBy(p => p.personLastName)
            .Where(p => !_context.Users.Any(u => u.personID == p.personID)), "personID", "personFullName");

            return Page();
        }

        [BindProperty]
        public userModel userModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            userModel.modifiedBy = User.Identity.Name!;
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

            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
