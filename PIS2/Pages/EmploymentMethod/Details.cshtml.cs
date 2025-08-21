using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.EmploymentMethod
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public employmentMethodModel employmentMethodModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employmentmethodmodel = await _context.EmploymentMethods.FirstOrDefaultAsync(m => m.employmentMethodID == id);
            if (employmentmethodmodel == null)
            {
                return NotFound();
            }
            else
            {
                employmentMethodModel = employmentmethodmodel;
            }
            return Page();
        }
    }
}
