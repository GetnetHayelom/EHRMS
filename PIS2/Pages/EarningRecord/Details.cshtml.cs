using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Views;
using System;

namespace PIS2.Pages.EarningRecord
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _db;

        public DetailsModel(PISContext db)
        {
            _db = db;
        }

        public earningModel Earning { get; set; } = default!;

        public List<earningRecordModel> earningRecords { get; set; }
        public List<TransactionVM> EarningTransactions { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Earning = await _db.Earnings
                .Include(d => d.earningType)
                .Include(d => d.EmploymentModel).ThenInclude(d => d.personModel)
                .Include(d => d.EarningHistories)
                .FirstOrDefaultAsync(d => d.earningID == id);

            if (Earning == null)
            {
                return NotFound();
            }

            // Sort history latest first
            Earning.EarningHistories = Earning.EarningHistories?
                .OrderByDescending(h => h.modifiedDate)
                .ToList();

            EarningTransactions = await _db.PayrollPays
                .Include(p => p.payrollModel)
                .Include(p => p.EarningRecords)
                .Where(p => p.EarningRecords.Any(d =>
                    d.earningReference == Earning.earningID.ToString()))
                .SelectMany(p => p.EarningRecords
                    .Where(d => d.earningReference == Earning.earningID.ToString())
                    .Select(d => new TransactionVM
                    {
                        PayrollPayID = p.payrollPayID,
                        PayrollName = p.payrollModel.payrollName,
                        PayrollMonth = p.payrollModel.payrollMonth,
                        PayrollStart = p.payrollModel.StartDate,
                        PayrollEnd = p.payrollModel.EndDate,
                        PayrollStatus = p.payrollModel.payrollStatus,

                        Amount = d.earningAmount,
                        GrossPay = p.GrossPay,
                        NetPay = p.NetPay,
                        ProcessedBy = p.modifiedBy
                    }))
                .OrderByDescending(x => x.PayrollStart)
                .ToListAsync();

            return Page();
        }
    }
}

