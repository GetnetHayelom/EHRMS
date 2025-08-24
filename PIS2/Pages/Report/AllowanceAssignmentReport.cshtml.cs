using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Report
{
    public class AllowanceAssignmentReportModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public AllowanceAssignmentReportModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<allowanceAssignmentModel> allowanceAssignmentModel { get;set; } = default!;
        public IList<employmentModel> employmentModel { get; set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<employmentTypeModel> EmploymentTypes { get; set; } = default!;
        public IList<jobModel> JobTitles { get; set; } = default!;
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
            Companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();

            allowanceAssignmentModel = await _context.AllowanceAssignments
                .Include(a => a.allowanceModel)
                .Include(a => a.employmentModel).ThenInclude(e => e.personModel)
                .ToListAsync();
            totalCount=allowanceAssignmentModel.Count();
            }
        
        public IActionResult OnGetFilter(int? department, int? jobTitle, int? allowanceStatus, int? allowanceType, int? company, DateTime? dateStart, DateTime? dateEnd)
        {
            Console.WriteLine("the Date is " + dateStart);
            // Start with the full list of employees
            var allowanceModel = _context.AllowanceAssignments
                .Include(a => a.allowanceModel)
                .Include(a => a.employmentModel).ThenInclude(e => e.personModel)
                .Include(a => a.employmentModel).ThenInclude(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.departmentModel)
                .Include(a => a.employmentModel).ThenInclude(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.jobModel)
                .Select(e => new
                {
                    Employee = e.employmentModel,
                    Department = e.employmentModel.JobPlacements.OrderByDescending(j => j.jobPlacementDate).First().departmentModel,
                    Company = e.employmentModel.JobPlacements.OrderByDescending(j => j.jobPlacementDate).First().departmentModel.companyModel,
                    jobTitle = e.employmentModel.JobPlacements.OrderByDescending(j => j.jobPlacementDate).First().jobModel,
                    person = e.employmentModel.personModel,
                    allowanceType = e.allowanceModel,
                    startDate = e.allowanceAssignmentDate,
                    endDate = e.allowanceAssignmentEndDate,
                    allowanceStatus = e.allowanceStatus,
                    allowanceAmount = e.allowanceAssignmentAmount,
                    modifiedBy =e.modifiedBy
                })
                .AsQueryable(); // Using IQueryable to build a dynamic query
            var departments = _context.Departments.OrderBy(d => d.departmentName).AsQueryable();
            // Apply filters based on the provided query parameters

            // Filter by company (if provided)
            if (company.HasValue && company != null)
            {
                allowanceModel = allowanceModel.Where(e => e.Company.companyID == company);
                departments = _context.Departments.Where(d => d.companyID == company.Value).OrderBy(d => d.departmentName).AsQueryable();
            }
            else
            {
                departments = _context.Departments.OrderBy(d => d.departmentName).AsQueryable();
            }
            // Filter by Department (if provided)
            if (department.HasValue && department != null)
            {
                allowanceModel = allowanceModel.Where(e => e.Department.departmentID == department);

            }

            // Filter by Status (if provided)
            if (allowanceStatus.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.allowanceStatus == (mainStatus)allowanceStatus);

            }

            // Filter by Job Title (if provided)
            if (jobTitle.HasValue)
            {

                allowanceModel = allowanceModel.Where(e => e.jobTitle.jobID == jobTitle);
            }
            // Filter by type (if provided)
            if (allowanceType.HasValue && allowanceType != null)
            {

                allowanceModel = allowanceModel.Where(e => e.allowanceType.allowanceID == allowanceType);
            }


            
            // Filter by Start TIme (if provided)
            if (dateStart.HasValue && dateStart != null)
            {
                allowanceModel = allowanceModel
                    .Where(e => e.startDate >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue && dateEnd != null)
            {
                allowanceModel = allowanceModel
                    .Where(e => e.endDate <= dateEnd);
            }

            // Execute the query and get the filtered results
            var filteredAllowance = allowanceModel.OrderBy(e => e.Employee.givenID).ToList();
            filteredCount = filteredAllowance.Count;
            // Generate the table HTML
            var tableHtml = string.Join("", filteredAllowance.Select(e =>
            {
                var url = Url.Page("Details", new { id = e.Employee.employmentID });


                return $"<tr onclick=\"location.href='{url}'\" style=\"cursor:pointer;\">" +
                        $"<td><a href='/Employment/Details?id={e.Employee.employmentID}' class='text-decoration-none text-dark'>{e.Employee.givenID}</a></td>" +
                        $"<td><a href='/Employment/Details?id={e.Employee.employmentID}' class='text-decoration-none text-dark'>{e.person.personFullName}</a></td>" +
                        $"<td>{e.allowanceType?.allowanceName}</td>" +
                        $"<td>{e.allowanceAmount}</td>" +
                        $"<td>{e.startDate }</td>" +
                        $"<td>{e.modifiedBy ?? "N/A"}</td>";
            }));

            var selectedDeparts = departments.ToList();
            Departments = departments.ToList();
            var departs = selectedDeparts.Select(d => new
            {
                id = d.departmentID,
                name = d.departmentName
            }).ToList();
            Console.WriteLine($"Departments Count: {departs.Count}");
            filteredCount = filteredAllowance.Count();
            // Return the generated HTML
            //return Content(tableHtml);
            return new JsonResult(new { tableHtml, departs, filteredCount });
        }
    }
    
}
