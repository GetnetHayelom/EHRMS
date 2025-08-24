using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
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
        public employmentModel Employment { get; set; }= new employmentModel();
        [BindProperty(SupportsGet = true)]
        public string? searchID { get; set; } = default!;
        public string? successMessage { get; set; }
        
        public IActionResult OnGet(int? id)
        {
            Employment = new employmentModel();
            
            

            if (!string.IsNullOrEmpty(searchID))
            {
                Employment =_context.Employments
                    .Include(e => e.personModel)
                    .Include(e => e.AllowanceAssignments)?.FirstOrDefault(e => e.givenID == searchID);
               
                if (Employment == null || Employment.employmentID ==0)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                    return Page();
                }

            }
            else
            {
                Employment = new employmentModel();
            }

            if (id != null)
            {
                Employment = _context.Employments
                    .Include(e => e.personModel)
                    .Include(e => e.AllowanceAssignments)?.FirstOrDefault(e => e.employmentID == id);

                if (Employment == null || Employment.employmentID == 0)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                    return Page();
                }
            }

            ViewData["allowanceID"] = new SelectList(_context.Allowances.Where(a => a.allowanceStatus == mainStatus.Active), "allowanceID", "allowanceName");
            return Page();
        }

        [BindProperty]
        public allowanceAssignmentModel allowanceAssignmentModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("allowanceAssignmentModel.modifiedBy");
            
            allowanceAssignmentModel.modifiedBy = User.Identity.Name;
            allowanceAssignmentModel.allowanceStatus = mainStatus.Suspended;

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

            _context.AllowanceAssignments.Add(allowanceAssignmentModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
