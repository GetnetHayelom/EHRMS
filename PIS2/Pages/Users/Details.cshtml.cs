using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public userModel userModel { get; set; } = default!;
        public List<userHistoryModel> userHistory { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usermodel = await _context.Users.Include(u => u.personModel)
                .Include(u => u.UserHistories).FirstOrDefaultAsync(m => m.userID == id);
            if (usermodel == null)
            {
                return NotFound();
            }
            else
            {
                userModel = usermodel;
                userHistory = userModel.UserHistories.ToList() ?? new List<userHistoryModel>();
            }
            return Page();
        }
    }
}
