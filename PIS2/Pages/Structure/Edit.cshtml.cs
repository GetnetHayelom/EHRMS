using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Structures
{
    [Authorize(Roles = "HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        public EditModel(PISContext context) => _context = context;

        [BindProperty] public structureModel Structure { get; set; } = new();
        public List<structureHistoryModel> Structures { get; set; }
        public IActionResult OnGet(int? id)
        {
            ViewData["companyID"] = new SelectList(_context.Companies.Where(c => c.companyStatus == mainStatus.Active), "companyID", "companyName");
            Structure = _context.Structures
                .Include(s => s.ReportsTo).ThenInclude(s => s.departmentModel)?.ThenInclude(d => d.companyModel)
                .Include(s => s.ReportsTo).ThenInclude(s => s.jobModel)
                .Include(s => s.departmentModel)?.ThenInclude(d => d.companyModel)?
                .FirstOrDefault(x => x.structureID == id);
            if(Structure == null)
            {
                return NotFound();
            }

            Structures =_context.StructureHistories
                .Include(sh=> sh.structureModel).ThenInclude(s => s.ReportsTo).ThenInclude(r => r.departmentModel).ThenInclude(d => d.companyModel)
                .Where(sh => sh.structureID == id).ToList();

            ViewData["departmentID"] = new SelectList(_context.Departments, "departmentID", "departmentName");
            ViewData["jobID"] = new SelectList(_context.Jobs, "jobID", "jobTitle");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Structure.modifiedBy");
            Structure.modifiedBy = User.Identity.Name;

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

            _context.Attach(Structure).State = EntityState.Modified;
           
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
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
        public JsonResult OnGetDepartmentsByCompany(int companyID)
        {
            var departments = _context.Departments
                .Where(d => d.companyID == companyID && d.departmentStatus == mainStatus.Active)
                .OrderBy(d => d.departmentName)
                .Select(d => new { d.departmentID, d.departmentName })
                .ToList();

            return new JsonResult(departments);
        }
    }
}
