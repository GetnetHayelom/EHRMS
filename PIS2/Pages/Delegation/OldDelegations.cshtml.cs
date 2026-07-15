using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.Delegation
{
    public class OldDelegationsModel : PageModel
    {
        private readonly PISContext _context;

        public OldDelegationsModel(PISContext context)
        {
            _context = context;
        }

        public IList<delegationModel> delegationModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            delegationModel = await _context.Delegations
                .Include(d => d.FromEmployment).ThenInclude(e => e.personModel)
                .Include(d => d.ToEmployment).ThenInclude(e => e.personModel)
                .Where(d => d.delegationStatus == mainStatus.Inactive).ToListAsync();
        }
    }
}
