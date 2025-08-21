using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Penalty
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public penaltyModel penaltyModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltymodel =  await _context.Penalties.FirstOrDefaultAsync(m => m.penaltyID == id);
            if (penaltymodel == null)
            {
                return NotFound();
            }
            penaltyModel = penaltymodel;
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
           ViewData["penaltyTypeID"] = new SelectList(_context.PenaltyTypes, "penaltyTypeID", "penaltyTypeID");
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

            _context.Attach(penaltyModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!penaltyModelExists(penaltyModel.penaltyID))
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

        private bool penaltyModelExists(int id)
        {
            return _context.Penalties.Any(e => e.penaltyID == id);
        }
    }
}
