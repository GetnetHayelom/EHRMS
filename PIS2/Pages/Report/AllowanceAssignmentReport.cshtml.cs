using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
{
    public class AllowanceAssignmentReportModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public AllowanceAssignmentReportModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<allowanceAssignmentModel> allowanceAssignmentModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            allowanceAssignmentModel = await _context.AllowanceAssignments
                .Include(a => a.allowanceModel)
                .Include(a => a.employmentModel).ThenInclude(e => e.personModel)
                .ToListAsync();
        }
    }
}
