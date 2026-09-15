using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.ServiceRequest
{
    [Authorize(Roles = "HRMANAGER,HRCLERK")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public serviceRequestModel serviceRequestModel { get; set; } = default!;
        public ICollection<serviceRequestHistoryModel> RequestHistory { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicerequestmodel =  await _context.ServiceRequests
                .Include(s => s.ServiceRequestType)
                .Include(s => s.Employment).ThenInclude(e => e.personModel)
                .Include(s => s.ServiceRequestHistoies)
                .FirstOrDefaultAsync(m => m.serviceRequestID == id);

            if (servicerequestmodel == null)
            {
                return NotFound();
            }
            serviceRequestModel = servicerequestmodel;
            RequestHistory = serviceRequestModel.ServiceRequestHistoies;
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var sr = await _context.ServiceRequests.FirstOrDefaultAsync(s => s.serviceRequestID == serviceRequestModel.serviceRequestID);

            if(sr == null) { TempData["message"] = ("Error", "Record Not Found!"); return Page(); }
            sr.serviceRequestStatus = serviceRequestModel.serviceRequestStatus;
            sr.modifiedBy = User.Identity.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!serviceRequestModelExists(serviceRequestModel.serviceRequestID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = sr.serviceRequestID});
        }

        private bool serviceRequestModelExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.serviceRequestID == id);
        }
    }
}
