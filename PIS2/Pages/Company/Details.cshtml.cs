using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Company
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public companyModel companyModel { get; set; } = default!;
        public List<departmentModel> Departments { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companymodel = await _context.Companies
                .Include(c => c.Departments)?.ThenInclude(d => d.employmentModel).ThenInclude(e => e.personModel)
                .Include(c => c.Departments).ThenInclude(d => d.JobPlacements)
                .Include(c => c.Departments).ThenInclude(d => d.subAccountModel)
                .FirstOrDefaultAsync(m => m.companyID == id) ?? new companyModel();

            if (companymodel == null)
            {
                return NotFound();
            }
            else
            {
                companyModel = companymodel;
                Departments = companyModel.Departments?.OrderBy(d =>d.departmentName).ToList() ?? new List<departmentModel>();
            }
            return Page();
        }
    }
}
