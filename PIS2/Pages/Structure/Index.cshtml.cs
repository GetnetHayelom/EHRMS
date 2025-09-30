using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Structure
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        public IndexModel(PISContext context) {_context = context;} 

        public IList<structureModel> Structures { get; set; } = new List<structureModel>();

        public async Task OnGetAsync()
        {
            Structures = await _context.Structures.Include(s => s.departmentModel)?.ThenInclude(d => d.companyModel)
                .Include(s => s.jobModel).ToListAsync();
        }
    }
}
