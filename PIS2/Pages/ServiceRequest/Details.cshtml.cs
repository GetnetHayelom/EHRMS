using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.ServiceRequest
{
     public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
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
                .Include(s => s.ServiceRequestType)
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
