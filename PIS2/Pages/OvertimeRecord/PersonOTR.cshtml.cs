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
    public class PersonOTRModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public PersonOTRModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<overtimeRecordModel> overtimeRecordModel { get;set; } = default!;
        public employmentModel employmentModel { get;set; }
        public int otCount { get; set; }
        public double TotalOtCost { get; set; }

        public async Task OnGetAsync(List<int>? otrID, int? empID)
        {
            //overtimeRecordModel = await _context.OvertimeRecords
            //    .Include(o => o.departmentModel)
            //    .Include(o => o.employmentModel)
            //    .Include(o => o.overtimeModel).ToListAsync();
            overtimeRecordModel = new List<overtimeRecordModel>();
            var otr = _context.OvertimeRecords
                .Include(o => o.employmentModel).ThenInclude(e =>e.personModel)
                .Include(o => o.overtimeModel)
                .Include(o => o.departmentModel)
                .Include(o => o.OvertimeHistories).AsQueryable();
            if (otrID != null && otrID.Any())
            {
                overtimeRecordModel = await otr.Where(o => otrID.Contains(o.overtimeRecordID))
                .ToListAsync();

            }
            else if(empID != null || empID>0)
            {
                overtimeRecordModel = await otr.Where(otr => otr.overtimeRecordStatus == overtimeStatus.Hold && otr.employmentID == empID).ToListAsync();
            }
            //else
            //{
            //   overtimeRecordModel = await otr.Where(otr => otr.overtimeRecordStatus == overtimeStatus.Hold).ToListAsync();
               
            //}
            otCount = overtimeRecordModel.Count;
            TotalOtCost = overtimeRecordModel.Sum(ot => ot.GetOtCost);

        }
    }
}
