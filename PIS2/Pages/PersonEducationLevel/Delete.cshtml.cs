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

namespace PIS2.Pages.PersonEducationLevel
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
        public personEducationLevelModel personEducationLevelModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personeducationlevelmodel = await _context.PersonEducationLevels
                .Include(pel => pel.personModel)
                .Include(pel => pel.educationLevelModel).FirstOrDefaultAsync(m => m.personEducationLevelID == id);

            if (personeducationlevelmodel == null)
            {
                return NotFound();
            }
            else
            {
                personEducationLevelModel = personeducationlevelmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personeducationlevelmodel = await _context.PersonEducationLevels.FindAsync(id);
            if (personeducationlevelmodel != null)
            {
                personEducationLevelModel = personeducationlevelmodel;
                _context.PersonEducationLevels.Remove(personEducationLevelModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Person/Details", new {id= personEducationLevelModel.personID});
        }
    }
}
