using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Guaranty
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;

        public CreateModel(PIS2.Models.PISContext context, Models.Core core)
        {
            _context = context;
            _core = core;
        }
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
         public SelectList Person { get; set; }
        public string modifiedBy { get; set; }
        public List<guarantyModel> GuarantyStatus { get; set; } = new List<guarantyModel>();
        public async Task OnGetAsync(int? id)
        {
            var activeEmps = await _context.Employments.Where(e => e.employmentStatus == mainStatus.Active).Select(e => e.personID).ToListAsync();

            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            
            if(id != null) {
                Person = new SelectList(_context.Persons.Where(p => activeEmps.Contains(p.personID)), "personID", "personFullName", id);
                givenID = _context.Employments.FirstOrDefault(e => e.personID == id && e.employmentStatus == mainStatus.Active)?.givenID ?? "";
                GuarantyStatus = await _context.Guaranties.Include(e => e.Employment).Where(e => e.Employment.personID == id).ToListAsync();
            }
            else if(!String.IsNullOrEmpty(givenID))
            {
                var employment = await _context.Employments.FirstAsync(e => e.employmentStatus == mainStatus.Active && e.givenID == givenID);
                if (employment == null)
                {
                    TempData["message"] = ("Error", "No active employment found for the given employee ID!");
                }
                else
                {
                    Person = new SelectList(_context.Persons.OrderBy(p => new {p.personFirstName, p.personFatherName, p.personLastName}).Where(p => activeEmps.Contains(p.personID)), "personID", "personFullName");
                }
                
                givenID = _context.Employments.FirstOrDefault(e => e.personID == id && e.employmentStatus == mainStatus.Active)?.givenID ?? "";
            }
            
            
        }

        [BindProperty]
        public guarantyModel guarantyModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("guarantyModel.modifiedBy");
            guarantyModel.modifiedBy = User.Identity.Name;
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
                    TempData["message"] = ("Error",$"No employment found with employment ID {givenID}!");
                    return Page();
                }
                if (Employment.employmentStatus == mainStatus.Inactive)
                {
                    TempData["message"] =("Error",$"Employment selected is not active!. {givenID}");
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
