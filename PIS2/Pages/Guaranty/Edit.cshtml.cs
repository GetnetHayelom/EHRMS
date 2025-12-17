using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Guaranty
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public EditModel(PIS2.Models.PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        [BindProperty]
        public guarantyModel guarantyModel { get; set; } = default!;

        public employmentModel employmentModel { get; set; }

        public bool isSelf { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guarantymodel =  await _context.Guaranties.FirstOrDefaultAsync(m => m.guarantyID == id);
            if (guarantymodel == null)
            {
                return NotFound();
            }
            guarantyModel = guarantymodel;
            employmentModel = _context.Employments.Include(e => e.personModel).FirstOrDefault(e => e.employmentID == guarantyModel.employmentID);
            isSelf = _core.IsSelf(User.Identity.Name, employmentModel?.employmentID);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            
            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            var guaranty = _context.Guaranties.FirstOrDefault(g => g.guarantyID == guarantyModel.guarantyID);

            if(guaranty == guarantyModel)
            {
                return Page();
            }

            guaranty.guarantyStatus = guarantyModel.guarantyStatus;
            guaranty.guarantyStartDate = guarantyModel.guarantyStartDate;
            guaranty.guarantyEndDate = guarantyModel.guarantyEndDate;
            guaranty.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(guaranty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!guarantyModelExists(guarantyModel.guarantyID))
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

        private bool guarantyModelExists(int id)
        {
            return _context.Guaranties.Any(e => e.guarantyID == id);
        }
    }
}
