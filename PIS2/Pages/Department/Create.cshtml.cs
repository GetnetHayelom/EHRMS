using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Department
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? id)
        {

            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            ViewData["employmentID"] = new SelectList(_context.Employments.Where(e => e.employmentStatus == mainStatus.Active).OrderBy(e => e.givenID),"employmentID","givenID");

            if (id != null)
            {
                ViewData["companyID"] = new SelectList(_context.Companies, "companyID", "companyName", _context.Companies.FirstOrDefault(c => c.companyID == id)?.companyID);
            }
            else
            {
                ViewData["companyID"] = new SelectList(_context.Companies.OrderBy(c => c.companyName).Where(c=> c.companyStatus == mainStatus.Active), "companyID", "companyName");
            }
                
            ViewData["subAccountID"] = new SelectList(_context.SubAccounts, "subAccountID", "subAccountDescription");
            return Page();
        }

        [BindProperty]
        public departmentModel departmentModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            var compStatus = await _context.Companies.FirstOrDefaultAsync(c => c.companyID == departmentModel.companyID);
            var activeEmps = await _context.JobPlacements.Where(d => d.departmentID == departmentModel.departmentID && d.jobPlacementStatus == mainStatus.Active).ToListAsync();

            if (compStatus == null)
            {
                TempData["message"] = ("Error", "Company not found!");
                return Page();
            }
            if (compStatus.companyStatus != mainStatus.Active)
            {
                TempData["message"] = ("Error", "Company not active!");
                return Page();
            }

            ModelState.Remove("departmentModel.modifiedBy");
            departmentModel.modifiedBy = User.Identity.Name;

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

            _context.Departments.Add(departmentModel);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Department created successfully!";
            return RedirectToPage("Create", new {id=departmentModel.companyID});
        }
    }
}
