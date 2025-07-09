using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Guaranty
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<guarantyModel> guarantyModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            guarantyModel = await _context.Guaranties
                .Include(g => g.Employment).ToListAsync();
        }
    }
}
