using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.delegation
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        [BindProperty]
        public employmentModel delegator { get; set; }
        public employmentModel delegatee { get; set; }
        public List<employmentModel> Employments { get; set; } =new List<employmentModel>();
        public IActionResult OnGet(int? id)
        {
            Employments = _context.Employments.Include(e => e.personModel).Where(e => e.employmentStatus == mainStatus.Active)
                    .OrderBy(e => e.personModel.personFirstName).ThenBy(e => e.personModel.personFatherName).ThenBy(e => e.personModel.personLastName)
                    .ToList() ?? new List<employmentModel>();
            if (id != null)
            {
                delegationModel.delegationFrom = id ?? 0;
                delegationModel.FromEmployment = _context.Employments.FirstOrDefault(e => e.employmentID ==id);

            }
            if(User.IsInRole("MIE\\PMS_MANAGEMENT") || User.IsInRole("MIE\\PMS_HRCLERK") || User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                 

                //ViewData["delegationFrom"] = new SelectList(employments, "employmentID", "personName");
                //ViewData["delegationTo"] = new SelectList(employments, "employmentID", "personName");
            }
            else
            {
                return NotFound();
            }   
     
            
            return Page();
        }

        [BindProperty]
        public delegationModel delegationModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if(delegationModel.delegationFrom == delegationModel.delegationTo)
            {
                ModelState.AddModelError("", "Delegator and Delegatee can not be the same.");
                Employments = _context.Employments.Include(e => e.personModel).Where(e => e.employmentStatus == mainStatus.Active)
                    .OrderBy(e => e.personModel.personFirstName).ThenBy(e => e.personModel.personFatherName).ThenBy(e => e.personModel.personLastName)
                    .ToList() ?? new List<employmentModel>();
                return Page();
            }
            ModelState.Remove("delegationModel.modifiedBy");
            ModelState.Remove("delegationModel.delegationStatus");
            ModelState.Remove("givenID");
            ModelState.Remove("modifiedBy");

            delegationModel.modifiedBy = User.Identity.Name;
            delegationModel.delegationStatus = mainStatus.Active;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return Page();
            }

            _context.Delegations.Add(delegationModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
