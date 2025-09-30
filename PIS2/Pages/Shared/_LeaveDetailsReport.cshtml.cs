using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Shared
{
    public class _LeaveDetailsReportModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
    

        public _LeaveDetailsReportModel(PIS2.Models.PISContext context)
        {
            _context = context;
       
        }
        public IList<leaveTypeModel> LeaveTypes { get; set; } = default!;
        public void OnGet()
        {
            LeaveTypes = _context.LeaveTypes.ToList();
        }
    }
}
