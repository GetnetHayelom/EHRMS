using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public userModel userModel { get; set; } = default!;
        public List<userHistoryModel> userHistory { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usermodel =  await _context.Users.Include(u => u.personModel).Include(u => u.UserHistories).FirstOrDefaultAsync(m => m.userID == id);
            if (usermodel == null)
            {
                return NotFound();
            }
            userModel = usermodel;
            userHistory =userModel.UserHistories.ToList() ?? new List<userHistoryModel>();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("userModel.modifiedBy");
            userModel.modifiedBy = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(userModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userModelExists(userModel.userID))
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

        private bool userModelExists(int id)
        {
            return _context.Users.Any(e => e.userID == id);
        }
    }
}
