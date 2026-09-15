using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Holiday
{
    [Authorize(Roles = "HRMANAGER,HRADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public holidayModel holidayModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidaymodel =  await _context.Holidays.FirstOrDefaultAsync(m => m.holidayID == id);
            if (holidaymodel == null)
            {
                return NotFound();
            }
            holidayModel = holidaymodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!(User.IsInRole("HRADMIN") || User.IsInRole("HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }

            ModelState.Clear();
            holidayModel.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(holidayModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!holidayModelExists(holidayModel.holidayID))
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

        private bool holidayModelExists(int id)
        {
            return _context.Holidays.Any(e => e.holidayID == id);
        }
    }
}
