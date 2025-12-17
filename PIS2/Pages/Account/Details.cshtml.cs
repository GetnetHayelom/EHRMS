using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Account
{
    [Authorize(Roles = "MIE\\PMS_FINANCE")]
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public accountModel accountModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountmodel = await _context.Accounts.FirstOrDefaultAsync(m => m.accountID == id);
            if (accountmodel == null)
            {
                return NotFound();
            }
            else
            {
                accountModel = accountmodel;
            }
            return Page();
        }
    }
}
