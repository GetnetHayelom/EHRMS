using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.OvertimHistory
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public overtimeHistoryModel overtimeHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimehistorymodel =  await _context.OvertimeHistories.FirstOrDefaultAsync(m => m.overtimeHistoryID == id);
            if (overtimehistorymodel == null)
            {
                return NotFound();
            }
            overtimeHistoryModel = overtimehistorymodel;
           ViewData["overtimeRecordID"] = new SelectList(_context.OvertimeRecords, "overtimeRecordID", "overtimeRecordID");
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

            _context.Attach(overtimeHistoryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!overtimeHistoryModelExists(overtimeHistoryModel.overtimeHistoryID))
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

        private bool overtimeHistoryModelExists(int id)
        {
            return _context.OvertimeHistories.Any(e => e.overtimeHistoryID == id);
        }
    }
}
