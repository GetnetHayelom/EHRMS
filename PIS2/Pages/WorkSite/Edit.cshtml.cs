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

namespace PIS2.Pages.WorkSite
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public workSiteModel workSiteModel { get; set; } = default!;
        public List<workSiteHistoryModel> workSiteHistoryModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var worksitemodel =  await _context.WorkSites.Include(ws => ws.WorkSiteHistories).FirstOrDefaultAsync(m => m.workSiteID == id);
            if (worksitemodel == null)
            {
                return NotFound();
            }
            workSiteModel = worksitemodel;
           ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            workSiteModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(workSiteModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!workSiteModelExists(workSiteModel.workSiteID))
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

        private bool workSiteModelExists(int id)
        {
            return _context.WorkSites.Any(e => e.workSiteID == id);
        }
    }
}
