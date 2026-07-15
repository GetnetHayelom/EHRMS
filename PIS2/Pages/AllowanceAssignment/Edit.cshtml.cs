using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public allowanceAssignmentModel allowanceAssignmentModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER") || !User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var allowanceassignmentmodel =  await _context.AllowanceAssignments
                .Include(aa => aa.employmentModel)
                .Include(aa => aa.allowanceModel).FirstOrDefaultAsync(m => m.allowanceAssignmentID == id);
            if (allowanceassignmentmodel == null)
            {
                return NotFound();
            }
            allowanceAssignmentModel = allowanceassignmentmodel;
            ViewData["allowanceID"] = new SelectList(_context.Allowances, "allowanceID", "allowanceName");
            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER") || !User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            var existing = await _context.AllowanceAssignments.FirstOrDefaultAsync(aa => aa.allowanceAssignmentID == allowanceAssignmentModel.allowanceAssignmentID);

            if(existing == null) { TempData["message"] = ("Error", "Not Found!"); return Page(); }

            if((existing.allowanceStatus == mainStatus.Active || existing.allowanceStatus == mainStatus.Inactive) && !User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Remove("allowanceAssignmentModel.modifiedBy");
            allowanceAssignmentModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid model:");
                
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            _context.Attach(allowanceAssignmentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!allowanceAssignmentModelExists(allowanceAssignmentModel.allowanceAssignmentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = allowanceAssignmentModel.allowanceAssignmentID});
        }

        private bool allowanceAssignmentModelExists(int id)
        {
            return _context.AllowanceAssignments.Include(aa => aa.allowanceModel).Any(e => e.allowanceAssignmentID == id);
        }
    }
}
