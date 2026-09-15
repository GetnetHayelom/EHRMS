using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Pages.Employment
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }
        public personModel Person { get; set; }
        public IActionResult OnGet(int? id)
        {
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (id >0)
            {
                Person = _context.Persons.FirstOrDefault(p => p.personID == id);
                ViewData["personID"] = new SelectList(_context.Persons
                    .Where(p => !_context.Employments
                        .Any(e => e.personID == p.personID && e.employmentStatus == mainStatus.Active))
                    .OrderBy(p => p.personFirstName).ToList(), "personID", "personFullName", id);
            }
            else
            {
                ViewData["personID"] = new SelectList(_context.Persons
                    .Where(p => !_context.Employments
                        .Any(e => e.personID == p.personID && e.employmentStatus == mainStatus.Active))
                    .OrderBy(p => p.personFirstName).ToList(), "personID", "personFullName");
            }
            
            ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            ViewData["employmentMethod"] = new SelectList(_context.EmploymentMethods.Where(e => e.employmentMethodStatus == mainStatus.Active), "employmentMethodID", "employmentMethodName");
            var req = _context.JobRequirements.Include(e => e.JobModel).Where(er => er.jobRequirementStatus != jobReqStatus.Hold || er.jobRequirementStatus != jobReqStatus.Declined).Select(r =>new
            {
                reqID = r.jobRequirementID,
                reqName = r.JobModel.jobTitle
            });

            ViewData["employmentRequest"] = new SelectList(req, "reqID", "reqName");
            return Page();
        }

        [BindProperty]
        public employmentModel employmentModel { get; set; } = default!;
       
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            var empMethod = await _context.EmploymentMethods.AsNoTracking().FirstAsync(e => e.employmentMethodID == employmentModel.employmentMethodID);
            if (empMethod.employmentMethodStatus != mainStatus.Active)
            {
                TempData["message"] = ("Error", $"Employment Method Is Not Active!");
                return Page();
            }
            ViewData["personID"] = new SelectList(_context.Persons, "personID", "personFullName");
            ViewData["employmentTypeID"] = new SelectList(_context.EmploymentTypes, "employmentTypeID", "employmentTypeName");
            var req = _context.JobRequirements.Include(e => e.JobModel).Where(er => er.jobRequirementStatus != jobReqStatus.Hold || er.jobRequirementStatus != jobReqStatus.Declined).Select(r => new
            {
                reqID = r.jobRequirementID,
                reqName = r.JobModel.jobTitle
            });
            ViewData["employmentRequest"] = new SelectList(req, "reqID", "reqName");
            ModelState.Remove("employmentModel.modifiedBy");
            employmentModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                // Log or display errors for debugging
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    TempData["message"] =("Error",$"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return Page();
            }

            _context.Employments.Add(employmentModel);
            await _context.SaveChangesAsync();

            // ✅ Get probation days
            var employmentType = await _context.EmploymentTypes
                .FirstOrDefaultAsync(e => e.employmentTypeID == employmentModel.employmentTypeID);

            int probationDays = employmentType?.probationDays ?? 0;
            var probation = new prohibitionModel
            {
                prohibitionDate = DateTime.Now,
                prohibitionStart = employmentModel.employmentDate,
                prohibitionEnd = employmentModel.employmentDate.AddDays(probationDays),
                employmentID = employmentModel.employmentID,
                prohibitionReason = "Probation Period",
                prohibitionStatus = mainStatus.Active,
                prohibitionType = ProhibitionType.Leave,
                prohibitionRemark = "",
                modifiedBy = User.Identity?.Name
            };
            _context.Prohibitions.Add(probation);
            await _context.SaveChangesAsync();
            return RedirectToPage("/JobPlacement/Create", new {id = employmentModel.employmentID});
        }
    }
}
