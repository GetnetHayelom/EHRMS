using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.EducationLevel
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public educationLevelModel educationLevelModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var educationlevelmodel = await _context.EducationLevels.FirstOrDefaultAsync(m => m.educationLevelID == id);
            if (educationlevelmodel == null)
            {
                return NotFound();
            }
            else
            {
                educationLevelModel = educationlevelmodel;
            }
            return Page();
        }
    }
}
