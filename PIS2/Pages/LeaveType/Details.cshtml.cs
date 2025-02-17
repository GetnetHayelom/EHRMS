using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.LeaveType
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

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
    }
}
