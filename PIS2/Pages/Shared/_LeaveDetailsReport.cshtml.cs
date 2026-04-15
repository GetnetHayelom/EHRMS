using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Shared
{
    public class _LeaveDetailsReportModel : PageModel
    {
        private readonly PISContext _context;
    

        public _LeaveDetailsReportModel(PISContext context)
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
