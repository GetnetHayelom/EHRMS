using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Termination
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string ErrorMessage { get; set; }
        [BindProperty]
        public employmentModel Employment { get; set; }
        int? EmpID { get; set; }
        public IActionResult OnGet(int? id)
        {
            Employment = new employmentModel();
            if (!string.IsNullOrEmpty(givenID))
            {
                var emp = _context.Employments
                    .FirstOrDefault(e => e.givenID == givenID);

                if (emp != null)
                {

                    // Redirect to the Details page with employmentID
                    id = emp.employmentID;
                    Console.WriteLine("############## The ID is == " + id);
                    employmentModel = _context.Employments.Include(e => e.personModel).FirstOrDefault(e => e.employmentID == id);
                    return Page();
                    //return RedirectToPage("Create", new { id = emp.employmentID });
                }

                ErrorMessage = "No employee found with that Given ID.";
            }
            if(id != null && id != 0)
            {
                employmentModel = _context.Employments.Include(e => e.personModel)
                    .Where(e => e.employmentStatus == mainStatus.Active && e.TerminationModel == null)
                    .FirstOrDefault(e => e.employmentID == id);
                givenID = employmentModel.givenID;
            }
            
            return Page();
        }

        [BindProperty]
        public terminationModel terminationModel { get; set; } = default!;
        public employmentModel employmentModel { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            ModelState.Remove("terminationModel.modifiedBy");
            ModelState.Remove("modifiedBy");
            ModelState.Remove("givenID");
            terminationModel.modifiedBy = User.Identity.Name;
            terminationModel.terminationStatus = terminationStatus.Hold;
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        
                    }
                }
                Console.WriteLine("===ID IS===" +id);
                
                return Page();
            }
            employmentModel = await _context.Employments.Include(e => e.personModel)?.FirstOrDefaultAsync(e => e.employmentID == id);
            Console.WriteLine("===Emp  ID IS===" + terminationModel.employmentID);
            if (employmentModel == null)
            {
                return NotFound();
            }
            
            _context.Terminations.Add(terminationModel);
            await _context.SaveChangesAsync();

            
            return RedirectToPage("./Edit", new { id =employmentModel.employmentID});
        }
    }
}
