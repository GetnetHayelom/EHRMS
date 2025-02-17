using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Department
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public departmentModel departmentModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentmodel =  await _context.Departments.FirstOrDefaultAsync(m => m.departmentID == id);
            if (departmentmodel == null)
            {
                return NotFound();
            }
            departmentModel = departmentmodel;
            ViewData["employmentID"] = new SelectList(
                _context.Employments.OrderBy(e => e.personModel.personFirstName).ThenBy(e=>e.personModel.personFatherName).ThenBy(e=>e.personModel.personLastName)
                .Select(e => new { e.employmentID, FullName = e.personModel.personFullName }),
                "employmentID",
                "FullName"
            );
            ViewData["companyID"] = new SelectList(_context.Companies, "companyID", "companyName");
           ViewData["subAccountID"] = new SelectList(_context.SubAccounts, "subAccountID", "subAccountDescription");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(departmentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!departmentModelExists(departmentModel.departmentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool departmentModelExists(int id)
        {
            return _context.Departments.Any(e => e.departmentID == id);
        }
    }
}
