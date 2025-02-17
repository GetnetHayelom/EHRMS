using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignmentHistory
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<allowanceAssignmentHistoryModel> allowanceAssignmentHistoryModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            allowanceAssignmentHistoryModel = await _context.AllowanceAssignmentsHistories
                .Include(a => a.allowanceAssignmentModel).ToListAsync();
        }
    }
}
