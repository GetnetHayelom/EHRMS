using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.EmploymentType
{
    [Authorize(Roles = "HRMANAGER,MANAGEMENT")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<employmentTypeModel> employmentTypeModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ModelState.AddModelError(string.Empty, "You do not have permission to perform this action.");
            employmentTypeModel = await _context.EmploymentTypes.ToListAsync();
        }
    }
}
