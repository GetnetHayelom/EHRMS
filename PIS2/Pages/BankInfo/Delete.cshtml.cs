using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.BankInfo
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public bankInfoModel bankInfoModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankinfomodel = await _context.BankInfos.FirstOrDefaultAsync(m => m.bankInfoID == id);

            if (bankinfomodel == null)
            {
                return NotFound();
            }
            else
            {
                bankInfoModel = bankinfomodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankinfomodel = await _context.BankInfos.FindAsync(id);
            if (bankinfomodel != null)
            {
                bankInfoModel = bankinfomodel;
                _context.BankInfos.Remove(bankInfoModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
