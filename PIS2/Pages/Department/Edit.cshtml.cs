using Microsoft.AspNetCore.Authorization;
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
    
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public departmentModel departmentModel { get; set; } = default!;
        public SelectList Manager { get; set; }
        public List<departmentHistoryModel> DepartmentHistories { get; set; } =default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var departmentmodel =  await _context.Departments
                .Include(d=> d.employmentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.departmentID == id);
            if (departmentmodel == null)
            {
                return NotFound();
            }
            departmentModel = departmentmodel;
            DepartmentHistories = _context.DepartmentHistories.Include(d => d.employmentModel).ThenInclude(e => e.personModel).Where(m => m.departmentID == departmentmodel.departmentID).ToList();

            // 1. Get the data from the database first
            var managerData = await _context.Employments
                .Include(e => e.personModel)
                .Where(e => e.employmentStatus == mainStatus.Active)
                .Select(e => new
                {
                    EmpID = e.employmentID,
                    // Combine ID and Name for the dropdown display
                    FullName = e.givenID + " - " + e.personModel.personFullName
                })
                .ToListAsync();

            // 2. Assign it to the SelectList
            // Parameters: (Items, DataValueField, DataTextField)
            Manager = new SelectList(managerData, "EmpID", "FullName", departmentModel.employmentID);

            ViewData["companyID"] = new SelectList(_context.Companies, "companyID", "companyName");
            ViewData["subAccountID"] = new SelectList(_context.SubAccounts, "subAccountID", "subAccountDescription");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
                return RedirectToPage("/Shared/AccessDenied");

            ModelState.Remove("departmentModel.modifiedBy");
            departmentModel.modifiedBy = User.Identity.Name;

            var existingDep = await _context.Departments
                .FirstOrDefaultAsync(d => d.departmentID == departmentModel.departmentID);

            if (existingDep == null)
            {
                TempData["message"] = ("Error","Department not found!");
                return Page();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.companyID == departmentModel.companyID);

            if (company == null)
            {
                TempData["message"] = ("Error","Company not found!");
                return Page();
            }

            var hasActiveEmps = await _context.JobPlacements.AnyAsync(
                d => d.departmentID == departmentModel.departmentID &&
                     d.jobPlacementStatus == mainStatus.Active);

            if (hasActiveEmps &&
                existingDep.departmentStatus == mainStatus.Active &&
                departmentModel.departmentStatus != mainStatus.Active)
            {
                TempData["message"] = ("Error","Department has active job placements!");
                return Page();
            }

            if (existingDep.departmentStatus != mainStatus.Active &&
                departmentModel.departmentStatus == mainStatus.Active &&
                company.companyStatus != mainStatus.Active)
            {
                TempData["message"] = ("Error","Company is not active!");
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            existingDep.departmentName = departmentModel.departmentName;
            existingDep.departmentStatus = departmentModel.departmentStatus;
            existingDep.companyID = departmentModel.companyID;
            existingDep.employmentID = departmentModel.employmentID;
            existingDep.modifiedBy = User.Identity.Name;
          
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = existingDep.departmentID });
        }


        private bool departmentModelExists(int id)
        {
            return _context.Departments.Any(e => e.departmentID == id);
        }


    }
}
