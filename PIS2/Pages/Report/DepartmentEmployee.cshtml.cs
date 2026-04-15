using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Report
{
    public class DepartmentEmployee : PageModel
    {
        private readonly PISContext _context;

        public DepartmentEmployee(PISContext context)
        {
            _context = context;
        }

        public IList<employmentModel> Employments { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Employments = await _context.Employments
                .ToListAsync();
        }
    }
}
