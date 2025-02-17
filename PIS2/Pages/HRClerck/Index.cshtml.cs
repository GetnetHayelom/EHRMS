using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.HRClerck
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<personModel> persons { get;set; } = default!;
        public IList<employmentModel> employees { get;set; } = default!;
        public IList<leaveModel> leaves { get; set; } = default!;
        public IList<jobPlacementModel> jobPlacements { get; set; } = default!;
        public IList<loyaltyModel> loyalties { get; set; } = default!;
        public IList<overtimeRecordModel> overtimeRecords { get; set; } = default!;
        public IList<allowanceAssignmentModel> allowances { get; set; } = default!;

        public async Task OnGetAsync()
        {
            persons = await _context.Persons
                .Include(p => p.addressModel).ToListAsync();
            employees = await _context.Employments
                .Include(p => p.JobPlacements)
                .Include(p => p.EmploymentHistories)
                .Include(p => p.Leaves)
                .Include(p => p.AllowanceAssignments)
                .Include(p => p.LoyaltyHistories)
                .Include(p => p.OvertimeRecords)
                .ToListAsync();
            leaves = await _context.Leaves
               .Include(p => p.leaveTypeModel).ToListAsync();
            jobPlacements = await _context.JobPlacements
               .Include(p => p.jobModel).ThenInclude(j=> j.jobClassModel)
               .Include(p => p.jobModel).ThenInclude(j => j.jobCategoryModel)
               .Include(p => p.jobModel).ThenInclude(j => j.jobGradeModel)
               .ToListAsync();
            loyalties = await _context.Loyalties.ToListAsync();
            overtimeRecords = await _context.OvertimeRecords.ToListAsync();
            allowances = await _context.AllowanceAssignments.ToListAsync();
        }
    }
}
