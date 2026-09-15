using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.Termination
{
    [Authorize(Roles = "HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public EditModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        [BindProperty]
        public terminationModel terminationModel { get; set; } = default!;
        public decimal SeverancePay { get; set; }
        public AnnualLeaveSummary LeaveSummary { get; set; }
        public decimal YearsOfService { get; set; }
        public jobPlacementModel? jobPlacement { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terminationmodel =  await _context.Terminations
                .Include(t => t.EmploymentModel).ThenInclude(e => e.personModel).FirstOrDefaultAsync(m => m.terminationID == id);
            if (terminationmodel == null)
            {
                return NotFound();
            }
            terminationModel = terminationmodel;

            SeverancePay = await _core.GetSeverance(terminationModel.employmentID);
            LeaveSummary = await _context.AnnualLeaveSummary.FirstOrDefaultAsync(a => a.employmentID == terminationModel.employmentID);
            YearsOfService = (decimal)((DateTime.Now - terminationModel.EmploymentModel.employmentDate).TotalDays) / 365.25m;

            jobPlacement = await _context.JobPlacements
                .Include(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Include(jp => jp.jobModel)
                .FirstOrDefaultAsync(j => j.employmentID == terminationModel.employmentID && j.jobPlacementStatus == mainStatus.Active);
            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
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

            _context.Attach(terminationModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!terminationModelExists(terminationModel.terminationID))
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

        private bool terminationModelExists(int id)
        {
            return _context.Terminations.Any(e => e.terminationID == id);
        }
    }
}
