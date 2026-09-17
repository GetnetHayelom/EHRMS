using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Structures
{
    [Authorize(Roles = "HRMANAGER")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        public CreateModel(PISContext context){ _context = context;} 

        [BindProperty] public structureModel Structure { get; set; } = new();
        public List<jobModel> Jobs { get; set; }


        public IActionResult OnGet(int? id)
        {
            Jobs = _context.Jobs.Where(j => j.jobStatus == mainStatus.Active).OrderBy(j => j.jobTitle).ToList();
            Structure = new structureModel();

            // Load companies
            var companyList = _context.Companies
                .Where(c => c.companyStatus == mainStatus.Active)
                .OrderBy(c => c.companyName)
                .ToList();

            int? selectedCompanyId = null;

            if (id != null)
            {
                // Get department and its company
                var dept = _context.Departments
                    .Include(d => d.companyModel)
                    .FirstOrDefault(d => d.departmentID == id && d.departmentStatus == mainStatus.Active);

                if (dept != null)
                {
                    selectedCompanyId = dept.companyID;          // Selected company
                    Structure.departmentID = dept.departmentID;  // Selected department
                }
            }

            // Put selected company into ViewData for dropdown preselect
            ViewData["selectedCompanyID"] = selectedCompanyId;

            // Build Company dropdown
            ViewData["companyID"] = new SelectList(
                companyList,
                "companyID", "companyName",
                selectedCompanyId   // <-- This pre-selects the company
            );

            // Load departments based on selected company
            var departmentList = selectedCompanyId == null
                ? new List<departmentModel>()  // empty list
                : _context.Departments
                    .Where(d => d.companyID == selectedCompanyId &&
                                d.departmentStatus == mainStatus.Active)
                    .OrderBy(d => d.departmentName)
                    .ToList();

            ViewData["departmentID"] = new SelectList(
                departmentList,
                "departmentID", "departmentName",
                Structure.departmentID  // <-- Pre-select department
            );
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dep = _context.Departments.FirstOrDefault(d => d.departmentID == Structure.departmentID && d.departmentStatus == mainStatus.Active);
            var job = _context.Jobs.FirstOrDefault(j => j.jobID == Structure.jobID && j.jobStatus == mainStatus.Active);

            var exists = _context.Structures.Where(s => s.jobID == Structure.jobID && s.departmentID == Structure.departmentID).Any();

            if(dep == null) { return Page(); }
            if (job == null) { return Page(); }
            if (exists) 
            { 
                Jobs = _context.Jobs.Where(j => j.jobStatus == mainStatus.Active).OrderBy(j => j.jobTitle).ToList(); 
                TempData["SuccessMessage"] = "Structure already exists!"; 
                return Page(); 
            }

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
        public async Task<IActionResult> OnGetJobsAsync(string term)
        {
            var jobs = await _context.Jobs
                .Where(j => j.jobTitle.Contains(term))
                .Select(j => new {
                    jobID = j.jobID,
                    jobTitle = j.jobTitle
                })
                .Take(20)
                .ToListAsync();

            return new JsonResult(jobs);
        }
    }
}
