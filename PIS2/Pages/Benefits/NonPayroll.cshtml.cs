using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Services;
using System.Threading.Tasks;
 
namespace PIS2.Pages.Benefits
{
    public class NonPayrollModel : PageModel
    {
        private readonly PISContext _db;
        private readonly Global_S _global;
        private readonly ILogger<NonPayrollModel> _logger;

        public NonPayrollModel (PISContext db, Global_S global, ILogger<NonPayrollModel> logger)
        {
            _db = db;
            _global = global;
            _logger = logger;
        }

        public List<earningModel> Earnings { get; set; }
        public async Task OnGet()
        {
            var ear = await _db.Earnings
                .Include(e => e.earningType)
                .Include(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .Where(e => !e.earningType.isPayroll).ToListAsync();
            Earnings = ear ?? new List<earningModel> ();
        }
    }
}
