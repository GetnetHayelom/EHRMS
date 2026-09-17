using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.HR;

namespace PIS2.Pages.Allowance
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<allowanceModel> allowanceModel { get; set; } = default!;

        // === SUMMARY PROPERTIES (for cards) ===
        public int ActiveCount { get; set; }
        public int SuspendedCount { get; set; }
        public int DisabledCount { get; set; }
        public int TotalCount { get; set; }

        public decimal TotalAllowanceAmount { get; set; }
        public int TaxableCount { get; set; }
        public int NonTaxableCount { get; set; }

        public async Task OnGetAsync()
        {
            allowanceModel = await _context.Allowances.ToListAsync();

            TotalCount = allowanceModel.Count;
            ActiveCount = allowanceModel.Count(a => a.allowanceStatus == mainStatus.Active);
            SuspendedCount = allowanceModel.Count(a => a.allowanceStatus == mainStatus.Suspended);
            DisabledCount = allowanceModel.Count(a => a.allowanceStatus == mainStatus.Inactive);

            TotalAllowanceAmount = allowanceModel.Sum(a => a.allowanceAmount);

            TaxableCount = allowanceModel.Count(a => a.allowanceTaxable);
            NonTaxableCount = allowanceModel.Count(a => !a.allowanceTaxable);
        }
    }
}
