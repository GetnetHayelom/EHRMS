using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Termination
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK,MIE\\PMS_MANAGEMENT")]
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public terminationModel terminationModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terminationmodel = await _context.Terminations.Include(e => e.EmploymentModel).FirstOrDefaultAsync(m => m.terminationID == id);
            if (terminationmodel == null)
            {
                return NotFound();
            }
            else
            {
                terminationModel = terminationmodel;
            }
            return Page();
        }
        [BindProperty]
        public int terminationID { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var termination = await _context.Terminations.FindAsync(terminationID);
            if (termination == null)
            {
                return NotFound();
            }

            termination.modifiedBy = User.Identity.Name;
            termination.terminationStatus = terminationStatus.Posted;

            _context.Attach(termination).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return RedirectToPage("./Details", new { id = terminationID });
        }
    }
}
