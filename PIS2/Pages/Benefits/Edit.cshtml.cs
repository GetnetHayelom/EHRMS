using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Services;

namespace PIS2.Pages.Benefits
{
    [Authorize(Roles ="MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER, MIE\\PMS_FINANCE")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly PayrollService _payrollService;

        public EditModel(PISContext db, PayrollService payrollService)
        {
            _context = db;
            _payrollService = payrollService;
        }
        [BindProperty]
        public otherPay OtherPay { get; set; } = default!;

        public SelectList EarningOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            OtherPay = await _context.OtherPayments
                .Include(p => p.earningModel).ThenInclude(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .Include(p => p.earningModel).ThenInclude(e => e.earningType)
                .FirstOrDefaultAsync(m => m.paymentID == id);

            if (OtherPay == null) return NotFound();
 
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = await _context.OtherPayments.FirstOrDefaultAsync(e => e.paymentID == OtherPay.paymentID);
            
            if(existing == null) { return NotFound(); }

            switch (existing.paymentStatus)
            {
                case payrollStatus.PENDING:

                    if (User.IsInRole("MIE\\PMS_HRMANAGER"))
                    {
                        
                        if (OtherPay.paymentStatus == payrollStatus.APPROVED)
                            existing.paymentStatus = payrollStatus.APPROVED;
                    }
                    else
                    {
                        return RedirectToPage("/Shared/AccessDenied");
                    }

                    break;

                case payrollStatus.APPROVED:

                    if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
                        return RedirectToPage("/Shared/AccessDenied");

                    if (OtherPay.paymentStatus == payrollStatus.POSTED)
                        existing.paymentStatus = payrollStatus.POSTED;

                    break;

                case payrollStatus.POSTED:

                    if (!User.IsInRole("MIE\\PMS_FINANCE"))
                        return RedirectToPage("/Shared/AccessDenied");

                    if (OtherPay.paymentStatus == payrollStatus.COMPLETED)
                    {
                        if(OtherPay.invoiceNo =="" || OtherPay.invoiceNo == "Empty")
                        {
                            TempData["message"] = ("Error", "Invoice No is required!");
                            return Page();
                        }
                        existing.paymentStatus = payrollStatus.COMPLETED;
                        existing.NetPay = OtherPay.NetPay;
                        existing.remark = OtherPay.remark;
                        existing.invoiceNo = OtherPay.invoiceNo;
                    }
                        
                    break;

                case payrollStatus.COMPLETED:

                    TempData["message"] = ("Error", "Payment already completed.");
                    return Page();
            }
            existing.modifiedBy = User.Identity.Name;
            existing.modifiedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToPage("./Details", new {id = OtherPay.paymentID});
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.payrollID == id);
            if (payroll == null) return NotFound();

            payroll.payrollStatus = payrollStatus.APPROVED;
            payroll.modifiedBy = User.Identity.Name ?? "system";
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostPostAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.payrollID == id);
            if (payroll == null) return NotFound();
            if (payroll.payrollStatus == payrollStatus.PENDING) return BadRequest("Payroll must be approved first.");

            // mark as posted
            await _payrollService.PostPayrollAsync(payroll.payrollID, User.Identity.Name ?? "system");

            return RedirectToPage(new { id });
        }
        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_FINANCE")) return RedirectToPage("/Shared/AccessDenied");
            await _payrollService.CompletePayrollAsync(id, User.Identity.Name);
            return RedirectToPage();
        }
    }
}
