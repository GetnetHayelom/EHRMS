using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.EmploymentHistory
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public employmentHistoryModel employmentHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employmenthistorymodel =  await _context.EmploymentHistories.FirstOrDefaultAsync(m => m.employmentHistoryID == id);
            if (employmenthistorymodel == null)
            {
                return NotFound();
            }
            employmentHistoryModel = employmenthistorymodel;
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
           ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
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

            _context.Attach(employmentHistoryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employmentHistoryModelExists(employmentHistoryModel.employmentHistoryID))
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

        private bool employmentHistoryModelExists(int id)
        {
            return _context.EmploymentHistories.Any(e => e.employmentHistoryID == id);
        }
    }
}
