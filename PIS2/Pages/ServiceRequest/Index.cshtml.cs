using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.ServiceRequest
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
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
       

        public async Task OnGetAsync(int? reqStatus, int? department, int? company, int? reqType, DateTime? startDate, DateTime? endDate)
        {
            Departments = new SelectList(await _context.Departments
                .Where(d => d.departmentStatus == mainStatus.Active)
                .OrderBy(d => d.departmentName).ToListAsync(), "departmentID", "departmentName");

           
            Companies = new SelectList(await _context.Companies
                .Where(d => d.companyStatus == mainStatus.Active)
                .OrderBy(c => c.companyName).ToListAsync(), "companyID", "companyName");


            StartDate = _context.ServiceRequests.OrderByDescending(s => s.serviceRequestDate).First().serviceRequestDate;
            EndDate = DateTime.Now;
            //EndDate = _context.ServiceRequests.OrderBy(s => s.serviceRequestDate).FirstOrDefault().serviceRequestDate;

            var qry = _context.ServiceRequests
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
                qry = qry.Where(s => s.requestedService == (ServiceRequestTypes) reqType);
            }

            if (company.HasValue)
            {
                qry = qry.Where(s => s.Employment.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.companyID == company);
            }
                


            if (department.HasValue) 
            {
                qry = qry.Where(s => s.Employment.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.departmentID == department);
            }
                

           

            if (startDate.HasValue) { qry = qry.Where(s => s.serviceRequestDate >= startDate);}


            if (endDate.HasValue) { qry = qry.Where(s => s.serviceRequestDate <= endDate); }


            serviceRequestModel = await qry.ToListAsync();
        }





    }
}
