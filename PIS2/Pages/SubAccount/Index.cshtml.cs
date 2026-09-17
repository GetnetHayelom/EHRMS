using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Finance;

namespace PIS2.Pages.SubAccount
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<subAccountModel> subAccountModel { get;set; } = default!;
        [BindProperty(SupportsGet =true)]
        public mainStatus? SubAccountStatus { get; set; }
        public async Task OnGetAsync()
        {
            var subs = _context.SubAccounts
                .Include(s => s.accountModel).AsQueryable();
              if(SubAccountStatus != null)
            {
                subs = subs.Where(s => s.subAccountStatus == SubAccountStatus);
            }

            subAccountModel = await subs.ToListAsync();
        }
    }
}
