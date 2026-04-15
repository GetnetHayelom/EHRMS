using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.ShiftAssignment
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<shiftAssignmentModel> shiftAssignmentModel { get;set; } = default!;

        public async Task OnGetAsync(int? id)
        {
            var activeEmps = await _context.Employments.Where(e => e.employmentStatus == mainStatus.Active).Select(j => j.employmentID).ToListAsync();
            if(id != null)
            {
                activeEmps = await _context.JobPlacements.Where(j => j.departmentID == id && j.jobPlacementStatus == mainStatus.Active).Select(j => j.employmentID).ToListAsync();
                shiftAssignmentModel = await _context.ShiftAssignments
                .Include(s => s.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(s => s.shiftModel)
                .Where(s => activeEmps.Contains(s.employmentID))
                .GroupBy(s=> s.employmentID)
                .Select(s => s.OrderByDescending(s =>s.modifiedDate).FirstOrDefault()).ToListAsync();
            }
            else
            {
                shiftAssignmentModel = await _context.ShiftAssignments
                    .Include(s => s.EmploymentModel).ThenInclude(e => e.personModel)
                    .Include(s => s.shiftModel)
                    .Where(e => activeEmps.Contains(e.employmentID))
                    .GroupBy(s => s.employmentID)
                    .Select(s => s.OrderByDescending(s => s.modifiedDate).FirstOrDefault())
                    .ToListAsync();

            }
                
        }
    }
}
