using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIS2.Pages.TaxRate
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<taxRateModel> TaxRates { get; set; }

        public async Task OnGetAsync()
        {
            TaxRates = await _context.TaxRates
                .OrderBy(t => t.amount)
                .ToListAsync();
        }
    }
}
