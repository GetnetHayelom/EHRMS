using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Account
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER, MIE\\PMS_FINANCE, MIE\\PMS_HRADMIN")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<accountModel> accountModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            accountModel = await _context.Accounts.ToListAsync();
        }
    }
}
