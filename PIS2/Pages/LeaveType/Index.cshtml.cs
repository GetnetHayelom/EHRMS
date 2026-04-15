using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.LeaveType
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        // Filters
        [BindProperty(SupportsGet = true)]
        public mainStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public leaveGroup? GroupFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public leaveTypeImpact? ImpactFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? AvailabilityFilter { get; set; }

        public IList<leaveTypeModel> leaveTypeModel { get; set; } = default!;
        public IList<string> leaveAvailabilty { get; set; } = default!;

        // Summary card values
        public int TotalTypes { get; set; }
        public int ActiveTypes { get; set; }
        public int SuspendedTypes { get; set; }
        public int DisabledTypes { get; set; }
        public int AnnualLeaveCount { get; set; }
        public int AllowedLeaveCount { get; set; }
        public int AbsentismCount { get; set; }

        public async Task OnGetAsync()
        {
            leaveAvailabilty = _context.LeaveTypes.Select(l => l.leaveAvailability).Distinct().ToList();
            var data = _context.LeaveTypes.AsQueryable();

            // Apply filters
            if (StatusFilter.HasValue)
                data = data.Where(l => l.leaveTypeStatus == StatusFilter);

            if (GroupFilter.HasValue)
                data = data.Where(l => l.leaveGroup == GroupFilter);

            if (ImpactFilter.HasValue)
                data = data.Where(l => l.leaveTypeImpact == ImpactFilter);

            if (!string.IsNullOrEmpty(AvailabilityFilter))
                data = data.Where(l => l.leaveAvailability == AvailabilityFilter);

            // Load filtered data
            leaveTypeModel = await data.ToListAsync();

            // Summary numbers (unfiltered: total system stats)
            var all = await _context.LeaveTypes.ToListAsync();

            TotalTypes = all.Count;
            ActiveTypes = all.Count(l => l.leaveTypeStatus == mainStatus.Active);
            SuspendedTypes = all.Count(l => l.leaveTypeStatus == mainStatus.Suspended);
            DisabledTypes = all.Count(l => l.leaveTypeStatus == mainStatus.Inactive);

            AnnualLeaveCount = all.Count(l => l.leaveGroup == leaveGroup.AnnualLeave);
            AllowedLeaveCount = all.Count(l => l.leaveGroup == leaveGroup.AllowedLeave);
            AbsentismCount = all.Count(l => l.leaveGroup == leaveGroup.Absentism);
        }
    }
}
