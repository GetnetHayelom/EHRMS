using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Penalty
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }
        
        [ValidateNever]
        public employmentModel Employment { get; set; }
        public int EmployeeID;
        [BindProperty(SupportsGet = true)]
        public string searchID { get; set; }
        public string message { get; set; }
        public List<penaltyModel>? Penalties { get; set; }
        public SelectList PenaltyTypes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)

        {
            Employment = new employmentModel();
            
            if (!string.IsNullOrEmpty(searchID))
            {
                Employment = _context.Employments?
                    .Include(e => e.personModel)?.FirstOrDefault(e => e.givenID == searchID) ?? new employmentModel();

                Penalties = _context.Penalties
                    .Include(p => p.penaltyTypeModel).Where(p => p.employmentID == Employment.employmentID).ToList();

                if (Employment == null)
                {
                    TempData["message"] = ("Error",$"No employment found with employment ID {searchID}");
                    return Page();
                }
            }

            if (id != null)
            {
                Employment = _context.Employments?
                    .Include(e => e.personModel)?.FirstOrDefault(e => e.employmentID == id) ?? new employmentModel();

                Penalties = _context.Penalties
                    .Include(p => p.penaltyTypeModel).Where(p => p.employmentID == Employment.employmentID).ToList();

                if (Employment == null)
                {
                    TempData["message"] = ("Error", $"No employment found!");
                    return Page();
                }
            }

            PenaltyTypes = new SelectList(_context.PenaltyTypes.Where(p => p.penaltyTypeStatus == mainStatus.Active), "penaltyTypeID", "penaltyName");
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
            penaltyModel.penaltyStatus = penaltyStatus.Hold;

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
                OnGetAsync(penaltyModel.employmentID);
                TempData["message"] = ("Error", "Missing property! Make sure all fields are filled!");
                return Page();
            }
            
            _context.Penalties.Add(penaltyModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
