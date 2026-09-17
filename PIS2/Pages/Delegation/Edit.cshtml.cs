using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Pages.delegation
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly ILogger<EditModel> _logger;
        public EditModel(PISContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public delegationModel delegationModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("MANAGEMENT") || User.IsInRole("HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }

            if (id == null)
            {
                return NotFound();
            }

            var delegationmodel =  await _context.Delegations
                .Include(d => d.FromEmployment).ThenInclude(e => e.personModel)
                .Include(d => d.ToEmployment).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.delegationID == id);
            if (delegationmodel == null)
            {
                return NotFound();
            }
            delegationModel = delegationmodel;
          
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRMANAGER") || !User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            var existing = await _context.Delegations.FirstOrDefaultAsync(d => d.delegationID == delegationModel.delegationID);

            if (existing == null) { return NotFound(); }

            ModelState.Remove("delegationModel.modifiedBy");

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                        _logger.LogError($"Validation Error for{kv.Key} --> {error.ErrorMessage}", User.Identity.Name, DateTime.Now);
                        var msg = new { success = false, message = $"Validation Error for {kv.Key}!" };
                        return new JsonResult(msg);
                    }
                }

                return Page();
            }

            existing.delegationStatus = delegationModel.delegationStatus;
            existing.modifiedBy = User.Identity.Name;
            existing.delegationStartDate = existing.delegationStartDate > DateTime.Now ? delegationModel.delegationStartDate :existing.delegationStartDate;
            existing.delegationEndDate = delegationModel.delegationEndDate;
            try
            {
                await _context.SaveChangesAsync();
                var msg = new { success = true, message = $"Updated Successfuly!" };
                return new JsonResult(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating delegation!", User.Identity.Name, DateTime.Now);
                var msg = new { success = false, message = $"Error updating delegation!" };
                return new JsonResult(msg);
                
            }

            return RedirectToPage("./Index");
        }

        private bool delegationModelExists(int id)
        {
            return _context.Delegations.Any(e => e.delegationID == id);
        }
    }
}
