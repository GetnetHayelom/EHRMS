using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Leave
{
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public leaveModel leaveModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavemodel = await _context.Leaves.Include(l => l.employmentModel).FirstOrDefaultAsync(m => m.leaveID == id);

            if (leavemodel == null)
            {
                return NotFound();
            }
            else
            {
                leaveModel = leavemodel;
                
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (User.IsInRole("HRCLERCK")) return RedirectToPage("/Shared/AccessDenied");

            if (id == null)
            {
                return NotFound();
            }

            var leavemodel = await _context.Leaves.FindAsync(id);
            if (leavemodel != null)
            {
                leaveModel = leavemodel;
                if (leaveModel.leaveStatus != Enums.leaveStatus.Hold) throw new ArgumentException("Only leave on hols status can be deleted!");
                    _context.Leaves.Remove(leaveModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
