using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Family
{
    [Authorize(Roles = "HRPERSONNEL")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public familyModel familyModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familymodel =  await _context.Families.FirstOrDefaultAsync(m => m.familyID == id);
            if (familymodel == null)
            {
                return NotFound();
            }
            familyModel = familymodel;
           ViewData["personID"] = new SelectList(_context.Persons, "personID", "personID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRPERSONNEL"))
                return RedirectToPage("/Shared/AccessDenied");

            var famrel = _context.Families.FirstOrDefault(f => f.familyID == familyModel.familyID);

            ModelState.Clear();
            familyModel.modifiedBy = User.Identity.Name;
            familyModel.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return Page();
            }
            

            // Ensure hidden inputs are valid integers
            if (familyModel.personID == 0 || familyModel.personID2 == 0)
            {
                ModelState.AddModelError("", "Please select valid Person and Relative from the list.");
                return Page();
            }

            if (familyModel.personID == familyModel.personID2)
            {
                ModelState.AddModelError("", "Person and Relative cannot be the same.");
                return Page();
            }

            // Enforce relation constraints
            if (familyModel.relation == Enums.familyRelation.Father)
            {
                bool exists = _context.Families.Any(f => (f.personID == familyModel.personID || f.personID2 ==familyModel.personID) && f.relation == Enums.familyRelation.Father);
                if (exists)
                {
                    ModelState.AddModelError("", "This person already has a father assigned.");
                    return Page();
                }

            }

            if (familyModel.relation == Enums.familyRelation.Mother)
            {
                bool exists = _context.Families.Any(f => f.personID == familyModel.personID && f.relation == Enums.familyRelation.Mother);
                if (exists)
                {
                    ModelState.AddModelError("", "This person already has a mother assigned.");
                    return Page();
                }
            }

            _context.Attach(familyModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!familyModelExists(familyModel.familyID))
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

        private bool familyModelExists(int id)
        {
            return _context.Families.Any(e => e.familyID == id);
        }
    }
}
