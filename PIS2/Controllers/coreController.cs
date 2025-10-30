using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
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
                .Select(d => new { d.departmentID, d.departmentName })
                .ToList();

            return Ok(departments);
        }

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
                EmployeeName = person.personModel?.personFullName ?? "",
                JobTitle = activeJobPlacement?.jobModel?.jobTitle ?? "",
                Department = department?.departmentName ?? "",
                Company = company?.companyName ?? "",
                IsFound = true
            };

            return Ok(result);
        }

        public class EmployeeData
        {
            public bool IsFound { get; set; }
            public string EmployeeName { get; set; }
            public string JobTitle { get; set; }
            public string Department { get; set; }
            public string Company { get; set; }
            public string WorkLocation { get; set; }
        }
    }
}
