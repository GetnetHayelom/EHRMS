using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Prohibition
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public prohibitionModel prohibitionModel { get; set; } = default!;
        public List<prohibitionHistoryModel>? prohibitionHistory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prohibitionmodel =  await _context.Prohibitions
                .Include(p => p.employmentModel).ThenInclude(e => e.personModel).FirstOrDefaultAsync(m => m.prohibitionID == id);
            if (prohibitionmodel == null)
            {
                return NotFound();
            }
            prohibitionHistory = _context.prohibitionHistories.Where(p => p.prohibitionID == prohibitionmodel.prohibitionID).OrderBy(p => p.modifiedDate).ToList();
            prohibitionModel = prohibitionmodel;
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) return RedirectToPage("/Shared/AccessDenied");
            var prob = await _context.Prohibitions.FindAsync(prohibitionModel.prohibitionID);
            if (prob == null)
                return NotFound();

            prob.prohibitionStart = prohibitionModel.prohibitionStart;
            prob.prohibitionEnd = prohibitionModel.prohibitionEnd;
            prob.prohibitionStatus = prohibitionModel.prohibitionStatus;
            prob.prohibitionReason = prohibitionModel.prohibitionReason;
            prob.prohibitionType = prohibitionModel.prohibitionType;
            prob.prohibitionRemark = prohibitionModel.prohibitionRemark;
            prob.modifiedBy = User.Identity.Name;

            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!prohibitionModelExists(prohibitionModel.prohibitionID))
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

        private bool prohibitionModelExists(int id)
        {
            return _context.Prohibitions.Any(e => e.prohibitionID == id);
        }
    }
}
