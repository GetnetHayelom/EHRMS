using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.EmploymentMethod
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_MANAGEMENT")]
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
