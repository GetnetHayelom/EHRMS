using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Account
{
    [Authorize(Roles = "HRADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public accountModel accountModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("FINANCE"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var accountmodel =  await _context.Accounts.FirstOrDefaultAsync(m => m.accountID == id);
            if (accountmodel == null)
            {
                return NotFound();
            }
            accountModel = accountmodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("FINANCE"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Clear();
            accountModel.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(accountModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!accountModelExists(accountModel.accountID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool accountModelExists(int id)
        {
            return _context.Accounts.Any(e => e.accountID == id);
        }
    }
}
