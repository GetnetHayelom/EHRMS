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
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<bankInfoModel> bankInfoModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            bankInfoModel = await _context.BankInfos
                .Include(b => b.personModel).ToListAsync();
        }
    }
}
