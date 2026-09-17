using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.LeaveHistory
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public leaveHistoryModel leaveHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavehistorymodel = await _context.LeaveHistories.FirstOrDefaultAsync(m => m.leaveHistoryID == id);
            if (leavehistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                leaveHistoryModel = leavehistorymodel;
            }
            return Page();
        }
    }
}
