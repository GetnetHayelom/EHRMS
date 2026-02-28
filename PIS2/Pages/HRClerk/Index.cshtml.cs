using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PIS2.Models;
using PIS2.Pages.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.HRClerck
{
        [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public IndexModel(PIS2.Models.PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }

        public int employmentRequests { get; set; }
        public int employments { get; set; }
        public int terminations { get; set; }
        public int jobPlacements { get; set; } = default!;
        public int loyalties { get; set; } = default!;  
        public int allowances { get; set; } = default!;
        public int overtimeCount { get; set; } = default!;
        public int leaveCount {  get; set; } = default!;
        public int absentismCount { get; set; } = default!;
        public int serviceRequestCount { get; set; } = default!;
        public int pensionCount { get; set; } = default!;
        public int contractEndCount { get; set; } = default!;
        public int prohibitions { get; set; } = default!;
        public int lettersCount { get; set; } = default!;
        public async Task OnGetAsync()
        {
            var userID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.userID ?? 0;

            var empID = _core.getUserEmp(User.Identity.Name);

            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;

            var accessibleCompanies = _context.Accesses
                .Where(a => a.userID == userID && a.accessStatus == mainStatus.Active) // 1 = active access
                .Select(a => a.companyID)
                .ToList();

            var allowedCompanies = accessibleCompanies
                .Append(company)
                .Where(c => c != null)
                .Distinct()
                .ToList();

            var jobs =await _context.JobPlacements
                .Include(j => j.employmentModel).ThenInclude(e => e.personModel)
                .Include(j => j.employmentModel).ThenInclude(e => e.employmentTypeModel)
                .Include(j => j.departmentModel).Where(d => allowedCompanies.Contains(d.departmentModel.companyID) && d.jobPlacementStatus == mainStatus.Active).ToListAsync();

            var emps = jobs.Select(j=>j.employmentModel).ToList();
            var empIDs = emps.Select(e => e.employmentID).ToList();

            var persons = _context.Persons.Where(p => emps.Select(e => e.personID).Contains(p.personID));
           

            employments = _context.Employments.Where(e => e.employmentStatus == mainStatus.Suspended).Count();
            terminations = _context.Terminations.Where(t => t.terminationStatus != terminationStatus.Complete && empIDs.Contains(t.employmentID)).Count();
            employmentRequests = _context.JobRequirements.Include(d => d.DepartmentModel)
                .Where(j => (j.jobRequirementStatus != jobReqStatus.Completed || j.jobRequirementStatus != jobReqStatus.Declined)
                    && allowedCompanies.Contains(j.DepartmentModel.companyID)).Count();

            leaveCount = _context.Leaves.Where(l => empIDs.Contains(l.employmentID) && l.leaveStatus == leaveStatus.Hold || l.leaveStatus == leaveStatus.Approved).Count();
            
            absentismCount = _context.Leaves.Include(p => p.leaveTypeModel).Include(l => l.employmentModel).Where(l => l.leaveStatus == leaveStatus.Hold && l.leaveTypeModel.leaveGroup == leaveGroup.Absentism && l.employmentModel.employmentStatus == mainStatus.Active).Count();

            jobPlacements = _context.JobPlacements.Where(j => empIDs.Contains(j.employmentID) && j.jobPlacementStatus == mainStatus.Suspended).Count();

            loyalties = _context.LoyaltyHistories.Where(l => l.loyaltyHistoryStatus == loyaltyStatus.Hold && empIDs.Contains(l.employmentID)).Count();

            overtimeCount = _context.OvertimeRecords.Where(o => o.overtimeRecordStatus == overtimeStatus.Hold && empIDs.Contains(o.employmentID)).Count();

            allowances = _context.AllowanceAssignments.Where(a => empIDs.Contains(a.employmentID) && a.allowanceStatus == mainStatus.Suspended).Count();

            serviceRequestCount = _context.ServiceRequests.Where(s => s.serviceRequestStatus == ServiceRequestStatus.Hold && empIDs.Contains(s.employmentID)).Count();

            pensionCount = emps.Where(e => e.personModel?.personDoB < (DateTime.Now.AddYears(-e.employmentTypeModel.employmentMaxAge)).AddMonths(6)).Count();

            contractEndCount = _context.Contracts.Where(e => empIDs.Contains(e.employmentID) && e.endDate < DateTime.Now.AddDays(-7)).Count();
            prohibitions = _context.Prohibitions.Where(p => p.prohibitionStatus != mainStatus.Inactive && empIDs.Contains(p.employmentID)).Count();
            lettersCount = _context.Letters.Where(l=> l.letterStatus == LetterStatus.Draft).Count();
        }

        public IActionResult OnGetGetSummary(string sumType)
        {
            var userID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.userID ?? 0;

            var empID = _core.getUserEmp(User.Identity.Name);

            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;

            var accessibleCompanies = _context.Accesses
                .Where(a => a.userID == userID && a.accessStatus == mainStatus.Active) // 1 = active access
                .Select(a => a.companyID)
                .ToList();

            var allowedCompanies = accessibleCompanies
                .Append(company)
                .Where(c => c != null)
                .Distinct()
                .ToList();

            var jobs = _context.JobPlacements
                .Include(j => j.departmentModel).Where(d => allowedCompanies.Contains(d.departmentModel.companyID) && d.jobPlacementStatus == mainStatus.Active).ToList();

            var emps = _context.Employments
                .Include(e => e.personModel)?
                .Include(e => e.employmentTypeModel)?
                .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                .Where(e => jobs.Select(j => j.employmentID).ToList().Contains(e.employmentID)).ToList();

            var empIDs = emps?.Select(e => e.employmentID).ToList() ?? new List<int>();
            var tableHeader="";
            var tableBody="";
            var tableTitle = "";
            switch (sumType)
            {
                case "Pension":
                    var pensionList =emps?
                    .Where(e => e.personModel.personDoB < (DateTime.Now.AddYears(-e.employmentTypeModel.employmentMaxAge)).AddMonths(6)).ToList();

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
                        .Include(l => l.employmentModel)?.ThenInclude(e => e.JobPlacements).ThenInclude(j =>j.departmentModel)
                        .Where(l => l.leaveStatus == leaveStatus.Hold && empIDs.Contains(l.employmentID))
                        .ToList();

                    tableTitle = "Pending Leave Requests";
                    tableHeader = "<td>Employee ID</td><td>Request Date</td><td>Department</td><td>Leave Status</td>";

                    tableBody = string.Join("", leaveList.Select(l =>
                    {
                        var url = Url.Page("/Leave/Details", new { id = l.leaveID });
                        var activeJobPlacement = l.employmentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{l.employmentModel.givenID}</td>" +
                        $"<td>{l.leaveRequestDate.ToString("MMM dd, yyyy")}</td><td>{department}</td><td>{l.leaveStatus}</td></tr>";
                    }));

                    break;
                case "Overtime":
                    var overtimeCount = _context.OvertimeRecords
                        .Include(o => o.employmentModel).ThenInclude(e =>e.JobPlacements).ThenInclude(j =>j.departmentModel)
                        .Include(o=>o.overtimeModel)
                        .Where(o => o.overtimeRecordStatus == overtimeStatus.Hold && empIDs.Contains(o.employmentID)).ToList();
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
                        .Where(s => s.serviceRequestStatus == ServiceRequestStatus.Hold && empIDs.Contains(s.employmentID)).ToList();

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
                    var contractList = _context.Contracts
                        .Include(c => c.employmentModel).ThenInclude(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(c => c.employmentModel).ThenInclude(e => e.personModel)
                        .Where(e => empIDs.Contains(e.employmentID) && e.endDate < DateTime.Now.AddDays(7)).ToList();

                    tableTitle = "Contracts Terminating Within 7 Days";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>Department</td><td>Job Title</td><td>Due Date</td>";

                    tableBody = string.Join("", contractList.Select(c =>
                    {
                        var url = Url.Page("/Employment/Details", new { id = c.employmentID });
                        var activeJobPlacement = c.employmentModel?.JobPlacements?.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active);

                        string jobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "N/A";
                        string department = activeJobPlacement?.departmentModel?.departmentName ?? "N/A";

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{c.employmentModel?.givenID}</td><td>{c.employmentModel?.personModel?.personFullName}</td><td>{department}</td><td>{jobTitle}</td><td>{c.endDate}</td></tr>";
                    }));
                    break;
                case "Termination":
                    var terminationList = _context.Terminations.Include(t => t.EmploymentModel)
                        .ThenInclude(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(t => t.EmploymentModel).ThenInclude(e => e.personModel)
                        .Where(e => e.terminationStatus == terminationStatus.Hold && empIDs.Contains(e.employmentID)).ToList();


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
                        .Where(e => (e.allowanceStatus == mainStatus.Suspended || (e.allowanceAssignmentEndDate < DateTime.Now && e.allowanceStatus == mainStatus.Active))&& empIDs.Contains(e.employmentID)).ToList();


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
                        .Where(e => e.jobPlacementStatus == mainStatus.Suspended && empIDs.Contains(e.employmentID)).ToList();

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
                        .Where(e => (e.prohibitionStatus == mainStatus.Active || e.prohibitionStatus == mainStatus.Suspended) && empIDs.Contains(e.employmentID)).ToList();

                    tableTitle = "Prohibitions Requests";
                    tableHeader = "<td>Employee ID</td><td>Full Name</td><td>From</td><td>To</td><td>Type</td><td>Reason</td><td>Modified By</td>";

                    tableBody = string.Join("", prohibitionList.Select(p =>
                    {
                        var url = Url.Page("/Prohibition/Edit", new { id = p.prohibitionID });
                        

                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'><td>{p.employmentModel.givenID}</td>" +
                        $"<td>{p.employmentModel?.personModel.personFullName}</td><td>{p.prohibitionStart}</td><td>{p.prohibitionEnd}</td><td>{p.prohibitionType}</td>" +
                        $"<td>{p.prohibitionReason}</td><td>{p.modifiedBy}</td></tr>";
                    }));
                    break;
                case "Employment":
                    
                    var employmentsList = _context.Employments.Include(e => e.personModel)
                        .Include(e => e.employmentTypeModel)
                        .Where(e => e.employmentStatus == mainStatus.Suspended && empIDs.Contains(e.employmentID)).ToList();

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
                case "Letter":

                    var letters = _context.Letters.Include(l => l.LetterType).Where(l => l.letterStatus == LetterStatus.Draft).ToList();

                    tableTitle = "Drafted Letters";
                    tableHeader = "<td>Title/Subject</td><td>Group</td><td>Type</td><td>Last Modified</td><td>From-To</td>";

                    tableBody = string.Join("", letters.Select(e =>
                    {
                        var url = Url.Page("/Letter/Details", new { id = e.letterID });
                        return $"<tr onclick=\"location.href='{url}'\" style='cursor:pointer'>" +
                        $"<td>{e.letterSubject}</td><td>{e.letterGroup}</td><td>{e.LetterType?.letterTypeName}</td><td><div><h6 class='fw-bold'>{e.modifiedBy}</h6><h6 class='text-muted'>{e.modifiedDate}</h6></div></td><td>{e.letterSender}-{e.letterReceiver}</td>" +
                        $"</tr>";
                    }));
                    break;

            }

            return new JsonResult(new { tableHeader, tableBody, tableTitle });
        }
    }
}
