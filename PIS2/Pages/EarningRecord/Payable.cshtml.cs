using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;

namespace PIS2.Pages.Payroll
{
    public class PayableModel : PageModel
    {
        private readonly PISContext _db;

        public PayableModel(PISContext db)
        {
            _db = db;
        }

        public List<earningModel> Earnings { get; set; }

        public async Task OnGet()
        {
            Earnings = new List<earningModel>();
            Earnings = await _db.Earnings.Include(e => e.earningType)
                .Include(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .Where(e => !e.earningType.isPayroll && e.earningStatus == mainStatus.Active)
                .ToListAsync();
        }
    }
}
