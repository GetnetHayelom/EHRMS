using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.JobRequirement
{
    [Authorize(Roles = "MANAGEMENT")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public List<jobRequirementModel> JobRequests { get; set; } = new List<jobRequirementModel>();
        public SelectList Jobs { get; set; }
        public SelectList EmploymentTypes { get; set; }
        public async Task<IActionResult> OnGet(int? id)
        {
            if (id != null)
            {
                var selectedDepartment =await _context.Departments.FirstOrDefaultAsync(d => d.departmentID ==id);
                
                if (selectedDepartment != null)
                {
                    ViewData["departmentID"] = new SelectList(_context.Departments
                        .Where(d => d.departmentStatus == mainStatus.Active)
                        .OrderBy(d => d.departmentName), "departmentID", "departmentName", selectedDepartment.departmentID);
                }

                JobRequests = await _context.JobRequirements.Where(j => j.departmentID == id).ToListAsync();
               
            }
            else
            {
                ViewData["departmentID"] = new SelectList(_context.Departments.Where(d => d.departmentStatus == mainStatus.Active).OrderBy(d => d.departmentName), "departmentID", "departmentName");
            }
               
            Jobs = new SelectList(_context.Jobs.Where(j => j.jobStatus == mainStatus.Active).Distinct().OrderBy(j => j.jobTitle), "jobID", "jobTitle");
            EmploymentTypes = new SelectList(_context.EmploymentTypes.Where(e => e.employmentTypeStatus == mainStatus.Active).Distinct().OrderBy(e => e.employmentTypeName), "employmentTypeID", "employmentTypeName");
            return Page();
        }

        [BindProperty]
        public jobRequirementModel jobRequirementModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            jobRequirementModel.modifiedBy = User.Identity.Name;
            jobRequirementModel.modifiedDate = DateTime.Now;
            jobRequirementModel.jobRequirementStatus = jobReqStatus.Hold;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.JobRequirements.Add(jobRequirementModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
