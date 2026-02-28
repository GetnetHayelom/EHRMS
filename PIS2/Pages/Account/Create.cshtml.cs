using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.Account
{
   [Authorize(Roles ="MIE\\PMS_HRADMIN")]
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
        public accountModel accountModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            if (!User.IsInRole("MIE\\PMS_FINANCE"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Clear();
            accountModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Accounts.Add(accountModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
