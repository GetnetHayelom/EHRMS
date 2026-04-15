using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.OvertimeRecord
{
    public class PersonOTRModel : PageModel
    {
        private readonly PISContext _context;

        public PersonOTRModel(PISContext context)
        {
            _context = context;
        }

        public IList<overtimeRecordModel> overtimeRecordModel { get;set; } = default!;
        public employmentModel employmentModel { get;set; }
        public departmentModel departmentModel { get;set; }
        public int otCount { get; set; }
        public decimal TotalOtCost { get; set; }

        public async Task OnGetAsync(List<int>? otrID, int? empID, DateTime? date)
        {
           
            overtimeRecordModel = new List<overtimeRecordModel>();
            employmentModel = new employmentModel();
            departmentModel = new departmentModel();

            var otr = _context.OvertimeRecords
                .Include(o => o.employmentModel).ThenInclude(e =>e.personModel)
                .Include(o => o.overtimeModel)
                .Include(o => o.departmentModel)
                .Include(o => o.OvertimeHistories).AsQueryable();

            Console.WriteLine("################### ALL" + otr.Count());

            if (otrID != null && otrID.Any()) 
            {
                otr = otr.Where(o => otrID.Contains(o.overtimeRecordID));
                
                Console.WriteLine("################### From OTR " + overtimeRecordModel.Count());

            }
            if(empID.HasValue && empID >0 )
            {
                otr = otr.Where(o => o.employmentID == empID.Value);
                Console.WriteLine("################### From EP " + overtimeRecordModel.Count());
               
            }
            if (date.HasValue)
            {
                otr = otr.Where(o => o.overtimeRecordDate == date.Value);
            }
            overtimeRecordModel =await otr.ToListAsync();
            // Protect from null / empty list
            if (overtimeRecordModel.Count == 0)
            {
                otCount = 0;
                TotalOtCost = 0;
                return; // Avoid processing empty data
            }
            var first = overtimeRecordModel.First();

            employmentModel = first.employmentModel ?? new employmentModel();
            departmentModel = first.departmentModel ?? new departmentModel();

           
            otCount = overtimeRecordModel.Count;
            TotalOtCost = overtimeRecordModel.Sum(ot => ot.GetOtCost);

            return ;
        }
    }
}
