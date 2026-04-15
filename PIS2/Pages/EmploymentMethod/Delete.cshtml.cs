using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.EmploymentMethod
{
    
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public employmentMethodModel employmentMethodModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employmentmethodmodel = await _context.EmploymentMethods.FirstOrDefaultAsync(m => m.employmentMethodID == id);

            if (employmentmethodmodel == null)
            {
                return NotFound();
            }
            else
            {
                employmentMethodModel = employmentmethodmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var employmentmethodmodel = await _context.EmploymentMethods.FindAsync(id);
            if (employmentmethodmodel != null)
            {
                employmentMethodModel = employmentmethodmodel;
                _context.EmploymentMethods.Remove(employmentMethodModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
