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
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(PISContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            return Page();
        }

        [BindProperty]
        public allowanceModel allowanceModel { get; set; } = default!;
        public SelectList EarningType { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRADMIN"))
            {
                _logger.LogError("Error: Access Denied! {Action} {UserName}", "Create Allowance", User.Identity.Name);
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
                        _logger.LogError(error.ErrorMessage, "Error validation failed for {UserName}", User.Identity.Name); 
                    }
                    return new JsonResult(new { success=false, message="Validation Error. Check all fields are set!"});
                }
                return Page();
            }
            var earn = await _context.EarningTypes.Where(e => e.earningTypeStatus == mainStatus.Active).ToListAsync();
            EarningType = new SelectList(earn, "earningTypeID", "earningTypeName");
            _context.Allowances.Add(allowanceModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = allowanceModel.allowanceID});
        }
    }
}
