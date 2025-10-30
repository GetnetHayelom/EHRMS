using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Users
{
    [Authorize(Roles = "MIE\\PMS_ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<userModel> userModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            userModel = await _context.Users
                .Include(u => u.personModel).ToListAsync();
        }
    }
}
