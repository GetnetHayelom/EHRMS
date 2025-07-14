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
        [BindProperty]
        public experienceModel experienceModel { get; set; } = default!;
        [BindProperty]
        public personEducationLevelModel personEducationLevelModel { get; set; } = default!;
        [BindProperty]
        public List<experienceModel> Experiences { get; set; } = default!;
        [BindProperty]
        public List<personEducationLevelModel> PersonEducationLevels { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string ErrorMessage { get; set; }
        int ? EmpID { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (!string.IsNullOrEmpty(givenID))
            {
                var emp = await _context.Employments
                    .FirstOrDefaultAsync(e => e.givenID == givenID);

                if (emp != null)
                {
                    
                    // Redirect to the Details page with employmentID
                    Console.WriteLine("############## The ID is == " + id);
                    return RedirectToPage("Edit", new { id = emp.employmentID });
                }

                ErrorMessage = "No employee found with that Given ID.";
            }
            if (id == null)
            {
                id = EmpID;
                if(id == null)
                {
                    return NotFound();
                }
                
            }
            
            var employmentmodel =  await _context.Employments
                .Include(e => e.personModel)
                .Include(e => e.JobPlacements)
                .FirstOrDefaultAsync(m => m.employmentID == id);

            
            Experiences = new List<experienceModel>();
            PersonEducationLevels = new List<personEducationLevelModel>();
            
            if (employmentmodel == null)
            {
                return NotFound();
            }
            
            employmentModel = employmentmodel ?? new employmentModel();
            jobPlacementModel = employmentModel.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active) ?? new jobPlacementModel();
            personModel = employmentModel.personModel ?? new personModel();
            PersonEducationLevels = _context.PersonEducationLevels.Where(p=>p.personID == employmentModel.personID).ToList();
            Experiences = _context.Experiences.Where(e => e.personID == employmentModel.personID).ToList();
            populateViewBags();
            return Page();
        }      

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostUpdateEmploymentAsync()
        {
            populateViewBags();
            ModelState.Clear();
            Console.WriteLine("####### Post is Called");
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
            EmpID = employmentModel.employmentID;
            return Page();
        }
        public async Task<IActionResult> OnPostUpdatePerson(int id)
        {
            ModelState.Clear();
            personModel.modifiedBy = User.Identity.Name;

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

            _context.Attach(personModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               
                throw;
               
            }
            EmpID = employmentModel.employmentID;
            return Page();
        }
        public async Task<IActionResult> OnPostSaveJobPlacement(int id)
        {
            populateViewBags();
            //ModelState.Clear();
            Console.WriteLine("####### Post job Plac is Called");
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
            EmpID = employmentModel.employmentID;
            return Page();
        }
        public async Task<IActionResult> OnPostAddExperience(int id)
        {
            ModelState.Clear();
            experienceModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                }
                return RedirectToPage("Edit", new { id = employmentModel.employmentID });
            }

            _context.Experiences.Add(experienceModel);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }
            EmpID = employmentModel.employmentID;
            return Page();
        }
        public async Task<IActionResult> OnPostAddEducation(int id)
        {
            ModelState.Clear();
            personEducationLevelModel.modifiedBy = User.Identity.Name;
            personEducationLevelModel.modifiedDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.PersonEducationLevels.Add(personEducationLevelModel);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }
            EmpID = employmentModel.employmentID;
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
            ViewData["jobStepID"] = new SelectList(_context.JobSteps, "jobStepID", "jobStepName");
            ViewData["shiftID"] = new SelectList(_context.Shifts, "shiftID", "shiftName");
            ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");
            ViewData["educationLevelID"] = new SelectList(_context.EducationLevels, "educationLevelID", "educationLevelName");

        }
    }
}
