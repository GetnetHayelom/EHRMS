using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.OvertimeHistory
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public overtimeHistoryModel overtimeHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimehistorymodel = await _context.OvertimeHistories.FirstOrDefaultAsync(m => m.overtimeHistoryID == id);
            if (overtimehistorymodel == null)
            {
                return NotFound();
            }
            else
            {
                overtimeHistoryModel = overtimehistorymodel;
            }
            return Page();
        }
    }
}
