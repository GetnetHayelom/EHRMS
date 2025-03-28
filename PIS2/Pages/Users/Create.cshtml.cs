using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        ViewData["personID"] = new SelectList(_context.Persons.OrderBy(p => p.personFirstName).ThenBy(p => p.personFatherName), "personID", "personFullName");
            return Page();
        }

        [BindProperty]
        public userModel userModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int personID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            userModel.personID = personID;
            userModel.modifiedBy = @User.Identity.Name;
            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
