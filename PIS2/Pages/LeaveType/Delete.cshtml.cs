using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.LeaveType
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
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
