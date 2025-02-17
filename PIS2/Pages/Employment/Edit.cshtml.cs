using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Employment
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public employmentModel employmentModel { get; set; } = default!;
        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;
        [BindProperty]
        public personModel personModel { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var employmentmodel =  await _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.JobPlacements)
                .FirstOrDefaultAsync(m => m.employmentID == id);
            
            if (employmentmodel == null)
            {
                return NotFound();
            }
            
            employmentModel = employmentmodel ?? new employmentModel();
            jobPlacementModel = employmentModel.JobPlacements.FirstOrDefault() ?? new jobPlacementModel();
            personModel = employmentModel.personModel ?? new personModel();
            
            populateViewBags();
            return Page();
        }      

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            populateViewBags();
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(employmentModel).State = EntityState.Modified;

            try
            {
                jobPlacementModel = employmentModel.JobPlacements?.FirstOrDefault() ?? new jobPlacementModel();
                personModel = employmentModel.personModel ?? new personModel();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employmentModelExists(employmentModel.employmentID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return Page();
        }     
        public async Task<IActionResult> OnPostSaveJobPlacement(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(jobPlacementModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!jobPlacementModelExists(jobPlacementModel.jobPlacementID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Page();
        }
        private bool jobPlacementModelExists(int id)
        {
            return _context.JobPlacements.Any(e => e.jobPlacementID == id);
        }
        private bool employmentModelExists(int id)
        {
            return _context.Employments.Any(e => e.employmentID == id);
        }
        public void populateViewBags()
        {
            ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName");
            ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            ViewData["departmentID"] = new SelectList(_context.Departments, "departmentID", "departmentName");
            ViewData["jobID"] = new SelectList(_context.Jobs, "jobID", "jobTitle");
            ViewData["shiftID"] = new SelectList(_context.Shifts, "shiftID", "shiftName");
            ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");

        }
    }
}
