using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Models;

namespace PIS2.Pages.Guaranty
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public CreateModel(PIS2.Models.PISContext context, Models.Core core)
        {
            _context = context;
            _core = core;
        }
         public personModel Person { get; set; }
        public string modifiedBy { get; set; }
        public IActionResult OnGet()
        {
            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            Person = _context.Persons.FirstOrDefault(p => p.personID == (_context.Users.FirstOrDefault().personID));
            givenID = _context.Employments.OrderByDescending(e => e.employmentDate).FirstOrDefault(e => e.personID == Person.personID).givenID;
            modifiedBy = User.Identity.Name;
            return Page();
        }

        [BindProperty]
        public guarantyModel guarantyModel { get; set; } = default!;
        [BindProperty]
        public string givenID { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserName = User.Identity?.Name;
            if (string.IsNullOrEmpty(givenID))
            {                
                guarantyModel.employmentID = _context.Employments
                    .Where(e => e.personID == _context.Users
                    .FirstOrDefault(u => u.userName == currentUserName).personID && e.employmentStatus == mainStatus.Active)
                    .FirstOrDefault().employmentID;
            }
            else
            {
                var Employment = _core.empByID(givenID);
                if(Employment == null)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {givenID}";
                    return Page();
                }
                if (Employment.employmentStatus == mainStatus.Inactive)
                {
                    TempData["SuccessMessage"] = $"Employment selected is not active. {givenID}";
                    return Page();
                }
                
                guarantyModel.employmentID = Employment.employmentID;
            }
            ModelState.Remove("guarantyModel.modifiedBy");
            guarantyModel.guarantyStatus = mainStatus.Suspended;
            guarantyModel.modifiedBy = currentUserName;
            Console.WriteLine("##### "+ guarantyModel.modifiedBy);
            Console.WriteLine("##### " + User.Identity.Name);
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                }
                return Page();
            }
            try
            {
                serviceRequestModel serReq = new serviceRequestModel();
                
                var employmentID = guarantyModel.employmentID ;
                    

                serReq = new serviceRequestModel
                {
                    employmentID = employmentID,
                    serviceRequestDate = DateTime.Now,
                    serviceRequestStatus = ServiceRequestStatus.Hold,
                    requestedService = ServiceRequestTypes.Guaranty,
                    modifiedBy = currentUserName
                };
                _context.ServiceRequests.Add( serReq );
                await _context.SaveChangesAsync();

                guarantyModel.serviceRequestID = serReq.serviceRequestID;

                _context.Guaranties.Add(guarantyModel);
                await _context.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return RedirectToPage("./Index");
        }
    }
}
