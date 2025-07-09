using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.JobStep
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<jobStepModel> jobStepModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            jobStepModel = await _context.JobSteps
                .Include(j => j.jobGradeModel).ToListAsync();
        }
    }
}
