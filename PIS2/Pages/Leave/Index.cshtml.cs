using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PIS2.Models;

namespace PIS2.Pages.Leave
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<leaveModel> leaveModel { get;set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<employmentTypeModel> EmploymentTypes { get; set; } = default!;
        public IList<workSiteModel> WorkLocations { get; set; } = default!;
        public IList<companyModel> Companies { get; set; } = default!;
        public IList<leaveTypeModel> LeaveTypes { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int totalCount { get; set; }
        public int filteredCount { get; set; }
        [BindProperty]
        public double totalUnposted {  get; set; }= default!;
        [BindProperty]
        public double CountUnposted { get; set; } = default!;

        public async Task OnGetAsync()
        {
            EmploymentTypes = await _context.EmploymentTypes.OrderBy(e => e.employmentTypeName).ToListAsync();
            Departments = await _context.Departments.Where(d => d.departmentStatus == mainStatus.Active).OrderBy(d => d.departmentName).ToListAsync();
            WorkLocations = await _context.WorkSites.Where(w => w.workSiteStatus==mainStatus.Active).OrderBy(w => w.workSiteName).ToListAsync();
            Companies = await _context.Companies.Where(c =>c.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToListAsync();
            LeaveTypes = await _context.LeaveTypes.Where(l => l.leaveTypeStatus==mainStatus.Active).OrderBy(lt => lt.leaveTypeName).ToListAsync();

            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel).ThenInclude(e => e.personModel)
                .Include(l => l.leaveTypeModel)
                .Where(l => l.leaveStatus == leaveStatus.Hold || l.leaveStatus == leaveStatus.Approved)
                .Select(e => new
                {

                })
                .ToListAsync();
           
            totalUnposted = leaveModel.Sum(l => l.leaveDays);
            CountUnposted = leaveModel.Count();
        }
        // Post handler
        [BindProperty]
        public int leaveId { get; set; }
        //[HttpPost]
        public async Task<IActionResult> OnPostApprove(int leaveId)
        {
            Console.WriteLine($"Received ID: {leaveId}");
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound();
            }

            // Update the leaveStatus
            leave.leaveStatus = leaveStatus.Posted;
            _context.Update(leave);
            await _context.SaveChangesAsync();
            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel).ToListAsync();
            return Page();
        }
        public IActionResult OnGetFilter(int? department, int? leaveStatus, int? leaveType, int? company, int? workLoc, DateTime? dateStart, DateTime? dateEnd)
        {
            Console.WriteLine("the Date is " + dateStart);
            // Start with the full list of employees
            var leaves = _context.Leaves.Include(l => l.leaveTypeModel)
                .Select(l => new
                {
                    Leave = l,
                    Employee = l.employmentModel,
                    Department = l.employmentModel.JobPlacements.OrderByDescending(j => j.jobPlacementDate).First().departmentModel,
                    Company = l.employmentModel.JobPlacements.OrderByDescending(j => j.jobPlacementDate).First().departmentModel.companyModel,
                    jobTitle = l.employmentModel.JobPlacements.OrderByDescending(j => j.jobPlacementDate).First().jobModel,
                    //WorkSite = _context.SiteAssignments.OrderByDescending(ws => ws.modifiedDate).Where(ws => ws.employmentID == l.employmentModel.employmentID),
                    person = l.employmentModel.personModel,
                    empType = l.employmentModel.employmentTypeModel
                })
                .AsQueryable(); // Using IQueryable to build a dynamic query
            var departments = _context.Departments.OrderBy(d => d.departmentName).AsQueryable();
            // Apply filters based on the provided query parameters

            // Filter by company (if provided)
            if (company.HasValue && company != null)
            {
                leaves = leaves.Where(e => e.Company.companyID == company);

            }
            else
            {
                departments = _context.Departments.OrderBy(d => d.departmentName).AsQueryable();
            }
            // Filter by Department (if provided)
            if (department.HasValue && department != null)
            {

                leaves = leaves.Where(e => e.Department.departmentID == department);
            }

            // Filter by Status (if provided)
            if (leaveStatus.HasValue)
            {

                leaves = leaves.Where(e => e.Leave.leaveStatus == (leaveStatus)leaveStatus);

            }

           
            // Filter by type (if provided)
            if (leaveType.HasValue && leaveType != null)
            {

                leaves = leaves.Where(e => e.Leave.leaveTypeID == leaveType);
            }


            // Filter by workloc (if provided)
            if (workLoc.HasValue && workLoc != null)
            {
                //leaves = leaves.Where(e => e.WorkSite.workSiteID == workLoc);
            }
            // Filter by Start TIme (if provided)
            if (dateStart.HasValue && dateStart != null)
            {
                leaves = leaves.Where(e => e.Leave.leaveStartDate >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue && dateEnd != null)
            {
                leaves = leaves.Where(e => e.Leave.leaveEndDate <= dateEnd);
            }

            // Execute the query and get the filtered results
            var filteredLeaves = leaves.OrderBy(e => e.Leave.leaveReaquestDate).ToList();

            // Generate the table HTML
            var tableHtml = string.Join("", filteredLeaves.Select(e =>
            {

                return $"<tr><td><a href='/Leave/Details?id={e.Leave.leaveID}' class='text-decoration-none text-dark'>{e.Leave.leaveID}</a></td>" +
                        $"<td> <a href='/Employment/Details?id={e.Employee.employmentID}' class='text-decoration-none text-dark'>{e.Employee.givenID}</a></td>" +
                        $"<td> <a href='/Person/Details?id={e.person.personID}' class='text-decoration-none text-dark'>{e.person.personFullName}</a></td>" +
                        $"<td>{e.Leave.leaveReaquestDate.ToShortDateString()}</td>" +
                        $"<td>{e.Leave.leaveStartDate.ToShortDateString()}</td>" +
                        $"<td>{e.Leave.leaveEndDate.ToShortDateString()}</td>" +
                        $"<td>{e.Leave.leaveDays}</td>" +
                        $"<td>{e.Leave.leaveTypeModel.leaveTypeName}</td>" +
                        $"<td><form method='post' asp-page-handler='Approve'>" +
                        $"<input asp-for={e.Leave.leaveID} name='leaveId' type='hidden'></input >" +
                        $"<button type='submit' class='btn btn-success post-button' data-id={e.Leave.leaveID}>Post Leave</button></form></td></tr>";
   
            }));

            
            filteredCount = leaves.Count();
            // Return the generated HTML
            //return Content(tableHtml);
            return new JsonResult(new { tableHtml});
        }
    }
}
