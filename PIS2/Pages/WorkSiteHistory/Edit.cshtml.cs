using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.WorkSiteHistory
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public workSiteHistoryModel workSiteHistoryModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worksitehistorymodel =  await _context.WorkSitesHistories.FirstOrDefaultAsync(m => m.workSiteHistoryID == id);
            if (worksitehistorymodel == null)
            {
                return NotFound();
            }
            workSiteHistoryModel = worksitehistorymodel;
           ViewData["workSiteID"] = new SelectList(_context.WorkSites, "workSiteID", "workSiteName");
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

            _context.Attach(workSiteHistoryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!workSiteHistoryModelExists(workSiteHistoryModel.workSiteHistoryID))
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

        private bool workSiteHistoryModelExists(int id)
        {
            return _context.WorkSitesHistories.Any(e => e.workSiteHistoryID == id);
        }
    }
}
