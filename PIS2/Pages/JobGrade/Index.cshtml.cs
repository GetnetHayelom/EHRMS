using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.JobGrade
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<jobGradeModel> jobGradeModel { get;set; } = default!;
        [BindProperty(SupportsGet=true)]
        public mainStatus? JobGradeStatus { get; set; }
        public async Task OnGetAsync()
        {
            var grades = _context.JobGrades.AsQueryable();

            if(JobGradeStatus != null)
            {
                grades = grades.Where(g => g.jobGradeStatus == JobGradeStatus);
            }
            jobGradeModel = await grades.ToListAsync();
        }
    }
}
