using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models.HR;

namespace PIS2.Pages.LeaveHistory
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<leaveHistoryModel> leaveHistoryModel { get;set; } = default!;

        public async Task OnGetAsync(int? id)
        {
            leaveHistoryModel = await _context.LeaveHistories
                .Include(l => l.leaveModel).Where(lh => lh.leaveID ==id).ToListAsync();
        }
    }
}
