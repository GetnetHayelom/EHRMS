using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.LeaveHistory
{
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavehistorymodel = await _context.LeaveHistories.FindAsync(id);
            if (leavehistorymodel != null)
            {
                leaveHistoryModel = leavehistorymodel;
                _context.LeaveHistories.Remove(leaveHistoryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
