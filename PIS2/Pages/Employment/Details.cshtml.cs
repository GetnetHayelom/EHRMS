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
        public string Age { get; set; }
        public string Exprience { get; set; }
        public decimal Severance { get; set; }
        public leaveDetail? LeaveDetail { get; set; } = new leaveDetail();
        public List<overtimeRecordModel>? Overtimes { get; set; } = default!;
        public ICollection<leaveModel>? Leaves { get; set; } = new List<leaveModel>();
        public personModel Person { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (!User.IsInRole("MIE\\PMS_HRCLERK") || !User.IsInRole("MIE\\PMS_HRMANAGER") || !User.IsInRole("MIE\\PMS_MANAGEMENT") || _core.IsSelf(User.Identity.Name, id))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

           

            if (!string.IsNullOrEmpty(givenID))
            {
               var emp = await _context.Employments
                    .FirstOrDefaultAsync(e => e.givenID == givenID);

                if (emp != null)
                {
                    Leaves = _context.Leaves.Include(l => l.leaveTypeModel).OrderByDescending(l => l.leaveRequestDate).Where(l => l.employmentID == emp.employmentID).ToList();
                    LeaveDetail = _core.GetLeaveSummary(emp.employmentID);
                    Overtimes = _context.OvertimeRecords.Include(o => o.overtimeModel).OrderByDescending(l => l.overtimeRecordDate).Where(l => l.employmentID == emp.employmentID).ToList();
                    Severance = _core.GetSeverance(emp.employmentID);
                    Person = _context.Persons.Include(p => p.Employments).FirstOrDefault(p => p.personID == emp.personID);
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
                .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel)
                .Include(e =>e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobGradeModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobCategoryModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel).ThenInclude(j => j.jobClassModel)
                .Include(e =>e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel).FirstOrDefaultAsync(m => m.employmentID == id);
            if (employmentmodel == null)
            {
                return NotFound();
            }
            else
            {
                Leaves = _context.Leaves.Include(l => l.leaveTypeModel).OrderByDescending(l => l.leaveRequestDate).Where(l => l.employmentID == employmentmodel.employmentID).ToList();
                LeaveDetail = _core.GetLeaveSummary(employmentmodel.employmentID);
                Overtimes = _context.OvertimeRecords.Include(o => o.overtimeModel).OrderByDescending(l => l.overtimeRecordDate).Where(l => l.employmentID == employmentmodel.employmentID).ToList();
                employmentModel = employmentmodel;
                Severance = _core.GetSeverance(id ?? 0);
                Person = _context.Persons
                    .Include(p => p.Employments)
                    .Include(p => p.addressModel).FirstOrDefault(p => p.personID == employmentModel.personID);
            }
            var currentUser = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name);

            if (currentUser != null && currentUser.personID == employmentModel.personID)
            {
                isSelf = true;
            }

            
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
