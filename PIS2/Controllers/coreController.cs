using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using static PIS2.Pages.Forms.SLAccessModel;

namespace PIS2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class coreController : ControllerBase
    {
        private readonly PISContext _context;

        public coreController(PISContext context)
        {
            _context = context;
        }

        [HttpGet("DepartmentsByCompany/{companyID}")]
        public IActionResult GetDepartmentsByCompany(int companyID)
        {
            var departments = _context.Departments
                .Where(d => d.companyID == companyID && d.departmentStatus == mainStatus.Active)
                .OrderBy(d => d.departmentName)
                .Select(d => new { d.departmentID, d.departmentName }).Distinct()
                .ToList();

            return Ok(departments);
        }

        /// <summary>
        /// Get Employee Info by given ID
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        // ✅ Employee Lookup API
        [HttpGet("EmployeeLookup/{employeeId}")]
        public IActionResult GetEmployeeLookup(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return Ok(new EmployeeData { IsFound = false });
            }

            var person = _context.Employments
                .Where(p => p.givenID == employeeId)
                .Include(p => p.personModel)
                .Include(e => e.JobPlacements
                    .Where(jp => jp.jobPlacementStatus == mainStatus.Active))
                    .ThenInclude(jp => jp.departmentModel)
                        .ThenInclude(d => d.companyModel)
                .Include(e => e.JobPlacements
                    .Where(jp => jp.jobPlacementStatus == mainStatus.Active))
                    .ThenInclude(jp => jp.jobModel)
                .FirstOrDefault();

            if (person == null)
            {
                return Ok(new EmployeeData { IsFound = false });
            }

            var activeJobPlacement = person.JobPlacements?.FirstOrDefault();

            if (activeJobPlacement == null)
            {
                return Ok(new EmployeeData { IsFound = false });
            }

            var department = activeJobPlacement.departmentModel;
            var company = department?.companyModel;

            var result = new EmployeeData
            {
                EmploymentID =person.employmentID,
                EmployeeName = person.personModel?.personFullName ?? "",
                JobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "",
                Department = department?.departmentName ?? "",
                Company = company?.companyName ?? "",
                EmploymentStatus = person.employmentStatus,
                IsFound = true
            };

            return Ok(result);
        }

        public class EmployeeData
        {
            public bool IsFound { get; set; }
            public int? EmploymentID { get; set; }
            public string EmployeeName { get; set; }
            public string JobTitle { get; set; }
            public string Department { get; set; }
            public string Company { get; set; }
            public string WorkLocation { get; set; }
            public mainStatus? EmploymentStatus { get; set; }
        }

        [HttpGet("JobsByJobClass/{jobClassID}")]
        public IActionResult GetJobsByJobClass(int jobClassID)
        {
            var jobs = _context.Jobs
                .Where(d => d.jobClassID == jobClassID)
                .OrderBy(d => d.jobTitle)
                .Select(d => new { d.jobID, d.jobTitle })
                .ToList();

            return Ok(jobs);
        }

        [HttpGet("JobsByJobTitle/{jobTitle}")]
        public IActionResult GetJobsByJobTitle(string jobTitle)
        {
            var jobs = _context.Jobs
                .Where(d => d.jobTitle.Contains(jobTitle))
                .OrderBy(d => d.jobTitle)
                .Select(d => new { d.jobID, d.jobTitle })
                .ToList();

            return Ok(jobs);
        }

        /// <summary>
        /// Returns job grade id of the given job
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        [HttpGet("GetJobGrade/{jobID}")]
        public IActionResult GetJobGradeByJobID(int jobID)
        {
            var jobGradeID = _context.Jobs
                .FirstOrDefault(d => d.jobID == jobID)?.jobGradeID ?? 0;

            return Ok(jobGradeID);
        }

        /// <summary>
        /// Returns job stepID and jobStep Name for the given job grade
        /// </summary>
        /// <param name="jobGradeID"></param>
        /// <returns></returns>
        [HttpGet("GetJobSteps/{jobGradeID}")]
        public IActionResult GetJobStepsByJobGrade(int jobGradeID)
        {
            var jobSteps = _context.JobSteps
                .Where(d => d.jobGradeID == jobGradeID && d.jobStepStatus == mainStatus.Active)
                .OrderBy(d => d.jobStepName)
                .Select(d => new { d.jobStepID, d.jobStepName })
                .ToList();

            return Ok(jobSteps);
        }

        /// <summary>
        /// Returns the salary of a given job step
        /// </summary>
        /// <param name="jobStepID"></param>
        /// <returns></returns>
        [HttpGet("GetJobSalary/{jobStepID}")]
        public IActionResult GetSalaryByJobStep(int jobStepID)
        {

            var salary = _context.JobSteps
                .FirstOrDefault(d => d.jobStepID == jobStepID && d.jobStepStatus == mainStatus.Active)?
                .jobStepSalary ?? 0;

            return Ok(salary);
        }
    }
}
