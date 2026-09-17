using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.LeaveType
{
    [Authorize(Roles = "HRADMIN")]
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public leaveTypeModel leaveTypeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavetypemodel = await _context.LeaveTypes.FirstOrDefaultAsync(m => m.leaveTypeID == id);

            if (leavetypemodel == null)
            {
                return NotFound();
            }
            else
            {
                leaveTypeModel = leavetypemodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {

            var leaves =  _context.Leaves.Any(l => l.leaveTypeID == id);
            if (leaves) { TempData["ErrorMessage"] = "Can not delete employment type while there are existing employments with this employment type!"; return Page(); }
            if (id == null)
            {
                return NotFound();
            }

            var leavetypemodel = await _context.LeaveTypes.FindAsync(id);
            if (leavetypemodel != null)
            {
                leaveTypeModel = leavetypemodel;
                _context.LeaveTypes.Remove(leaveTypeModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
