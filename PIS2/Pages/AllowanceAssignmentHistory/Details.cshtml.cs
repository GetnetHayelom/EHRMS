using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignmentHistory
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public allowanceAssignmentHistoryModel allowanceAssignmentHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmenthistorymodel = await _context.AllowanceAssignmentsHistories.FirstOrDefaultAsync(m => m.allowanceAssignmentHistoryID == id);
            if (allowanceassignmenthistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                allowanceAssignmentHistoryModel = allowanceassignmenthistorymodel;
            }
            return Page();
        }
    }
}
