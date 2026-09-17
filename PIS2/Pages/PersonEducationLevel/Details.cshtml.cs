using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.Foundation;
using PIS2.Models.HR;

namespace PIS2.Pages.PersonEducationLevel
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public personEducationLevelModel personEducationLevelModel { get; set; } = default!;
        public List<AttachmentModel> attachments = new List<AttachmentModel>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personeducationlevelmodel = await _context.PersonEducationLevels.FirstOrDefaultAsync(m => m.personEducationLevelID == id);
            if (personeducationlevelmodel == null)
            {
                return NotFound();
            }
            else
            {
                personEducationLevelModel = personeducationlevelmodel;
            }
            // Example: load attachments manually
            

            if (personeducationlevelmodel.AttachementIDs != null && personeducationlevelmodel.AttachementIDs.Any())
            {
                attachments = _context.Attachments
                    .Where(a => personeducationlevelmodel.AttachementIDs.Contains(a.AttachmentID) && a.TableName == "PersonEducationlevels")
                    .ToList();
            }

            return Page();
        }
    }
}
