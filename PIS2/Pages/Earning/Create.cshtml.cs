using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using System.Threading.Tasks;

namespace PIS2.Pages.Earning
{
    [Authorize(Roles = "HRADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        public SelectList Accounts { get; set; }
        [BindProperty]
        public earningType EarningType { get; set; } = new earningType();

        public async Task OnGet()
        {
            getOptions();
        }
       
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("HRADMIN"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            ModelState.Remove("EarningType.modifiedBy");
            EarningType.modifiedBy = User.Identity.Name;
            EarningType.modifiedDate = DateTime.Now;
            if (!ModelState.IsValid) {
                getOptions(); return Page();}
                

            _context.EarningTypes.Add(EarningType);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        private void getOptions()
        {
            var accounts = _context.Accounts
                .Where(a => a.accountStatus == mainStatus.Active)
                .OrderBy(a => a.accountName)
                .ToList();
            Accounts = new SelectList(accounts, "accountID", "accountName");
        }
    }
}
