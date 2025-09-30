using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIS2.Models;

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
    }
}
