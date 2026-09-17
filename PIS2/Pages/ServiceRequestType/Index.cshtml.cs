using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.ServiceRequestType
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public List<serviceRequestTypeModel> serviceRequestTypeModel { get; set; } = default!;
        
       

        public async Task OnGetAsync()
        {
            serviceRequestTypeModel = await _context.ServiceRequestTypes
                .ToListAsync();

        }

        
    }
}
