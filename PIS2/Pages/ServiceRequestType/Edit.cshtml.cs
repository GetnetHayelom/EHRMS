using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.ServiceRequestType
{
    [Authorize(Roles = "HRADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public serviceRequestTypeModel serviceRequestTypeModel { get; set; } = default!;
   
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicerequestmodel =  await _context.ServiceRequestTypes.FirstOrDefaultAsync(m => m.serviceRequestTypeID == id);

            if (servicerequestmodel == null)
            {
                return NotFound();
            }
            serviceRequestTypeModel = servicerequestmodel;

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var sr = await _context.ServiceRequestTypes.FirstOrDefaultAsync(s => s.serviceRequestTypeID == serviceRequestTypeModel.serviceRequestTypeID);

            if(sr == null) { TempData["message"] = ("Error", "Record Not Found!"); return Page(); }

            ModelState.Remove("serviceRequestTypeModel.modifiedBy");
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            sr.serviceRequestTypeStatus = serviceRequestTypeModel.serviceRequestTypeStatus;
            sr.serviceRequestTypeName = serviceRequestTypeModel.serviceRequestTypeName;
            sr.serviceRequestTypeDescription = serviceRequestTypeModel.serviceRequestTypeDescription;
            sr.modifiedBy = User.Identity.Name;
            sr.modifiedDate = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!serviceRequestModelExists(serviceRequestTypeModel.serviceRequestTypeID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = sr.serviceRequestTypeID});
        }

        private bool serviceRequestModelExists(int id)
        {
            return _context.ServiceRequestTypes.Any(e => e.serviceRequestTypeID == id);
        }
    }
}
