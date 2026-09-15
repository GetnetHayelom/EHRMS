using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Users
{
    //[Authorize(Roles = "ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
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
