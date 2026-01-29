using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Benefits
{
    [Authorize(Roles ="MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER, MIE\\PMS_FINANCE")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
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
            var exisiting = await _context.OtherPayments.FirstOrDefaultAsync(e => e.paymentID == OtherPay.paymentID);
            
            if(exisiting == null) { return NotFound(); }

            if (exisiting.paymentStatus != payrollStatus.PENDING && !User.IsInRole("MIE\\PMS_HRMANGER"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (exisiting.paymentStatus == payrollStatus.POSTED && !User.IsInRole("MIE\\PMS_FINANCE"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (exisiting.paymentStatus == payrollStatus.COMPLETED)
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
                // Update Audit Info before saving
                // In a real app, User.Identity.Name would be used for modifiedBy
            OtherPay.modifiedBy = User.Identity?.Name ?? "Illegal User";
            OtherPay.modifiedDate = DateTime.Now;

            //if (!ModelState.IsValid)
            //{
            //    EarningOptions = new SelectList(_context.Earnings, "earningID", "earningName");
            //    return Page();
            //}

            

            _context.Attach(OtherPay).State = EntityState.Modified;

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

        
    }
}
