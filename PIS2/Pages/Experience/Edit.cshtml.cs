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

namespace PIS2.Pages.Experience
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK")]
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public experienceModel experienceModel { get; set; } = default!;

        [BindProperty]
        public personModel personModel { get; set; } = default!;
        [BindProperty]
        public List<experienceModel> Experiences { get; set; } = default!;
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var experiencemodel =  await _context.Experiences.FirstOrDefaultAsync(m => m.experienceID == id);
            if (experiencemodel == null)
            {
                return NotFound();
            }

            
            experienceModel = experiencemodel;
            Experiences = _context.Experiences.Where(e => e.personID == experienceModel.personID).ToList();
            personModel = await _context.Persons.FirstOrDefaultAsync(m => m.personID == experienceModel.personID);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Clear();
            experienceModel.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Page();
            }


            _context.Attach(experienceModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!experienceModelExists(experienceModel.experienceID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Experiences = _context.Experiences.Where(e => e.personID == experienceModel.personID).ToList();
            personModel = await _context.Persons.FirstOrDefaultAsync(m => m.personID == experienceModel.personID);
            TempData["SuccessMessage"] = "Updates saved successfully!";
            return Page();
        }

        private bool experienceModelExists(int id)
        {
            return _context.Experiences.Any(e => e.experienceID == id);
        }
    }
}
