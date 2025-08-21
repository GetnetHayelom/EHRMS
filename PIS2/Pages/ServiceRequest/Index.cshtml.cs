using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        public IList<serviceRequestModel> serviceRequestModel { get; set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<companyModel> Companies { get; set; }
        public IList<workSiteModel> WorkLocations { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRequest { get; set; }
        public int Hold { get; set; }
        public int Post { get; set; }
        public int Approved { get; set; }
        public int Declined { get; set; }
        public int Completed { get; set; }
       

        public async Task OnGetAsync(string status, string department, string company, string workLoc, string startDate, string endDate)
        {
            Departments = await _context.Departments
                .Where(d => d.departmentStatus == mainStatus.Active)
                .OrderBy(d => d.departmentName).ToListAsync();

            WorkLocations = await _context.WorkSites
                .Where(d => d.workSiteStatus == mainStatus.Active)
                .OrderBy(w => w.workSiteName).ToListAsync();

            Companies = await _context.Companies
                .Where(d => d.companyStatus == mainStatus.Active)
                .OrderBy(c => c.companyName).ToListAsync();


            StartDate = _context.ServiceRequests.OrderByDescending(s => s.serviceRequestDate).First().serviceRequestDate;
            EndDate = DateTime.Now;
            //EndDate = _context.ServiceRequests.OrderBy(s => s.serviceRequestDate).FirstOrDefault().serviceRequestDate;

            var qry = _context.ServiceRequests
                .Include(s => s.Employment).ThenInclude(e => e.SiteAssignments.OrderByDescending(sa =>sa.modifiedDate).FirstOrDefault())
                    .Include(s => s.Employment).ThenInclude(e => e.JobPlacements)
                        .ThenInclude(j => j.departmentModel)
                            .ThenInclude(d => d.companyModel)
                .Include(s => s.Employment)
                    .ThenInclude(e => e.JobPlacements)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                qry = qry.Where(s => s.serviceRequestStatus.ToString() == status);
            }
            else
            {
                
            }

            if (!string.IsNullOrEmpty(department))
                qry = qry.Where(s => s.Employment.JobPlacements
                                      .Any(j => j.departmentModel.departmentID == int.Parse(department)));

            if (!string.IsNullOrEmpty(company))
                qry = qry.Where(s => s.Employment.JobPlacements
                                      .Any(j => j.departmentModel.companyModel.companyID == int.Parse(company)));

            if (!string.IsNullOrEmpty(workLoc))
                qry = qry.Where(s => s.Employment.SiteAssignments
                                      .Any(j => j.workSiteModel.workSiteID == int.Parse(workLoc)));

            if (DateTime.TryParse(startDate, out var start))
                qry = qry.Where(s => s.serviceRequestDate >= start);

            if (DateTime.TryParse(endDate, out var end))
                qry = qry.Where(s => s.serviceRequestDate <= end);


            serviceRequestModel = await qry.ToListAsync();
        }


        //public async Task OnGetAsync()
        //{
        //    Departments = await _context.Departments
        //        .Where(d => d.departmentStatus == mainStatus.Active)
        //        .OrderBy(d => d.departmentName).ToListAsync();

        //    WorkLocations = await _context.WorkSites
        //        .Where(d => d.workSiteStatus == mainStatus.Active)
        //        .OrderBy(w => w.workSiteName).ToListAsync();

        //    Companies = await _context.Companies
        //        .Where(d => d.companyStatus == mainStatus.Active)
        //        .OrderBy(c => c.companyName).ToListAsync();

        //    StartDate = DateTime.Now;

        //    EndDate =DateTime.Now;

        //    serviceRequestModel = await _context.ServiceRequests
        //        .Include(s => s.Employment)
        //            .ThenInclude(e => e.JobPlacements)
        //                .ThenInclude(j => j.departmentModel)
        //                    .ThenInclude(d =>d.companyModel)

        //        .Include(s => s.Employment)
        //            .ThenInclude(e => e.JobPlacements)
        //                .ThenInclude(j => j.workSiteModel)
        //        .ToListAsync();
        //}



    }
}
