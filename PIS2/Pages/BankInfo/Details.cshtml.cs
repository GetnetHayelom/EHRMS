using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.BankInfo
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public bankInfoModel bankInfoModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

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
    }
}
