using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Department
{
    
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public departmentModel departmentModel { get; set; } = default!;

        public List<departmentHistoryModel> DepartmentHistories { get; set; } =default!;

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

            var departmentmodel =  await _context.Departments
                .Include(d=> d.employmentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.departmentID == id);
            if (departmentmodel == null)
            {
                return NotFound();
            }
            departmentModel = departmentmodel;
            DepartmentHistories = _context.DepartmentHistories.Include(d => d.employmentModel).ThenInclude(e => e.personModel).Where(m => m.departmentID == departmentmodel.departmentID).ToList();

          
            ViewData["companyID"] = new SelectList(_context.Companies, "companyID", "companyName");
            ViewData["subAccountID"] = new SelectList(_context.SubAccounts, "subAccountID", "subAccountDescription");
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
            ModelState.Remove("departmentModel.modifiedBy");
            departmentModel.modifiedBy = User.Identity.Name;

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
                //departmentModel = await _context.Departments
                //.Include(d => d.employmentModel).ThenInclude(e => e.personModel)
                //.FirstOrDefaultAsync(m => m.departmentID == departmentModel.departmentID);
                return Page();
            }
            

                _context.Attach(departmentModel).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!departmentModelExists(departmentModel.departmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
            return RedirectToPage("./Details", new {id = departmentModel.departmentID});
        }

        private bool departmentModelExists(int id)
        {
            return _context.Departments.Any(e => e.departmentID == id);
        }


    }
}
