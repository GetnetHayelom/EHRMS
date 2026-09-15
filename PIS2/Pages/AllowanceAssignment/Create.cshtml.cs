using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.AllowanceAssignment
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
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

        public ICollection<allowanceAssignmentModel>? AllowanceAssignments { get; set; }
        
        public async Task<IActionResult> OnGet(int? id)
        {

            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            Employment = new employmentModel();                   

            if (!string.IsNullOrEmpty(searchID))
            {
                Employment =await _context.Employments
                    .Include(e => e.personModel)
                    .Include(e => e.AllowanceAssignments)?.FirstOrDefaultAsync(e => e.givenID == searchID);
               
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
                Employment =await _context.Employments
                    .Include(e => e.personModel)
                    .Include(e => e.AllowanceAssignments)?.FirstOrDefaultAsync(e => e.employmentID == id);

                if (Employment == null || Employment.employmentID == 0)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                    return Page();
                }
            }

            AllowanceAssignments = await _context.AllowanceAssignments.Where(aa=> aa.employmentID == Employment.employmentID).OrderBy(aa=> aa.allowanceAssignmentDate).ToListAsync();
            ViewData["allowanceID"] = new SelectList(_context.Allowances.Where(a => a.allowanceStatus == mainStatus.Active), "allowanceID", "allowanceName");
            return Page();
        }

        [BindProperty]
        public allowanceAssignmentModel allowanceAssignmentModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            var ifExist = await _context.AllowanceAssignments.AnyAsync(aa => aa.allowanceID == allowanceAssignmentModel.allowanceID && aa.employmentID == allowanceAssignmentModel.employmentID);

            if (ifExist)
            {
                TempData["message"] = ("Error", "Allowance assignment duplicate!");
                return Page();
            }

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
