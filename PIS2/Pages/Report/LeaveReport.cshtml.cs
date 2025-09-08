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

namespace PIS2.Pages.Report
{
    public class LeaveReportModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public LeaveReportModel(PIS2.Models.PISContext context)
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
            
            Companies = await _context.Companies.Where(c =>c.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToListAsync();
            LeaveTypes = await _context.LeaveTypes.Where(l => l.leaveTypeStatus==mainStatus.Active).OrderBy(lt => lt.leaveTypeName).ToListAsync();

            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel).ThenInclude(e => e.personModel)
                .Include(l => l.leaveTypeModel)
                .Where(l => l.leaveStatus == leaveStatus.Hold || l.leaveStatus == leaveStatus.Approved)
                .Take(1500)
                .ToListAsync();
           
            totalUnposted = leaveModel.Sum(l => l.leaveDays);
            CountUnposted = leaveModel.Count();
            totalCount = leaveModel.Count();
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
        public IActionResult OnGetFilter(
     int? department, int? company, int? leaveType,
     int? leaveStatus, DateTime? dateStart, DateTime? dateEnd)
        {
            Console.WriteLine("the Date is " + dateStart);

            var leaves = _context.Leaves
                .Include(l => l.leaveTypeModel)
                .Select(l => new
                {
                    Leave = l,
                    Employee = l.employmentModel,
                    person = l.employmentModel.personModel,
                    empType = l.employmentModel.employmentTypeModel,

                    ActiveJobPlacement = l.employmentModel.JobPlacements
                        .Where(j => j.jobPlacementStatus == mainStatus.Active)
                        .OrderByDescending(j => j.jobPlacementDate)
                        .FirstOrDefault(),

                })
                .AsEnumerable() // switch to in-memory for null safety
                .Select(x => new LeaveView
                {
                    Leave = x.Leave,
                    Employee = x.Employee,
                    person = x.person,
                    empType = x.empType,
                    Department = x.ActiveJobPlacement?.departmentModel,
                    Company = x.ActiveJobPlacement?.departmentModel?.companyModel,
                    JobTitle = x.ActiveJobPlacement?.jobModel
                })
                .AsQueryable();

            // Apply filters
            if (company.HasValue)
                leaves = leaves.Where(e => e.Company != null && e.Company.companyID == company.Value);

            if (department.HasValue)
                leaves = leaves.Where(e => e.Department != null && e.Department.departmentID == department.Value);

            if (leaveStatus.HasValue) // enum filter
                leaves = leaves.Where(e => e.Leave.leaveStatus ==(leaveStatus) leaveStatus.Value);

            if (leaveType.HasValue)
                leaves = leaves.Where(e => e.Leave.leaveTypeID == leaveType.Value);

            if (dateStart.HasValue)
                leaves = leaves.Where(e => e.Leave.leaveStartDate >= dateStart.Value);

            if (dateEnd.HasValue)
                leaves = leaves.Where(e => e.Leave.leaveEndDate <= dateEnd.Value);

            var filteredLeaves = leaves
                .OrderBy(e => e.Leave.leaveReaquestDate)
                .ToList();

            // Build HTML
            var tableHtml = string.Join("", filteredLeaves.Select(e =>
                $"<tr><td><a href='/Leave/Details?id={e.Leave.leaveID}' class='text-decoration-none text-dark'>{e.Leave.leaveID}</a></td>" +
                $"<td><a href='/Employment/Details?id={e.Employee.employmentID}' class='text-decoration-none text-dark'>{e.Employee.givenID}</a></td>" +
                $"<td><a href='/Person/Details?id={e.person.personID}' class='text-decoration-none text-dark'>{e.person.personFullName}</a></td>" +
                $"<td>{e.Leave.leaveReaquestDate:yyyy-MM-dd}</td>" +
                $"<td>{e.Leave.leaveStartDate:yyyy-MM-dd}</td>" +
                $"<td>{e.Leave.leaveEndDate:yyyy-MM-dd}</td>" +
                $"<td>{e.Leave.leaveDays}</td>" +
                $"<td>{e.Leave.leaveTypeModel.leaveTypeName}</td>" +
                $"<td>{e.Leave.leaveStatus}</td></tr>"
            ));

            filteredCount = filteredLeaves.Count;

            return new JsonResult(new { tableHtml, filteredCount });
        }


    }
    public class LeaveView
    {
        public leaveModel Leave { get; set; }
        public employmentModel Employee { get; set; }
        public personModel person { get; set; }
        public employmentTypeModel empType { get; set; }

        public departmentModel Department { get; set; }
        public companyModel Company { get; set; }
        public jobModel JobTitle { get; set; }

    }

}
