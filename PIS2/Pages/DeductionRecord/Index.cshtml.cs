using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.DeductionRecord
{
    [Authorize(Roles ="MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _db;

        public IndexModel(PISContext db)
        {
            _db = db;
        }
        public List<deductionModel> deductions { get; set; }
        public void OnGet(int? id)
        {
            deductions = _db.Deductions
            .Include(d => d.DeductionType)
            .Include(d => d.EmploymentModel)
            .ToList();
        }
    }
}
