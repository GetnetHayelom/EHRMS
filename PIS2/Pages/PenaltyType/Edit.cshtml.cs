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

namespace PIS2.Pages.PenaltyType
{
    [Authorize(Roles = "MIE\\PMS_HRMANAGER")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public penaltyTypeModel penaltyTypeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltytypemodel =  await _context.PenaltyTypes.FirstOrDefaultAsync(m => m.penaltyTypeID == id);
            if (penaltytypemodel == null)
            {
                return NotFound();
            }
            penaltyTypeModel = penaltytypemodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("penaltyTypeModel.modifiedBy");
            ModelState.Remove("penaltyTypeModel.modifiedDate");

            penaltyTypeModel.modifiedBy = User.Identity?.Name ?? "N\\A";
            penaltyTypeModel.modifiedDate = DateTime.Now;


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

            _context.Attach(penaltyTypeModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!penaltyTypeModelExists(penaltyTypeModel.penaltyTypeID))
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

        private bool penaltyTypeModelExists(int id)
        {
            return _context.PenaltyTypes.Any(e => e.penaltyTypeID == id);
        }
    }
}
