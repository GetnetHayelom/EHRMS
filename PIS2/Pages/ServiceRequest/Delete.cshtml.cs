using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.ServiceRequest
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK")]
    public class DeleteModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DeleteModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public serviceRequestModel serviceRequestModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicerequestmodel = await _context.ServiceRequests.FirstOrDefaultAsync(m => m.serviceRequestID == id);

            if (servicerequestmodel == null)
            {
                return NotFound();
            }
            else
            {
                serviceRequestModel = servicerequestmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicerequestmodel = await _context.ServiceRequests.FindAsync(id);
            if (servicerequestmodel != null)
            {
                serviceRequestModel = servicerequestmodel;
                _context.ServiceRequests.Remove(serviceRequestModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
