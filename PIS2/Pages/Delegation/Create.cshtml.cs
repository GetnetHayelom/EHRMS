using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.delegation
{
    [Authorize(Roles = "MIE\\PMS_MANAGEMENT,MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
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
        public SelectList Employments { get; set; }
        public List<delegationModel> myDelegations { get; set; } = new List<delegationModel>();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            
            await LoadHelper();

            delegationModel = new delegationModel();
            if (id != null)
            {
                var delegator = await _context.Employments.Include(e => e.personModel).Where(e => e.employmentStatus == mainStatus.Active).FirstOrDefaultAsync(e => e.employmentID ==id);
                myDelegations = await _context.Delegations
               .Include(d => d.FromEmployment).ThenInclude(e => e.personModel)
               .Include(d => d.ToEmployment).ThenInclude(e => e.personModel)
               .Where(d => d.delegationFrom == delegator.employmentID).ToListAsync();

                delegationModel.delegationFrom = delegator.employmentID;
                delegationModel.FromEmployment = delegator;
            }
            
            return Page();
        }

        [BindProperty]
        public delegationModel delegationModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var exists = await _context.Delegations.Where(d => d.delegationFrom == delegationModel.delegationFrom
            && d.delegationTo == delegationModel.delegationTo && d.delegationScope == delegationModel.delegationScope
            && d.delegationStatus == mainStatus.Active).AnyAsync();

            if (exists) { TempData["message"] = ("Error","Delegation Already Exists!");
               
                await LoadHelper(); return Page(); }

            if (delegationModel.delegationFrom == delegationModel.delegationTo)
            {
                ModelState.AddModelError("Error", "Delegator and Delegatee can not be the same.");
                await LoadHelper();
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
        private async Task LoadHelper()
        {
            ModelState.AddModelError("Error", "Delegator and Delegatee can not be the same.");
            var emps = await _context.Employments
            .Include(e => e.personModel)
            .Where(e => e.employmentStatus == mainStatus.Active)
            .Select(e => new
            {
                EmpID = e.employmentID,
                // Combine ID and Name for the dropdown display
                FullName = e.givenID + " - " + e.personModel.personFullName
            })
            .ToListAsync();
            Employments = new SelectList(emps, "EmpID", "FullName");
        }
    }
}
