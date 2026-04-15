using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.ServiceRequest
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }
        public SelectList RequestTypes { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
        ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            var req =await _context.ServiceRequestTypes.Where(s => s.serviceRequestTypeStatus == mainStatus.Active).ToListAsync();
            RequestTypes=new SelectList(req, "serviceRequestTypeID", "serviceRequestTypeName");

            return Page();
        }

        [BindProperty]
        public serviceRequestModel serviceRequestModel { get; set; } = default!;
        

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ServiceRequests.Add(serviceRequestModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
