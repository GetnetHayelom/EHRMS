using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;

namespace PIS2.Pages.BankInfo
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? Bank { get; set; }
        public IList<bankInfoModel> bankInfoModel { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public bool EmployeeOnly { get; set; } = false;

        public async Task OnGetAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                RedirectToPage("/Shared/AccessDenied");
            }

            var bankInfos = _context.BankInfos
                .Include(b => b.personModel).ThenInclude(p => p.Employments).AsQueryable();

            if (!Bank.IsNullOrEmpty()) bankInfos.Where(b => b.bankName == Bank);

            bankInfoModel = bankInfos.ToList();
        }
    }
}
