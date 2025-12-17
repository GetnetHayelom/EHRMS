using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Address
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<addressModel> addressModel { get;set; } = default!;
        [BindProperty(SupportsGet =true)]
        public mainStatus? AddressStatus { get; set; }
        public async Task OnGetAsync()
        {
            var address = _context.Addresses.AsQueryable();
            
            if(AddressStatus != null)
            {
                address = address.Where(a => a.addressStatus == AddressStatus);
            }
            addressModel = await _context.Addresses.ToListAsync();
        }
    }
}
