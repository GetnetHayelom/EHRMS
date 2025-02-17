using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.SubAccount
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public subAccountModel subAccountModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subaccountmodel = await _context.SubAccounts.FirstOrDefaultAsync(m => m.subAccountID == id);

            if (subaccountmodel == null)
            {
                return NotFound();
            }
            else
            {
                subAccountModel = subaccountmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subaccountmodel = await _context.SubAccounts.FindAsync(id);
            if (subaccountmodel != null)
            {
                subAccountModel = subaccountmodel;
                _context.SubAccounts.Remove(subAccountModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
