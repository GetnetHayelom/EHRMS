using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.HR;

namespace PIS2.Pages.ServiceRequest
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public List<serviceRequestModel> serviceRequestModel { get; set; } = default!;
        public SelectList Departments { get; set; } = default!;
        public SelectList Companies { get; set; }
        public SelectList RequestType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRequest { get; set; }
        public int Hold { get; set; }
        public int Post { get; set; }
        public int Approved { get; set; }
        public int Declined { get; set; }
        public int Completed { get; set; }
        public SelectList RequestTypes { get; set; }
       

        public async Task OnGetAsync(int? reqStatus, int? department, int? company, int? reqType, DateTime? startDate, DateTime? endDate)
        {

            Departments = new SelectList(await _context.Departments
                .Where(d => d.departmentStatus == mainStatus.Active)
                .OrderBy(d => d.departmentName).ToListAsync(), "departmentID", "departmentName");

           
            Companies = new SelectList(await _context.Companies
                .Where(d => d.companyStatus == mainStatus.Active)
                .OrderBy(c => c.companyName).ToListAsync(), "companyID", "companyName");

            var req = await _context.ServiceRequestTypes.Where(s => s.serviceRequestTypeStatus == mainStatus.Active).ToListAsync();
            RequestTypes = new SelectList(req, "serviceRequestTypeID", "serviceRequestTypeName");

            var qry =_context.ServiceRequests
                .Include(s => s.Employment).ThenInclude(e => e.SiteAssignments)
                .Include(s => s.Employment).ThenInclude(e => e.JobPlacements).ThenInclude(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Include(s => s.Employment).ThenInclude(e => e.JobPlacements)
                .AsQueryable();

            if (reqStatus.HasValue)
            {
                qry = qry.Where(s => s.serviceRequestStatus ==(ServiceRequestStatus)reqStatus);
            }

            if (reqType.HasValue)
            {
                qry = qry.Where(s => s.serviceRequestTypeID == reqType);
            }

            if (company.HasValue)
            {
                qry = qry.Where(s => s.Employment.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.companyID == company);
            }
                


            if (department.HasValue) 
            {
                qry = qry.Where(s => s.Employment.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.departmentID == department);
            }
            
            if (startDate.HasValue) { qry = qry.Where(s => s.serviceRequestDate >= startDate); }


            if (endDate.HasValue) { qry = qry.Where(s => s.serviceRequestDate <= endDate); }


            serviceRequestModel = await qry.ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id)
        {
            if (!User.IsInRole("HRPERSONNEL")) { return new JsonResult(new { success = false, message = "Redirecting" }); }

            var req = await _context.ServiceRequests.Include(e => e.Employment).FirstOrDefaultAsync(s => s.serviceRequestID == id);

            if(req == null) { return new JsonResult(new { success = false, message = "NotFound" }); }

            if (req.serviceRequestStatus == ServiceRequestStatus.Hold)
                req.serviceRequestStatus = ServiceRequestStatus.Reviewed;

            req.modifiedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Invalid request type.", type=req.ServiceRequestType?.serviceRequestTypeName.ToString(), empID=req.employmentID, prsnID=req.Employment?.personID });
        }

    }
}
