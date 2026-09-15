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

namespace PIS2.Pages.Allowance
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public allowanceModel allowanceModel { get; set; } = default!;
        public SelectList EarningType { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!User.IsInRole("HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id == null)
            {
                return NotFound();
            }

            var allowancemodel =  await _context.Allowances.Include(e => e.EarningType).FirstOrDefaultAsync(m => m.allowanceID == id);
            if (allowancemodel == null)
            {
                return NotFound();
            }

            var earn = await _context.EarningTypes.Where(e => e.earningTypeStatus == mainStatus.Active).ToListAsync();
            EarningType = new SelectList(earn, "earningTypeID", "earningTypeName");
            allowanceModel = allowancemodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Remove("allowanceModel.modifiedBy");
            allowanceModel.modifiedBy = User.Identity.Name;

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
            _context.Attach(allowanceModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!allowanceModelExists(allowanceModel.allowanceID))
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

        private bool allowanceModelExists(int id)
        {
            return _context.Allowances.Any(e => e.allowanceID == id);
        }
    }
}
