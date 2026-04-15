using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.delegation
{
    
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<delegationModel> delegationModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            delegationModel = await _context.Delegations
                .Include(d => d.FromEmployment).ThenInclude(e => e.personModel)
                .Include(d => d.ToEmployment).ThenInclude(e => e.personModel)
                .Where(d => d.delegationStatus != mainStatus.Inactive).ToListAsync();
        }
    }
}
