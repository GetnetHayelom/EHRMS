using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.LeaveType
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
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
