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
        public async Task OnGetAsync(int id=1)
        {
            CurrentPage = id > 0 ? id : 1;
            var OTR = await _context.OvertimeRecords.CountAsync();

            TotalPages = (int)Math.Ceiling(OTR / (double)PageSize);
            overtimeRecordModel = await _context.OvertimeRecords
                .Include(o => o.employmentModel)
                .Include(o => o.overtimeModel)
                .Include(o => o.OvertimeHistories)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize).ToListAsync();
        }
    }
}
