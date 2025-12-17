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

            var delegationmodel =  await _context.Delegations.FirstOrDefaultAsync(m => m.delegationID == id);
            if (delegationmodel == null)
            {
                return NotFound();
            }
            delegationModel = delegationmodel;
           ViewData["delegationFrom"] = new SelectList(_context.Employments, "employmentID", "givenID");
           ViewData["delegationTo"] = new SelectList(_context.Employments, "employmentID", "givenID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            ModelState.Remove("delegationModel.modifiedBy");
            delegationModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(delegationModel).State = EntityState.Modified;

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
