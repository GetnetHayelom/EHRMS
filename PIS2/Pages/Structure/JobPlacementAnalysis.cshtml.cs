using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Services;
using PIS2.Views;

namespace PIS2.Pages.Structure
{
    public class JobPlacementAnalaysisModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public JobPlacementAnalaysisModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
        public JobPlacementAnalysisView Analysis { get; set; }= new JobPlacementAnalysisView
        {
            UnderStaffed = new List<PlacementGap>(),
            OverStaffed = new List<PlacementGap>(),
            Unstructured = new List<UnstructuredPlacement>()
        };
        public async Task OnGetAsync()
        {
            // 1. Get all formal structures
            var structures = await _context.Structures
                .Include(s => s.departmentModel)
                .Include(s => s.jobModel)
                .Where(s => s.structureStatus == mainStatus.Active)
                .ToListAsync();

            // 2. Get all active placements
            var activePlacements = await _context.JobPlacements
                .Include(p => p.employmentModel).ThenInclude(e => e.personModel)
                .Include(p => p.departmentModel).ThenInclude(d => d.companyModel)
                .Include(p => p.jobModel)
                .Where(p => p.jobPlacementStatus == mainStatus.Active) // Assuming "Active" status exists
                .ToListAsync();

            var structs = await _context.JobGapAnalysisView.ToListAsync();
            // 3. Analyze Gaps (Structure vs Placements)
            foreach (var structItem in structs)
            {
                if (structItem == null) continue;
                var actualCount = activePlacements.Count(p =>
                    p != null &&
                    p.departmentID == structItem.DeptID &&
                    p.jobID == structItem.JobID);

                var gap = new PlacementGap
                {
                    CompanyName = structItem.CompanyName ?? "Unknown",
                    DepartmentName = structItem.DepartmentName ?? "Unknown",
                    JobTitle = structItem.JobTitle ?? "Unknown",
                    Required = structItem.TargetCount,
                    Actual = actualCount
                };

                if (actualCount < structItem.TargetCount)
                    Analysis.UnderStaffed.Add(gap);
                else if (actualCount > structItem.TargetCount)
                    Analysis.OverStaffed.Add(gap);
            }

            // 4. Identify Unstructured Placements
            // (Placements that don't have a matching entry in the Structure table)
            var unstructured = activePlacements.Where(p => !structures.Any(s =>
                s.departmentID == p.departmentID &&
                s.jobID == p.jobID));

            foreach (var p in unstructured)
            {
                Analysis.Unstructured.Add(new UnstructuredPlacement
                {
                    EmploymentID= p.employmentID,
                    JobPlacementID = p.jobPlacementID,
                    GivenID =p.employmentModel?.givenID ?? "",
                    EmployeeName = p.employmentModel?.personModel?.personFullName ?? "", // Adjust based on your employmentModel
                    DepartmentName = p.departmentModel?.departmentName ?? "",
                    JobTitle = p.jobModel?.jobTitle ?? "",
                    CompanyName = p.departmentModel?.companyModel?.companyName ?? ""
                });
            }
        }
    }
}
