using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Pages.Company;

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
        public int overtimeCount { get; set; } = default!;
        public int leaveCount {  get; set; } = default!;
        public int absentismCount { get; set; } = default!;
        public int serviceRequestCount { get; set; } = default!;
        public int pensionCount { get; set; } = default!;
        public int contractEndCount { get; set; } = default!;
        public async Task OnGetAsync()
        {
            persons = await _context.Persons
                .Include(p => p.addressModel).ToListAsync();
            
            leaveCount = _context.Leaves
               .Include(p => p.leaveTypeModel)
               .Include(l => l.employmentModel)
               .Where(l => l.leaveStatus == leaveStatus.Hold && l.employmentModel.employmentStatus == mainStatus.Active && l.leaveTypeModel.leaveGroup != leaveGroup.Absentism).Count();

            absentismCount = _context.Leaves
                .Include(p => p.leaveTypeModel)
                .Include(l => l.employmentModel)
                .Where(l => l.leaveStatus == leaveStatus.Hold && l.leaveTypeModel.leaveGroup == leaveGroup.Absentism && l.employmentModel.employmentStatus == mainStatus.Active).Count();

            jobPlacements = await _context.JobPlacements
               .Include(p => p.jobModel).ThenInclude(j => j.jobClassModel)
               .Include(p => p.jobModel).ThenInclude(j => j.jobCategoryModel)
               .Include(p => p.jobModel).ThenInclude(j => j.jobGradeModel)
               .ToListAsync();

            loyalties = await _context.Loyalties.ToListAsync();

            overtimeCount = _context.OvertimeRecords
                .Include(o => o.employmentModel)
                .Include(o => o.overtimeModel)
                .Where(o => o.overtimeRecordStatus == overtimeStatus.Hold && o.employmentModel.employmentStatus== mainStatus.Active && o.overtimeModel.overtimeStatus == mainStatus.Active).Count();

            allowances = await _context.AllowanceAssignments.ToListAsync();
            serviceRequestCount = _context.ServiceRequests.Where(s => s.serviceRequestStatus == ServiceRequestStatus.Hold).Count();

            pensionCount = _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Where(e => e.employmentStatus == mainStatus.Active && e.personModel.personDoB < (DateTime.Now.AddYears(-e.employmentTypeModel.employmentMaxAge)).AddMonths(6)).Count();

            contractEndCount = _context.Employments
                .Where(e =>e.employmentStatus == mainStatus.Active && e.employmentTerminationDate < DateTime.Now.AddDays(-7) && e.employmentTypeID == 2).Count();
        }

        public IActionResult OnGetGetSummary(string sumType)
        {
            var tableHeader="";
            var tableBody="";
            var tableTitle = "";
            switch (sumType)
            {
                case "Pension":
                    var pensionList = _context.Employments
                    .Include(e => e.personModel)
                    .Include(e => e.employmentTypeModel)
                    .Include(e => e.JobPlacements)
                        .ThenInclude(j => j.departmentModel)
                    .Include(e => e.JobPlacements)
                        .ThenInclude(j => j.jobModel)
                    .Where(e => e.employmentStatus == mainStatus.Active && e.personModel.personDoB < (DateTime.Now.AddYears(-e.employmentTypeModel.employmentMaxAge)).AddMonths(6)).ToList();

                    tableTitle = "Employee Late& Upcoming(6 months) Retirements ";
                    tableHeader =  "<td>Employee ID</td><td>Full Name</td><td>Department</td><td>Job Title</td><td>Due Date</td>";
                     
                    tableBody = string.Join("", pensionList.Select(e =>
                    {
                        var url = Url.Page("/Employment/Details", new { id = e.employmentID });
                        var activeJobPlacement = e.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string jobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "N/A";
                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                     
                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{e.givenID}</td><td>{e.personModel.personFullName}</td><td>{department}</td><td>{jobTitle}</td>" +
                        $"<td>{e.personModel.personDoB.AddYears(e.employmentTypeModel.employmentMaxAge).ToString("MMM dd,yyyy")}</td></tr>";
                    }));
                    break;
                case "Leave":
                    var leaveList = _context.Leaves
                        .Include(l => l.leaveTypeModel)
                        .Include(l => l.employmentModel).ThenInclude(e => e.JobPlacements).ThenInclude(j =>j.departmentModel)
                        .Where(l => l.leaveStatus == leaveStatus.Hold && l.employmentModel.employmentStatus == mainStatus.Active)
                        .ToList();

                    tableTitle = "Pending Leave Requests";
                    tableHeader = "<td>Employee ID</td><td>Request Date</td><td>Department</td><td>Leave Status</td>";

                    tableBody = string.Join("", leaveList.Select(l =>
                    {
                        var url = Url.Page("/Leave/Details", new { id = l.leaveID });
                        var activeJobPlacement = l.employmentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{l.employmentModel.givenID}</td>" +
                        $"<td>{l.leaveReaquestDate.ToString("MMM dd, yyyy")}</td><td>{department}</td><td>{l.leaveStatus}</td></tr>";
                    }));

                    break;
                case "Overtime":
                    var overtimeCount = _context.OvertimeRecords
                        .Include(o => o.employmentModel).ThenInclude(e =>e.JobPlacements).ThenInclude(j =>j.departmentModel)
                        .Include(o=>o.overtimeModel)
                        .Where(o => o.overtimeRecordStatus == overtimeStatus.Hold && o.employmentModel.employmentStatus == mainStatus.Active && o.overtimeModel.overtimeStatus == mainStatus.Active).ToList();
                    tableTitle = "Pending Overtime";
                    tableHeader = "<td>Employee ID</td><td>Overtime Type</td><td>Request Date</td><td>Department</td><td>Leave Status</td>";

                    tableBody = string.Join("", overtimeCount.Select(o =>
                    {
                        var url = Url.Page("/Overtime/Details", new { id = o.overtimeID });
                        var activeJobPlacement = o.employmentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{o.employmentModel.givenID}</td><td>{o.overtimeModel.overtimeName}</td>" +
                        $"<td>{o.overtimeRecordDate.ToString("MMM dd, yyyy")}</td><td>{department}</td><td>{o.overtimeRecordStatus}</td></tr>";
                    }));
                    break;
                case "Service":
                    var servicesList = _context.ServiceRequests
                        .Include(s => s.Employment).ThenInclude(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Where(s => s.serviceRequestStatus == ServiceRequestStatus.Hold).ToList();

                    tableTitle = "Service Requests";
                    tableHeader = "<td>Employee ID</td><td>Requested Service</td><td>Request Date</td><td>Department</td><td>Request Status</td>";

                    tableBody = string.Join("", servicesList.Select(s =>
                    {
                        var url = Url.Page("/ServiceRequest/Details", new { id = s.serviceRequestID });
                        var activeJobPlacement = s.Employment.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{s.Employment.givenID}</td><td>{s.requestedService}</td><td>{s.serviceRequestDate}</td><td>{department}</td><td>{s.serviceRequestStatus}</td></tr>";
                    }));
                    break;
                case "Contract":
                    var contractList = _context.Employments
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(e => e.personModel)
                        .Where(e => e.employmentStatus == mainStatus.Active && e.employmentTerminationDate < DateTime.Now.AddDays(7) && e.employmentTypeID == 2).ToList();

                    
                    tableTitle = "Contracts Terminating Within 7 Days";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>Department</td><td>Job Title</td><td>Due Date</td>";

                    tableBody = string.Join("", contractList.Select(c =>
                    {
                        var url = Url.Page("/Employment/Details", new { id = c.employmentID });
                        var activeJobPlacement = c.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string jobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "N/A";
                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{c.givenID}</td><td>{c.personModel.personFullName}</td><td>{department}</td><td>{jobTitle}</td><td>{c.employmentTerminationDate}</td></tr>";
                    }));
                    break;
                case "Termination":
                    var terminationList = _context.Terminations.Include(t => t.EmploymentModel)
                        .ThenInclude(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(t => t.EmploymentModel).ThenInclude(e => e.personModel)
                        .Where(e => e.terminationStatus == terminationStatus.Hold).ToList();


                    tableTitle = "Requests for Employment Termination";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>Date</td><td>Department</td><td>Job Title</td><td>Reason</td><td>Status</td>";

                    tableBody = string.Join("", terminationList.Select(t =>
                    {
                        var url = Url.Page("/Termination/Details", new { id = t.terminationID });
                        var activeJobPlacement = t.EmploymentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string jobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "N/A";
                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{t.EmploymentModel.givenID}</td>" +
                        $"<td>{t.EmploymentModel?.personModel.personFullName}</td><td>{t.terminationDate.ToShortDateString()}</td><td>{department}</td><td>{jobTitle}</td>" +
                        $"<td>{t.terminationReason}</td><td>{t.terminationStatus}</td></tr>";
                    }));
                    break;
                case "Allowance":
                    var AllowanceList = _context.AllowanceAssignments.Include(a => a.employmentModel)
                        .ThenInclude(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(a => a.allowanceModel)
                        .Include(e => e.employmentModel).ThenInclude(e => e.personModel)
                        .Where(e => e.allowanceStatus == mainStatus.Suspended).ToList();


                    tableTitle = "Pending Allowance";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>Date</td><td>Department</td><td>Job Title</td><td>Allowance Name</td><td>Status</td>";

                    tableBody = string.Join("", AllowanceList.Select(a =>
                    {
                        var url = Url.Page("/AllowanceAssignment/Details", new { id = a.allowanceAssignmentID });
                        var activeJobPlacement = a.employmentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string jobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "N/A";
                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{a.employmentModel.givenID}</td>" +
                        $"<td>{a.employmentModel?.personModel.personFullName}</td><td>{a.allowanceAssignmentDate}</td><td>{department}</td><td>{jobTitle}</td>" +
                        $"<td>{a.allowanceModel.allowanceName}</td><td>{a.allowanceStatus}</td></tr>";
                    }));
                    break;
                case "JobPlacement":
                    var JobPlacementList = _context.JobPlacements.Include(j => j.departmentModel)
                        .Include(j => j.jobModel)
                        .Include(j => j.employmentModel).ThenInclude(e => e.personModel)
                        .Where(e => e.jobPlacementStatus == mainStatus.Suspended).ToList();


                    tableTitle = "Pending Job Placements";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>Date</td><td>Department</td><td>Job Title</td><td>Reason</td><td>Status</td>";

                    tableBody = string.Join("", JobPlacementList.Select(j =>
                    {
                        var url = Url.Page("/JobPlacement/Details", new { id = j.jobPlacementID });
                        var activeJobPlacement = j.employmentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string jobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "N/A";
                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{j.employmentModel.givenID}</td>" +
                        $"<td>{j.employmentModel?.personModel.personFullName}</td><td>{j.jobPlacementDate}</td><td>{department}</td><td>{jobTitle}</td>" +
                        $"<td>{j.jobPlacementReason}</td><td>{j.jobPlacementStatus}</td></tr>";
                    }));
                    break;
                case "Prohibition":
                    var prohibitionList = _context.Prohibitions
                        .Include(j => j.employmentModel).ThenInclude(e => e.personModel)
                        .Where(e => e.prohibitionStatus == mainStatus.Active).ToList();


                    tableTitle = "Active Prohibitions";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>From</td><td>To</td><td>Type</td><td>Reason</td><td>Modified By</td>";

                    tableBody = string.Join("", prohibitionList.Select(p =>
                    {
                        var url = Url.Page("/Prohibition/Details", new { id = p.prohibitionID });
                        

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{p.employmentModel.givenID}</td>" +
                        $"<td>{p.employmentModel?.personModel.personFullName}</td><td>{p.prohibitionStart}</td><td>{p.prohibitionEnd}</td><td>{p.prohibitionType}</td>" +
                        $"<td>{p.prohibitionReason}</td><td>{p.modifiedBy}</td></tr>";
                    }));
                    break;
                case "Employment":
                    var employmentsList = _context.Employments.Include(e => e.personModel)
                        .Include(e => e.employmentTypeModel)
                        .Where(e => e.employmentStatus == mainStatus.Suspended).ToList();


                    tableTitle = "Suspended Employments";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>Hire Date</td><td>Type</td><td>Reference</td><td>Modified By</td>";

                    tableBody = string.Join("", employmentsList.Select(e =>
                    {
                        var url = Url.Page("/Employment/Details", new { id = e.employmentID });


                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{e.givenID}</td>" +
                        $"<td>{e.personModel?.personFullName}</td><td>{e.employmentDate}</td><td>{e.employmentTypeModel?.employmentTypeName}</td>" +
                        $"<td>{e.employmentReference}</td><td>{e.modifiedBy}</td></tr>";
                    }));
                    break;

            }

            return new JsonResult(new { tableHeader, tableBody, tableTitle });
        }
    }
}
