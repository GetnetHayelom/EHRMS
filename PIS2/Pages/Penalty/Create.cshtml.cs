using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PIS2.Models;

namespace PIS2.Pages.Penalty
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        [BindProperty]
        [ValidateNever]
        public List<employmentModel>? Employments { get; set; } = default!;
        [ValidateNever]
        public employmentModel Employment { get; set; }
        public int EmployeeID;
        [ValidateNever]
        public personModel Person { get; set; } = new personModel();
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string message { get; set; }
        public IActionResult OnGet(int? id)
        {
            Employments = _context.Employments.ToList();
            Employment = new employmentModel();
            if (!string.IsNullOrEmpty(givenID))
            {
                

                Person = _context.Persons.Where(p => p.Employments.Any(e => e.givenID == givenID))
                    .FirstOrDefault();

                if (Person == null)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {givenID}";
                    return Page();
                }
                else
                {
                    TempData["PersonID"] = Person.personID;
                    Employment = _context.Employments
                        .Include(e => e.Leaves).ThenInclude(l => l.leaveTypeModel)?
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)?
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)?
                        .FirstOrDefault(e => e.personID == Person.personID);

                    if (Employment == null)
                    {
                        TempData["SuccessMessage"] = $"No employment found with employment ID {givenID}";
                        return Page();

                    }
                    

                }

            }
            else
            {
                
                Employments = _context.Employments.ToList();
                Person = new personModel();
                Employment = new employmentModel();
                ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
                var leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveGroup == leaveGroup.Absentism && lt.leaveTypeStatus == mainStatus.Active).ToList();
    
            }

            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            ViewData["penaltyTypeID"] = new SelectList(_context.PenaltyTypes.Where(p => p.penaltyTypeStatus == mainStatus.Active), "penaltyTypeID", "penaltyName");
            return Page();
        }

        [BindProperty]
        public penaltyModel penaltyModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("penaltyModel.modifiedBy");
            ModelState.Remove("modifiedBy");
            penaltyModel.modifiedBy = User.Identity.Name;
            //penaltyModel = new penaltyModel();

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
            
            _context.Penalties.Add(penaltyModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
