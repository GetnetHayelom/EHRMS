using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.OvertimeHistory
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
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
