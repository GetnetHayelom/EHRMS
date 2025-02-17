using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.LoyaltyHistory
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public loyaltyHistoryModel loyaltyHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyhistorymodel =  await _context.LoyaltyHistories.FirstOrDefaultAsync(m => m.loyaltyHistoryID == id);
            if (loyaltyhistorymodel == null)
            {
                return NotFound();
            }
            loyaltyHistoryModel = loyaltyhistorymodel;
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
           ViewData["loyaltyID"] = new SelectList(_context.Loyalties, "loyaltyID", "loyaltyName");
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

            _context.Attach(loyaltyHistoryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!loyaltyHistoryModelExists(loyaltyHistoryModel.loyaltyHistoryID))
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

        private bool loyaltyHistoryModelExists(int id)
        {
            return _context.LoyaltyHistories.Any(e => e.loyaltyHistoryID == id);
        }
    }
}
