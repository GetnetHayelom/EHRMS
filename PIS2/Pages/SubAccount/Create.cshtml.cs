using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.SubAccount
{
    [Authorize(Roles = "MIE\\PMS_FINANCE")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["accountID"] = new SelectList(_context.Accounts, "accountID", "accountNumber");
            return Page();
        }

        [BindProperty]
        public subAccountModel subAccountModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_FINANCE"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Clear();
            subAccountModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SubAccounts.Add(subAccountModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
