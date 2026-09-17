using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;

namespace PIS2.Pages.HRClerck
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public personModel personModel { get; set; } = default!;
        [BindProperty]
        public personModel employments { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personmodel =  await _context.Persons.FirstOrDefaultAsync(m => m.personID == id);
            var employments = await _context.Employments.Where(m => m.personID == id)
                .Include(p => p.JobPlacements)
                .Include(p => p.EmploymentHistories)
                .Include(p => p.Leaves)
                .Include(p => p.AllowanceAssignments)
                .Include(p => p.LoyaltyHistories)
                .Include(p => p.OvertimeRecords).ToListAsync();
            if (personmodel == null)
            {
                return NotFound();
            }
            personModel = personmodel;
           ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressTitle");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(personModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!personModelExists(personModel.personID))
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

        private bool personModelExists(int id)
        {
            return _context.Persons.Any(e => e.personID == id);
        }
    }
}
