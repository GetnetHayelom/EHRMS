using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Family
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? id)
        {
            familyModel = new familyModel();

            if(id != null)
            {
                familyModel.personID = id ?? 0;
            }
            ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName");
            return Page();
        }

        [BindProperty]
        public familyModel familyModel { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
                return RedirectToPage("/Shared/AccessDenied");

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
                bool exists = _context.Families.Any(f => f.personID == familyModel.personID && f.relation == Enums.familyRelation.Father);
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

            // You can add more constraints per familyRelation type

            familyModel.modifiedBy = User.Identity.Name;
            familyModel.modifiedDate = DateTime.Now;

            _context.Families.Add(familyModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }



    }
}
