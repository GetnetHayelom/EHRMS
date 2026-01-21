using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Penalty
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public penaltyModel penaltyModel { get; set; } = default!;
        public List<penaltyHistoryModel>? penaltyHistory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penaltymodel =  await _context.Penalties
                .Include(p => p.employmentModel).ThenInclude(e => e.personModel)
                .Include(e => e.penaltyTypeModel)
                .FirstOrDefaultAsync(m => m.penaltyID == id);

            if (penaltymodel == null)
            {
                return NotFound();
            }
            penaltyHistory = _context.PenaltyHistories.Where(p => p.penaltyID == penaltymodel.penaltyID).OrderBy(p => p.modifiedDate).ToList();
            penaltyModel = penaltymodel;
           ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
           ViewData["penaltyTypeID"] = new SelectList(_context.PenaltyTypes, "penaltyTypeID", "penaltyName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) return RedirectToPage("/Shared/AccessDenied");

            var penal = _context.Penalties.FirstOrDefault(p => p.penaltyID == penaltyModel.penaltyID) ?? new penaltyModel();

            penal.penaltyIssueDate = penaltyModel.penaltyIssueDate;
            penal.penaltyReason = penaltyModel.penaltyReason;
            penal.penaltyReference = penaltyModel.penaltyReference;
            penal.penaltyStartDate = penaltyModel.penaltyStartDate;
            penal.penaltyEndDate = penaltyModel.penaltyEndDate;
            penal.penaltyStatus = penaltyModel.penaltyStatus;
            penal.modifiedBy = User.Identity.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!penaltyModelExists(penaltyModel.penaltyID))
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

        private bool penaltyModelExists(int id)
        {
            return _context.Penalties.Any(e => e.penaltyID == id);
        }
    }
}
