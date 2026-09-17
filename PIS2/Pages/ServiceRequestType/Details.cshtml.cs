using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.ServiceRequestType
{
     public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public serviceRequestTypeModel serviceRequestTypeModel { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicerequestmodel = await _context.ServiceRequestTypes
                .FirstOrDefaultAsync(m => m.serviceRequestTypeID == id);
            if (servicerequestmodel == null)
            {
                return NotFound();
            }
            else
            {
                serviceRequestTypeModel = servicerequestmodel;
            }
            return Page();
        }
    }
}
