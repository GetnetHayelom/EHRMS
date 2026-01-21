using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;
using System;

namespace PIS2.Pages.DeductionRecord
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _db;

        public DetailsModel(PISContext db)
        {
            _db = db;
        }

        public deductionModel Deduction { get; set; } = default!;

        public List<deductionRecordModel> deductionRecords { get; set; }
        public List<TransactionVM> DeductionTransactions { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Deduction = await _db.Deductions
                .Include(d => d.DeductionType)
                .Include(d => d.EmploymentModel).ThenInclude(d => d.personModel)
                .Include(d => d.DeductionHitroies)
                .FirstOrDefaultAsync(d => d.deductionID == id);

            if (Deduction == null)
            {
                return NotFound();
            }

            // Sort history latest first
            Deduction.DeductionHitroies = Deduction.DeductionHitroies?
                .OrderByDescending(h => h.modifiedDate)
                .ToList();

            DeductionTransactions = await _db.PayrollPays
                .Include(p => p.payrollModel)
                .Include(p => p.DeductionRecords)
                .Where(p => p.DeductionRecords.Any(d =>
                    d.deductionReference == Deduction.deductionID.ToString()))
                .SelectMany(p => p.DeductionRecords
                    .Where(d => d.deductionReference == Deduction.deductionID.ToString())
                    .Select(d => new TransactionVM
                    {
                        PayrollPayID = p.payrollPayID,
                        PayrollName = p.payrollModel.payrollName,
                        PayrollMonth = p.payrollModel.payrollMonth,
                        PayrollStart = p.payrollModel.StartDate,
                        PayrollEnd = p.payrollModel.EndDate,
                        PayrollStatus = p.payrollModel.payrollStatus,

                        Amount = d.deductionAmount,
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

