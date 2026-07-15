using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.Company
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public companyModel companyModel { get; set; } = default!;
        public SelectList Manager { get; set; }
        public List<Models.AuditLog> History { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var companymodel =  await _context.Companies
                .Include(c => c.employmentModel).ThenInclude(e => e.personModel).FirstOrDefaultAsync(m => m.companyID == id);
            if (companymodel == null)
            {
                return NotFound();
            }
            companyModel = companymodel;
            // 1. Get the data from the database first
            var managerData = await _context.Employments
                .Include(e => e.personModel)
                .Where(e => e.employmentStatus == mainStatus.Active)
                .Select(e => new
                {
                    EmpID = e.employmentID,
                    // Combine ID and Name for the dropdown display
                    FullName = e.givenID + " - " + e.personModel.personFullName
                })
                .ToListAsync();

            // 2. Assign it to the SelectList
            // Parameters: (Items, DataValueField, DataTextField)
            Manager = new SelectList(managerData, "EmpID", "FullName", companyModel.employmentID);

            ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");

            
            // Fetch Audit Logs for this specific record
            History = await _context.AuditLogs
            .Where(a => a.TableName == "OtherPayments" && a.RecordID == id)
            .OrderByDescending(a => a.ModifiedDate)
            .ToListAsync();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            var existingComp = await _context.Companies.Include(c=> c.Departments).FirstOrDefaultAsync();

            if(existingComp == null)
            {
                TempData["message"] = ("Error", "Company not found");
                return Page();
            }

            if(existingComp.companyStatus == mainStatus.Active && companyModel.companyStatus != mainStatus.Active 
                && existingComp.Departments != null && existingComp.Departments.Any(c => c.departmentStatus == mainStatus.Active))
            {
                TempData["message"] = ("Error", "Active departments exist!");
                return Page();
            }

            ModelState.Remove("companyModel.modifiedBy");
            companyModel.modifiedBy = User.Identity.Name;

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
            _context.Attach(companyModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!companyModelExists(companyModel.companyID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool companyModelExists(int id)
        {
            return _context.Companies.Any(e => e.companyID == id);
        }
        public string FormatAuditValue(string columnName, string value)
        {
            if (string.IsNullOrEmpty(value)) return "None";

            // Handle Booleans
            if (columnName.StartsWith("is"))
            {
                return value.ToLower() == "true" ? "Yes" : "No";
            }

            // Handle Enums (assuming mainStatus is 0=Active, 1=Inactive, etc.)
            if (columnName == "companyStatus")
            {
                if (int.TryParse(value, out int enumValue))
                {
                    // Cast the integer back to the Enum to get the name (e.g., 0 -> "Active")
                    return ((mainStatus)enumValue).ToString();
                }
                return value;
            }

            return value;
        }
        public string GetFriendlyColumnName(string columnName)
        {
            return columnName switch
            {
                "comapnyName" => "Name",
                "companyAlias" => "Code",
                "employmentID" => "Manager ID",
                "addressID" => "Address ID",
                "companyStatus" => "Status",
                _ => columnName
            };
        }
    }
}
