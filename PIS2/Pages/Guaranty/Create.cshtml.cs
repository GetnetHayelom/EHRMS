using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Services;

namespace PIS2.Pages.Guaranty
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;

        public CreateModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
        
        
        public string modifiedBy { get; set; }
        public List<guarantyModel> GuarantyStatus { get; set; } = new List<guarantyModel>();
        public SelectList Employments { get; set; } 
        public employmentModel Employee { get; set; } = new employmentModel();
        public async Task OnGetAsync(int? id)
        {
            var activeEmps = await _context.Employments.AsNoTracking()
                .Where(e => e.employmentStatus == mainStatus.Active)
                .Select(e => new { Value =e.employmentID, Text=$"{e.givenID}-{e.personModel.personFullName}"}).ToListAsync();

            Employments = new SelectList(activeEmps, "Value", "Text");
            
            if(id != null) {
                Employee = await _context.Employments.Include(e => e.personModel).FirstOrDefaultAsync(e => e.employmentID==id);
                GuarantyStatus = await _context.Guaranties.Where(e => e.employmentID == id).ToListAsync();
            }
        }

        [BindProperty]
        public guarantyModel guarantyModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("guarantyModel.modifiedBy");
            guarantyModel.modifiedBy = User.Identity.Name;
            var currentUserName = User.Identity?.Name;
                        
            guarantyModel.guarantyStatus = mainStatus.Suspended;
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                }
                return Page();
            }
            try
            {
                _context.Guaranties.Add(guarantyModel);
                await _context.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return RedirectToPage("./Details", new { id =guarantyModel.guarantyID});
        }
    }
}
