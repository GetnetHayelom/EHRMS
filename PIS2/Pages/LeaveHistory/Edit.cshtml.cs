using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.LeaveHistory
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public leaveHistoryModel leaveHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavehistorymodel =  await _context.LeaveHistories.FirstOrDefaultAsync(m => m.leaveHistoryID == id);
            if (leavehistorymodel == null)
            {
                return NotFound();
            }
            leaveHistoryModel = leavehistorymodel;
           ViewData["leaveID"] = new SelectList(_context.Leaves, "leaveID", "emplymentModel.givenID");
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

            _context.Attach(leaveHistoryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!leaveHistoryModelExists(leaveHistoryModel.leaveHistoryID))
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

        private bool leaveHistoryModelExists(int id)
        {
            return _context.LeaveHistories.Any(e => e.leaveHistoryID == id);
        }
    }
}
