using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.EmploymentType
{
    
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public employmentTypeModel employmentTypeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var employmenttypemodel = await _context.EmploymentTypes.FirstOrDefaultAsync(m => m.employmentTypeID == id);

            if (employmenttypemodel == null)
            {
                return NotFound();
            }
            else
            {
                employmentTypeModel = employmenttypemodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!User.IsInRole("HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            var emps = _context.Employments.Any(e => e.employmentID == id);

            if (emps) { TempData["ErrorMessage"] = "Can not delete employment type while there are existing employments with this employment type!"; return Page(); }

            if (id == null)
            {
                return NotFound();
            }

            var employmenttypemodel = await _context.EmploymentTypes.FindAsync(id);
            if (employmenttypemodel != null)
            {
                employmentTypeModel = employmenttypemodel;
                _context.EmploymentTypes.Remove(employmentTypeModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
