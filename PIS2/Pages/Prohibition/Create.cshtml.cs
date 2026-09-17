using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Prohibition
{
    [Authorize(Roles = "HRMANAGER")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }
        [BindProperty]
        public prohibitionModel prohibitionModel { get; set; } = default!;
       
        public employmentModel? Employment { get; set; } = default!;
    
        [BindProperty(SupportsGet = true)]
        public string searchID { get; set; } = default!;
        public List<prohibitionModel>? Prohibitions { get; set; }
        
        public IActionResult OnGet()
        {
            
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            Employment = new employmentModel();
            Prohibitions = new List<prohibitionModel>();

            if (!string.IsNullOrEmpty(searchID))
            {
                Employment = _context.Employments.Include(e => e.personModel).FirstOrDefault(e => e.givenID == searchID);

                if (Employment == null)
                {
                    Employment = new employmentModel();
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                    return Page();
                }
                Prohibitions = _context.Prohibitions.Where(p => p.employmentID == Employment.employmentID).ToList() ?? new List<prohibitionModel>();
                if (Employment.employmentStatus != mainStatus.Active) TempData["SuccessMessage"] = $"Employee is not active!"; return Page();
                
            }

            return Page();
        }

       

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Remove("prohibitionModel.modifiedBy");
            prohibitionModel.prohibitionStatus = mainStatus.Suspended;
            prohibitionModel.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Prohibitions.Add(prohibitionModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
