using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobClass
{
    
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public jobClassModel jobClassModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobclassmodel = await _context.JobClasses.FirstOrDefaultAsync(m => m.JobClassId == id);
            if (jobclassmodel == null)
            {
                return NotFound();
            }
            else
            {
                jobClassModel = jobclassmodel;
            }
            return Page();
        }
    }
}
