using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Finance;

namespace PIS2.Pages.SubAccount
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

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
    }
}
