using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;


namespace PIS2.Pages.EarningRecord
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _db;

        public IndexModel(PISContext db)
        {
            _db = db;
        }

        public List<earningModel> Earnings { get; set; }

        public async Task OnGet()
        {
            Earnings = new List<earningModel>();
            Earnings = await _db.Earnings.Include(e => e.earningType)
                .Include(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .ToListAsync();
        }
    }
}
