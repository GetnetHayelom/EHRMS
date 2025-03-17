using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int totalCount { get; set; }
        public int filteredCount { get; set; }

        public async Task OnGetAsync()
        {
            EmploymentTypes = await _context.EmploymentTypes.OrderBy(e => e.employmentTypeName).ToListAsync();
            Departments = await _context.Departments.OrderBy(d => d.departmentName).ToListAsync();
            JobTitles = await _context.Jobs.OrderBy(j => j.jobTitle).ToListAsync();
            WorkLocations = await _context.WorkSites.OrderBy(w => w.workSiteName).ToListAsync();
            Companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
            employmentModel = await _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.departmentModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.jobModel)
                .ToListAsync();
            StartDate = employmentModel.Min(e => e.employmentDate);
            totalCount = employmentModel.Count;            
        }
     
        public IActionResult OnGetFilter(int? department, int? jobTitle, int? empStatus, int? empType, int? company, int? workLoc, DateTime? dateStart, DateTime? dateEnd)
        {
            Console.WriteLine("the Date is " + dateStart);
            // Start with the full list of employees
            var employmentModel = _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.departmentModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.jobModel)
                .AsQueryable(); // Using IQueryable to build a dynamic query
            var departments = _context.Departments.AsQueryable();
            // Apply filters based on the provided query parameters

            // Filter by company (if provided)
            if (company.HasValue && company != null)
            {
                employmentModel = employmentModel
                   .Where(e => e.JobPlacements != null && e.JobPlacements
                   .Any(jp => jp.departmentModel.companyID == company));
                departments = departments.Where(d => d.companyID == company);
                
            }
            // Filter by Department (if provided)
            if (department.HasValue)
            {
                employmentModel = employmentModel
                    .Where(e => e.JobPlacements != null && e.JobPlacements
                    .Any(jp => jp.departmentModel.departmentID == department));
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
            // Filter by type (if provided)
            if (empType.HasValue && empType != null)
            {
                employmentModel = employmentModel
                    .Where(e => e.employmentTypeID == empType);
            }
            
            
            // Filter by workloc (if provided)
            if (workLoc.HasValue && workLoc != null)
            {
                employmentModel = employmentModel
                   .Where(e => e.JobPlacements != null && e.JobPlacements
                   .Any(jp => jp.workSiteID == workLoc));
            }
            // Filter by Start TIme (if provided)
            if (dateStart.HasValue && dateStart != null)
            {
                employmentModel = employmentModel
                    .Where(e => e.employmentDate >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue && dateEnd != null)
            {
                employmentModel = employmentModel
                    .Where(e => e.employmentDate <= dateEnd);
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

            var selectedDeparts = departments.ToList();
            var departs = selectedDeparts.Select(d => new
            {
                id = d.departmentID,
                name = d.departmentName
            }).ToList();
            Console.WriteLine($"Departments Count: {departs.Count}");
            filteredCount = employmentModel.Count();
            // Return the generated HTML
            //return Content(tableHtml);
            return new JsonResult(new { tableHtml, departs });
        }
    }
}
