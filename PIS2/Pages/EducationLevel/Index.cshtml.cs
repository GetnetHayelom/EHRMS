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
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<educationLevelModel> educationLevelModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            educationLevelModel = await _context.EducationLevels.ToListAsync();
        }
    }
}
