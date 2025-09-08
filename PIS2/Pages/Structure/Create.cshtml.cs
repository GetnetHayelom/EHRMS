using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Structures
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        public CreateModel(PISContext context){ _context = context;} 

        [BindProperty] public structureModel Structure { get; set; } = new();

        public IActionResult OnGet()
        {
            ViewData["companyID"] = new SelectList(_context.Companies.Where(c => c.companyStatus == mainStatus.Active), "companyID", "companyName");
            
            ViewData["structureID"] = new SelectList(_context.Structures
                .Include(s => s.departmentModel).ThenInclude(d => d.companyModel)
                .Where(j => j.structureStatus == mainStatus.Active)
                .OrderBy(j => j.departmentModel.departmentName)
                , "structureID", "jobTitle");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Structure.modifiedBy");
            Structure.modifiedBy = User?.Identity?.Name ?? "System";

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return Page();
            }
            if (Structure == null)
                return BadRequest("Structure is missing");

            _context.Structures.Add(Structure);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
        public JsonResult OnGetDepartmentsByCompany(int companyID)
        {
            var departments = _context.Departments
                .Where(d => d.companyID == companyID && d.departmentStatus == mainStatus.Active)
                .OrderBy(d => d.departmentName)
                .Select(d => new { d.departmentID, d.departmentName })
                .ToList();

            return new JsonResult(departments);
        }
        public JsonResult OnGetAvailableJobs(int departmentID)
        {
            var jobs = _context.Jobs
                .Where(j => j.jobStatus == mainStatus.Active &&
                !_context.Structures.Where(s => s.departmentID == departmentID).Select(s => s.jobID).Contains(j.jobID))
                .OrderBy(d => d.jobTitle)
                .Select(d => new { d.jobID, d.jobTitle })
                .ToList();

            return new JsonResult(jobs);
        }
        public JsonResult OnGetReportsTo(int departmentID)
        {
            var structures = _context.Structures
                .Include(s => s.jobModel)
                .Where(s => s.structureStatus == mainStatus.Active && s.departmentID == departmentID)
                .OrderBy(s => s.jobModel.jobTitle)
                .Select(d => new { d.structureID, d.jobModel.jobTitle })
                .ToList();

            return new JsonResult(structures);
        }
    }
}
