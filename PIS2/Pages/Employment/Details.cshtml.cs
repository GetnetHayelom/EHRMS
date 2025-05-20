using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Employment
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        public DetailsModel(PIS2.Models.PISContext context, PIS2.Models.Core core)
        {
            _context = context;
            _core = core;
        }
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsUserSelf { get; set; }
        public bool isSelf =false;
        public employmentModel employmentModel { get; set; } = default!;
        public String Age { get; set; }
        public String Exprience { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            
            if (!string.IsNullOrEmpty(givenID))
            {
                var emp = await _context.Employments
                    .FirstOrDefaultAsync(e => e.givenID == givenID);

                if (emp != null)
                {
                    //id = emp.employmentID;
                    // Redirect to the Details page with employmentID
                    return RedirectToPage("Details", new { id = emp.employmentID });
                }

                ErrorMessage = "No employee found with that Given ID.";
            }
            if (id == null)
            {
                return NotFound();
            }
            var employmentmodel = await _context.Employments.Include(e=>e.EmploymentHistories)
                .Include(e=>e.TerminationModel)
                .Include(e=>e.personModel).ThenInclude(p => p.PersonEducationLevels).ThenInclude(pe =>pe.educationLevelModel)
                .Include(e=>e.employmentTypeModel)
                .Include(e => e.Leaves)
                .Include(e =>e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobGradeModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobCategoryModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobClassModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.workSiteModel)
                .Include(e =>e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel).FirstOrDefaultAsync(m => m.employmentID == id);
            if (employmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                employmentModel = employmentmodel;
            }
            var currentUser = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name);

            if (currentUser != null && currentUser.personID == employmentModel.personID)
            {
                isSelf = true;
            }

            //isSelf = _context.Users.FirstOrDefault(u => u.userName.ToLower() == User.Identity.Name!.ToLower()).personID == employmentModel.personID ? true : false;
            Age = _core.GetYearsAndMonths(employmentModel.personModel.personDoB, DateTime.Now).Item1 + " years " +
                _core.GetYearsAndMonths(employmentModel.personModel.personDoB, DateTime.Now).Item2 + " months ";
            if (employmentmodel.employmentStatus == mainStatus.Active)
            {
                Exprience = _core.GetYearsAndMonths(employmentModel.employmentDate, DateTime.Now).Item1 + " years " +
                    _core.GetYearsAndMonths(employmentModel.employmentDate, DateTime.Now).Item2 + " months ";
            }
            else
            {
                Exprience = _core.GetYearsAndMonths(employmentModel.employmentDate, DateTime.Now).Item1 + " years " +
                    _core.GetYearsAndMonths(employmentModel.employmentDate, employmentModel.employmentTerminationDate?? DateTime.MinValue).Item2 + " months ";
            }
            return Page();
        }

       

    }
}
