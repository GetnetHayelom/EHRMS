using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.PenaltyType
{
    [Authorize(Roles = "HRMANAGER")]
    public class DeleteModel : PageModel
    {
        private readonly PISContext _context;

        public DeleteModel(PISContext context)
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

            var penaltytypemodel = await _context.PenaltyTypes.FirstOrDefaultAsync(m => m.penaltyTypeID == id);

            if (penaltytypemodel == null)
            {
                return NotFound();
            }
            else
            {
                penaltyTypeModel = penaltytypemodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltytypemodel = await _context.PenaltyTypes.FindAsync(id);
            if (penaltytypemodel != null)
            {
                penaltyTypeModel = penaltytypemodel;
                _context.PenaltyTypes.Remove(penaltyTypeModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
