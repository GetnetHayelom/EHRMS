using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.OvertimHistory
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<overtimeHistoryModel> overtimeHistoryModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            overtimeHistoryModel = await _context.OvertimeHistories
                .Include(o => o.overtimeRecordModel).ToListAsync();
        }
    }
}
