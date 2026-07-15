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
        public payrollModel OtherPay { get; set; } = default!;

        public SelectList EarningOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            OtherPay = await _context.Payrolls
                .Include(p => p.PayrollPays).ThenInclude(e => e.EmploymentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.payrollID == id);

            if (OtherPay == null) return NotFound();
 
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = await _context.Payrolls.FirstOrDefaultAsync(e => e.payrollID == OtherPay.payrollID);
            
            if(existing == null) { return NotFound(); }

            switch (existing.payrollStatus)
            {
                case Enums.payrollStatus.PENDING:

                    if (User.IsInRole("MIE\\PMS_HRMANAGER"))
                    {
                        
                        if (OtherPay.payrollStatus == Enums.payrollStatus.APPROVED)
                            existing.payrollStatus = Enums.payrollStatus.APPROVED;
                    }
                    else
                    {
                        return RedirectToPage("/Shared/AccessDenied");
                    }

                    break;

                case Enums.payrollStatus.APPROVED:

                    if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
                        return RedirectToPage("/Shared/AccessDenied");

                    if (OtherPay.payrollStatus == Enums.payrollStatus.POSTED)
                        existing.payrollStatus = Enums.payrollStatus.POSTED;

                    break;

                case Enums.payrollStatus.POSTED:

                    if (!User.IsInRole("MIE\\PMS_FINANCE"))
                        return RedirectToPage("/Shared/AccessDenied");

                    if (OtherPay.payrollStatus == Enums.payrollStatus.COMPLETED)
                    {
                        
                        existing.payrollStatus = Enums.payrollStatus.COMPLETED;
                        existing.totalNet = OtherPay.totalNet;
                        existing.remark = OtherPay.remark;
                        existing.reference = OtherPay.reference;
                    }
                        
                    break;

                case Enums.payrollStatus.COMPLETED:

                    TempData["message"] = ("Error", "Payment already completed.");
                    return Page();
            }
            existing.modifiedBy = User.Identity.Name;
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToPage("./Details", new {id = OtherPay.payrollID});
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.payrollID == id);
            if (payroll == null) return NotFound();

            payroll.payrollStatus = Enums.payrollStatus.APPROVED;
            payroll.modifiedBy = User.Identity.Name ?? "system";
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostPostAsync(int id)
        {
            if (!User.IsInRole("MIE\\PMS_PAYROLL")) return RedirectToPage("/Shared/AccessDenied");
            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.payrollID == id);
            if (payroll == null) return NotFound();
            if (payroll.payrollStatus == Enums.payrollStatus.PENDING) return BadRequest("Payroll must be approved first.");

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
