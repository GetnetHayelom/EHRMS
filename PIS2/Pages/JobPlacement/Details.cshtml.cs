using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.JobPlacement
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public jobPlacementModel jobPlacementModel { get; set; } = default!;
        public shiftModel? shiftAssigned { get; set; }
        public List<jobPlacementHistoryModel> jobPlacementHistoryList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           

            var jobplacementmodel = await _context.JobPlacements
                .Include(j => j.jobModel)
                .Include(j => j.jobStepModel).ThenInclude(j => j.jobGradeModel)
                .Include(j => j.JobPlacementHistories)
                .Include(j => j.departmentModel).ThenInclude(j => j.companyModel)
                .Include(j => j.employmentModel)
                .FirstOrDefaultAsync(m => m.jobPlacementID == id);
            if (jobplacementmodel == null)
            {
                return NotFound();
            }
            else
            {

                jobPlacementModel = jobplacementmodel;
                jobPlacementHistoryList =await _context.JobPlacementHistories
                    .Include(jh => jh.departmentModel).Where(jh => jh.jobPlacementID == jobPlacementModel.jobPlacementID).ToListAsync() ?? new List<jobPlacementHistoryModel>();

                shiftAssigned = _context.ShiftAssignments
                    .Where(s => s.employmentID == jobPlacementModel.employmentID)?.OrderBy(s => s.modifiedDate)?.LastOrDefault()?.shiftModel ?? new shiftModel();

            }
            return Page();
        }
        [BindProperty]
        public int jobPlacementID { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var jobPlacement = await _context.JobPlacements.FindAsync(jobPlacementID);
            if (jobPlacement == null)
            {
                return NotFound();
            }

            jobPlacement.modifiedBy = User.Identity.Name;
            jobPlacement.jobPlacementStatus = mainStatus.Active;

            _context.Attach(jobPlacement).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return RedirectToPage("./Details", new { id = jobPlacementID });
        }
    }
}
