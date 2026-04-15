using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.PenaltyType
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

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
    }
}
