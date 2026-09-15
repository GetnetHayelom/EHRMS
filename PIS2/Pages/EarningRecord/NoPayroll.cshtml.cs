using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Earning
{
    [Authorize(Roles = "FINANCE, HRMANAGER")]
    public class NoPayrollModel : PageModel
    {
        private readonly PISContext _db;

        public NoPayrollModel(PISContext db)
        {
            _db = db;
        }

        [BindProperty]
        public earningModel Earning { get; set; }

        public SelectList EarningTypes { get; set; }
        public SelectList Employees { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Earning = await _db.Earnings.FindAsync(id);

            if (Earning == null)
                return NotFound();

            EarningTypes = new SelectList(await _db.EarningTypes.ToListAsync(), "earningTypeID", "earningTypeName");
            Employees = new SelectList(await _db.Employments.ToListAsync(), "employmentID", "givenID");

            return Page();
        }


    }
}
