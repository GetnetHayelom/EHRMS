using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.OvertimeRecord
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<overtimeRecordModel> overtimeRecordModel { get;set; } = default!;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public async Task OnGetAsync(List<int>? id)
        {
            var otr = _context.OvertimeRecords
                .Include(o => o.employmentModel)
                .Include(o => o.overtimeModel)
                .Include(o => o.OvertimeHistories).AsQueryable();
            if (id != null && id.Any())
            {
                overtimeRecordModel =await otr.Where(o => id.Contains(o.overtimeRecordID))
                .ToListAsync();
            }
            else
            {
                overtimeRecordModel = await otr.Where(otr => otr.overtimeRecordStatus == overtimeStatus.Hold).ToListAsync();
            }
        }
    }
}
