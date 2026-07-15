using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.Company
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<companyModel> companyModel { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public mainStatus? CompanyStatus { get; set; }

        public async Task OnGetAsync()
        {
          
            var query = _context.Companies
                .Include(c => c.employmentModel).ThenInclude(e => e.personModel)
                .Include(c => c.addressModel).OrderBy(c => c.companyName).AsQueryable();

            // Apply filter only if selected
            if (CompanyStatus != null)
            {
                query = query.Where(c => c.companyStatus == CompanyStatus);
            }

            companyModel = await query.ToListAsync();
        }
    }
}
