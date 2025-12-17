using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Department
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<departmentModel> departmentModel { get;set; } = default!;
        public IList<companyModel> Companies { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Companies =await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
            departmentModel = await _context.Departments
                .Include(d => d.companyModel)
                .Include(d => d.subAccountModel)
                .Include(d => d.employmentModel).ThenInclude(e => e.personModel)
                .OrderBy(d => d.departmentName).ToListAsync();
        }

        public async Task<IActionResult> OnGetFilterAsync(int? companyId, int? status)
        {
            var depts = _context.Departments
                .Include(d => d.companyModel)
                .Include(d => d.subAccountModel)
                .OrderBy(d => d.companyModel.companyName).ThenBy(d => d.departmentName).AsQueryable();

            if (companyId.HasValue)
                depts = depts.Where(d => d.companyID == companyId.Value);

            if (status != null)
                depts = depts.Where(d => d.departmentStatus == (mainStatus) status);

            var grouped = await depts.ToListAsync();

            return Partial("_DepartmentsPartial", grouped);
        }

    }
}
