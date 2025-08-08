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
        [BindProperty]
        public bool EmpSelected { get; set; } = true;
        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (!string.IsNullOrEmpty(givenID) && id == null)
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
                EmpSelected = false;
                return Page();
            }
            
            var employmentmodel =  await _context.Employments
                .Include(e => e.personModel).ThenInclude(p => p.addressModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel)
                .FirstOrDefaultAsync(m => m.employmentID == id);

            
            Experiences = new List<experienceModel>();
            PersonEducationLevels = new List<personEducationLevelModel>();
            
            if (employmentmodel == null)
            {
                EmpSelected = false;
                return Page();
            }
            
            employmentModel = employmentmodel ?? new employmentModel();
            jobPlacementModel = employmentModel.JobPlacements?.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active) ?? new jobPlacementModel();
            personModel = _context.Persons.Include(p => p.addressModel).FirstOrDefault( p=> p.personID == employmentModel.personID) ?? new personModel();
            PersonEducationLevels = _context.PersonEducationLevels.Where(p=>p.personID == employmentModel.personID).ToList();
            Experiences = _context.Experiences.Where(e => e.personID == employmentModel.personID).ToList();
            if(jobPlacementModel.jobPlacementID != 0)
            {
                Experiences.Add(new experienceModel
                {
                    jobTitle = jobPlacementModel.jobModel?.jobTitle,
                    jobDepartment = "MIE",
                    jobSalary = jobPlacementModel.jobPlacementSalary,
                    experienceEndDate = DateTime.Now,
                    experienceStartDate =jobPlacementModel.jobPlacementDate,
                    experienceType = Ex_In.Internal
                });
            }
            else
            {
                //jobPlacementModel = _context.JobPlacements.Include(jp => jp.jobModel).OrderByDescending(jp=> jp.jobPlacementDate).FirstOrDefault(jp => jp.employmentID == id) ?? new jobPlacementModel();
            }
            populateViewBags();
            return Page();
        }      

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostUpdateEmploymentAsync()
        {
            populateViewBags();
            ModelState.Clear();
            employmentModel.modifiedBy = User.Identity.Name;
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
            return RedirectToPage("Edit", new {id=EmpID});
        }
        public async Task<IActionResult> OnPostUpdatePerson(int id)
        {

            personModel existing =new personModel();
            existing = _context.Persons.AsNoTracking().FirstOrDefault(p => p.personID == personModel.personID);
            if (IsSamePerson(personModel, existing))
            {
                return RedirectToPage("Edit", new { id = getEmpIDFromPerson(personModel.personID) });
            }
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
                return RedirectToPage("Edit", new { id = getEmpIDFromPerson(personModel.personID) });
            }
            catch (DbUpdateConcurrencyException)
            {               
                throw;               
            }
            
               
            //return RedirectToPage("Edit", new { id = EmpID });
        }
        public async Task<IActionResult> OnPostSaveJobPlacement(int id)
        {
            populateViewBags();
            ModelState.Clear();
            jobPlacementModel.modifiedBy = User.Identity.Name;
            Console.WriteLine("####### Post job Plac is Called " + id);
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

            var currentJP = await _context.JobPlacements.AsNoTracking().FirstOrDefaultAsync( j => j.jobPlacementID == jobPlacementModel.jobPlacementID);
            if (currentJP == null)
            {
                currentJP = new jobPlacementModel();
                jobPlacementModel.jobPlacementID = 0;
            }

            if (currentJP.jobID != jobPlacementModel.jobID)
            {
                jobPlacementModel.jobPlacementID = 0;
                _context.JobPlacements.Add(jobPlacementModel);
            }
            else
            {
                _context.Attach(jobPlacementModel).State = EntityState.Modified;
            }

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
            return RedirectToPage("Edit", new {id = jobPlacementModel.employmentID});
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
                return RedirectToPage("Edit", new { id = getEmpIDFromPerson(experienceModel.personID) });
            }

            _context.Experiences.Add(experienceModel);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("Edit", new { id = getEmpIDFromPerson(experienceModel.personID) });
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }
            
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
                
                Console.WriteLine("###############111_____" + EmpID);
                return RedirectToPage("Edit", new { id = getEmpIDFromPerson(personEducationLevelModel.personID) });
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }
            EmpID = employmentModel.employmentID;
            Console.WriteLine("###############_____" + EmpID);
            return RedirectToPage("Edit", new {id = EmpID});
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
            ViewData["departmentID"] = new SelectList(_context.Departments.Where(d => d.departmentStatus ==mainStatus.Active), "departmentID", "departmentName");
            ViewData["jobID"] = new SelectList(_context.Jobs.Where(j => j.jobStatus == mainStatus.Active), "jobID", "jobTitle");
            ViewData["jobStepID"] = new SelectList(_context.JobSteps, "jobStepID", "jobStepName");
            ViewData["shiftID"] = new SelectList(_context.Shifts.Where(s => s.shiftStatus == mainStatus.Active), "shiftID", "shiftName");
            ViewData["addressID"] = new SelectList(_context.Addresses.Where(a => a.addressStatus == mainStatus.Active), "addressID", "addressFormatted");
            ViewData["educationLevelID"] = new SelectList(_context.EducationLevels, "educationLevelID", "educationLevelName");
            ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");

        }
        private bool IsSamePerson(personModel newPerson, personModel existingPerson)
        {
            return
                newPerson.personFirstName == existingPerson.personFirstName &&
                newPerson.personFatherName == existingPerson.personFatherName &&
                newPerson.personLastName == existingPerson.personLastName &&
                newPerson.personIDType == existingPerson.personIDType &&
                newPerson.personIDNumber == existingPerson.personIDNumber &&
                newPerson.personPhoneNumber == existingPerson.personPhoneNumber &&
                newPerson.personDoB == existingPerson.personDoB &&
                newPerson.addressID == existingPerson.addressID &&
                newPerson.personGender == existingPerson.personGender &&
                newPerson.personEmailAddress == existingPerson.personEmailAddress;

            // Add other fields as necessary
        }
        private int getEmpIDFromPerson(int personID)
        {
            var empID = 0;
            empID = _context.Employments.FirstOrDefault(e => e.personID == personID && e.employmentStatus == mainStatus.Active).employmentID;
            if (empID == 0 || empID == null)
            {
                empID = _context.Employments.OrderBy(e => e.employmentDate).FirstOrDefault(e => e.personID == personID).employmentID;
            }

            return empID;
        }
        
    }
}
