using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.JobClass
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<jobClassModel> jobClassModel { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public mainStatus? JobClassStatus { get; set; }
        public async Task OnGetAsync()
        {
            var classes = _context.JobClasses.AsQueryable();
            if (JobClassStatus != null)
            {
                classes = classes.Where(g => g.jobClassStatus == JobClassStatus);
            }
            jobClassModel = await classes.ToListAsync();
        }
    }
}
