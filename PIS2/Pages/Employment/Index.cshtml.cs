using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        public async Task OnGetAsync()
        {
            Departments = await _context.Departments.ToListAsync();
            employmentModel = await _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.employmentTypeModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.departmentModel)
                .Include(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Take(1)).ThenInclude(jp => jp.jobModel)
                .ToListAsync();
        }
        public IActionResult OnGetFilter(string filter, int filterValue)
        {

            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "Department":
                        employmentModel = employmentModel
                            .Where(e => e.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate)
                            .Select(jp => jp.departmentModel.departmentID).FirstOrDefault() == filterValue).ToList();
                        break;
                }

            }

            // Return filtered employees as HTML
            var tableHtml = string.Join("", employmentModel.Select(e =>
                $"<tr><td>{e.personModel.personFullName}</td><td>{e.JobPlacements.FirstOrDefault().departmentModel.departmentName}</td><td>{e.JobPlacements.FirstOrDefault().jobModel.jobTitle}</td><td>{e.JobPlacements.FirstOrDefault().jobPlacementDate.ToShortDateString()}</td></tr>")
            );

            return Content(tableHtml);
        }
    }
}
