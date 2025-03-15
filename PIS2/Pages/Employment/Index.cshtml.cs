using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;

namespace PIS2.Pages.Employment
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<employmentModel> employmentModel { get;set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<employmentTypeModel> EmploymentTypes { get; set; } = default!;
        public IList<jobModel> JobTitles { get; set; } = default!;
        public IList<workSiteModel> WorkLocations { get; set; } = default!;
        public IList<companyModel> Companies { get; set; } = default!;
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int totalCount { get; set; }
        public int filteredCount { get; set; }

        public async Task OnGetAsync()
        {
            EmploymentTypes = await _context.EmploymentTypes.ToListAsync();
            Departments = await _context.Departments.ToListAsync();
            JobTitles = await _context.Jobs.ToListAsync();
            WorkLocations = await _context.WorkSites.ToListAsync();
            Companies = await _context.Companies.ToListAsync();
            employmentModel = await _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.departmentModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.jobModel)
                .ToListAsync();

            totalCount = employmentModel.Count;            
        }
     
        public IActionResult OnGetFilter(int? department, int? jobTitle, int? empStatus, int? empType, int? company, int? workLoc, DateTime? dateStart, DateTime? dateEnd)
        {
            // Start with the full list of employees
            var employmentModel = _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.departmentModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.jobModel)
                .AsQueryable(); // Using IQueryable to build a dynamic query

            // Apply filters based on the provided query parameters

            // Filter by Department (if provided)
            if (department.HasValue)
            {
                employmentModel = employmentModel
                    .Where(e => e.JobPlacements != null && e.JobPlacements
                    .Any(jp => jp.departmentModel.departmentID==department));
            }

            // Filter by Status (if provided)
            if (empStatus.HasValue)
            {
                //if (Enum.TryParse(empStatus, out mainStatus statusEnum))
                //{
                    employmentModel = employmentModel.Where(e => e.employmentStatus == (mainStatus) empStatus);
                //}
            }

            // Filter by Job Title (if provided)
            if (jobTitle.HasValue)
            {
                employmentModel = employmentModel
                    .Where(e => e.JobPlacements != null && e.JobPlacements
                    .Any(jp => jp.jobModel.jobID==jobTitle));
            }
            // Filter by Job Title (if provided)
            if (empType.HasValue && empType != null)
            {
                employmentModel = employmentModel
                    .Where(e => e.employmentTypeID == empType);
            }

            // Execute the query and get the filtered results
            var filteredEmployees = employmentModel.ToList();

            // Generate the table HTML
            var tableHtml = string.Join("", filteredEmployees.Select(e =>
            {
                var firstJobPlacement = e.JobPlacements.FirstOrDefault();
                return $"<tr><td>{e.givenID}</td>" +
                        $"<td>{e.personModel.personFullName}</td>" +
                        $"<td>{e.employmentDate.ToShortDateString()}</td>" +
                        $"<td>{e.employmentStatus}</td>" +
                        $"<td>{e.employmentTypeModel.employmentTypeName}</td>" +
                        $"<td>{firstJobPlacement?.jobModel?.jobTitle ?? "N/A"}</td>" +
                        $"<td>{firstJobPlacement?.departmentModel?.departmentName ?? "N/A"}</td>" +
                        $"<td><a href='/Employment/Edit?id={e.employmentID}'>Edit</a> |" +
                        $"<a href='/Employment/Details?id={e.employmentID}'>Details</a></td></tr>";
            }));
            filteredCount= employmentModel.Count();
            // Return the generated HTML
            return Content(tableHtml);
        }
    }
}
