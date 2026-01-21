using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Absentism
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public leaveModel leaveModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("MIE\\PMS_HRMANAGER") || User.IsInRole("MIE\\PMS_HRCLERK")))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var leavemodel =  await _context.Leaves.FirstOrDefaultAsync(m => m.leaveID == id);
            if (leavemodel == null)
            {
                return NotFound();
            }
            leaveModel = leavemodel;
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
           ViewData["leaveTypeID"] = new SelectList(_context.LeaveTypes, "leaveTypeID", "leaveTypeName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.IsInRole("MIE\\PMS_HRMANAGER")|| User.IsInRole("MIE\\PMS_HRCLERK")))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if(leaveModel.leaveStatus == leaveStatus.Completed) { return Page(); }
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return Page();
            }

            _context.Attach(leaveModel).State = EntityState.Modified;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!leaveModelExists(leaveModel.leaveID))
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

        private bool leaveModelExists(int id)
        {
            return _context.Leaves.Any(e => e.leaveID == id);
        }
    }
}
