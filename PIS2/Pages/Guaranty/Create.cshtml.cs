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

            ViewData["employmentID"] = new SelectList(activeEmps, "employmentID", "givenID");
            
            if(id != null) {
                var prsn = await _context.Persons.Where(p => activeEmps.Contains(p.personID)).ToListAsync();
                Person = new SelectList(prsn, "personID", "personFullName", id);
                var gvnID = await _context.Employments.FirstOrDefaultAsync(e => e.personID == id && e.employmentStatus == mainStatus.Active);
                givenID = gvnID?.givenID ?? "";
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
                    var prsn2 = await _context.Persons.OrderBy(p => new { p.personFirstName, p.personFatherName, p.personLastName }).Where(p => activeEmps.Contains(p.personID)).ToListAsync();
                    Person = new SelectList(prsn2, "personID", "personFullName");
                }
                
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
                var prsnID = await _context.Users.FirstOrDefaultAsync(u => u.userName == currentUserName);
                if(prsnID == null)
                {
                    TempData["message"] = ("Error", $"Unknown Person!"); 
                    return Page();
                }
                var emp = await _context.Employments.Where(e => e.personID == prsnID.personID && e.employmentStatus == mainStatus.Active).FirstOrDefaultAsync();
                if (emp == null)
                {
                    TempData["message"] = ("Error", $"Eo Employment Found!");
                    return Page();
                }
                guarantyModel.employmentID = emp.employmentID;

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
            guarantyModel.guarantyStatus = mainStatus.Suspended;
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
