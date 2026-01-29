using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.ServiceRequest
{
     public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public serviceRequestModel serviceRequestModel { get; set; } = default!;
        public ICollection<serviceRequestHistoryModel>? RequestHistory { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicerequestmodel = await _context.ServiceRequests
                .Include(s => s.ServiceRequestHistoies)
                .Include(s => s.Employment).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.serviceRequestID == id);
            if (servicerequestmodel == null)
            {
                return NotFound();
            }
            else
            {
                serviceRequestModel = servicerequestmodel;
                RequestHistory = serviceRequestModel.ServiceRequestHistoies;
            }
            return Page();
        }
    }
}
