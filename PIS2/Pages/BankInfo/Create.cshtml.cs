using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.BankInfo
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ViewData["personID"] = new SelectList(_context.Persons, "personID", "personID");
            return Page();
        }

        [BindProperty]
        public bankInfoModel bankInfoModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRPERSONNEL"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.BankInfos.Add(bankInfoModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
