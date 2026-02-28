using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.delegation
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public delegationModel delegationModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("MIE\\PMS_MANAGEMENT") || User.IsInRole("MIE\\PMS_HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }

            if (id == null)
            {
                return NotFound();
            }

            var delegationmodel =  await _context.Delegations
                .Include(d => d.FromEmployment).ThenInclude(e => e.personModel)
                .Include(d => d.ToEmployment).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.delegationID == id);
            if (delegationmodel == null)
            {
                return NotFound();
            }
            delegationModel = delegationmodel;
          
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
            var existing = await _context.Delegations.FirstOrDefaultAsync(d => d.delegationID == delegationModel.delegationID);

            if (existing == null) { return NotFound(); }

            ModelState.Remove("delegationModel.modifiedBy");

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            existing.delegationStatus = delegationModel.delegationStatus;
            existing.modifiedBy = User.Identity.Name;
            existing.delegationStartDate = existing.delegationStartDate > DateTime.Now ? delegationModel.delegationStartDate :existing.delegationStartDate;
            existing.delegationEndDate = delegationModel.delegationEndDate;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!delegationModelExists(delegationModel.delegationID))
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

        private bool delegationModelExists(int id)
        {
            return _context.Delegations.Any(e => e.delegationID == id);
        }
    }
}
