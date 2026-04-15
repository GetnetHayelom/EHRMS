using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.WorkSite
{
    [Authorize(Roles = "mie\\PMS_HRMANAGER")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<workSiteModel> workSiteModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            workSiteModel = await _context.WorkSites
                .Include(w => w.addressModel).OrderBy(ws => ws.workSiteName).ToListAsync();
        }
    }
}
