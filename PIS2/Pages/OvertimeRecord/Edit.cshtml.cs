using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PIS2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PIS2.Enums;
using PIS2.Models.HR;

namespace PIS2.Pages.OvertimeRecord
{
    
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public overtimeRecordModel overtimeRecordModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimerecordmodel =  await _context.OvertimeRecords.FirstOrDefaultAsync(m => m.overtimeRecordID == id);
            if (overtimerecordmodel == null)
            {
                return NotFound();
            }
            overtimeRecordModel = overtimerecordmodel;
             if(overtimeRecordModel.overtimeRecordStatus != overtimeStatus.Hold && !(User.IsInRole("HRCLERCK") || User.IsInRole("HRMANAGER")))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
               ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
               ViewData["overtimeID"] = new SelectList(_context.Overtimes, "overtimeID", "overtimeName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var ol = HasOverlappingOvertime(overtimeRecordModel.employmentID, overtimeRecordModel.overtimeRecordDate, overtimeRecordModel.overtimeRecordStartTime, overtimeRecordModel.overtimeRecordEndTime);
            if (ol != null)
            {
                TempData["ErrorMessage"] = $"Overlapping overtime record existed: Batch number={ol.overtimeRecordID}" +
                    $" Start={ol.overtimeRecordStartTime}" +
                    $" End={ol.overtimeRecordEndTime}";
                
                return Page();

            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(overtimeRecordModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!overtimeRecordModelExists(overtimeRecordModel.overtimeRecordID))
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

        private bool overtimeRecordModelExists(int id)
        {
            return _context.OvertimeRecords.Any(e => e.overtimeRecordID == id);
        }

        public overtimeRecordModel? HasOverlappingOvertime(
            int employmentID,
            DateTime date,
            TimeSpan newStart,
            TimeSpan newEnd,
            int? currentRecordID = null)
        {
            return _context.OvertimeRecords
                .Where(o => o.employmentID == employmentID &&
                            o.overtimeRecordDate.Date == date.Date &&
                            (currentRecordID == null || o.overtimeRecordID != currentRecordID))
                .FirstOrDefault(o =>
                    o.overtimeRecordStartTime < newEnd &&
                    o.overtimeRecordEndTime > newStart
                );
        }
    }
}
