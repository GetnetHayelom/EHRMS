using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Services;
using PIS2.Views;
using static System.Formats.Asn1.AsnWriter;
using static PIS2.Pages.Leave.IndexModel;

namespace PIS2.Pages.EmployeeService
{
    public class PayrollModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public PayrollModel(PISContext context, Core core)
        {
            _context = context;
            _core = core;
        }
        public class GroupedEarningByDep
        {
            public string Name { get; set; } = string.Empty;
            public decimal Salary { get; set; }
            public decimal Allowance { get; set; }
            public decimal Sum { get { return Allowance + Salary; } }
            public List<EarningView> Records { get; set; } = new();
        }

        
        public List<GroupedEarningByDep> GroupedEarning { get; set; }
      
        
        public decimal allowanceSum { get; set; }
        public decimal SalarySum { get; set; }
        public List<EarningView> EarningView { get; set; }

        public string Company { get; set; }
        public async Task OnGetAsync()
        {
            var empID = _core.getUserEmp(User.Identity.Name);
            var earnings = await _context.EarningView.ToListAsync();

            allowanceSum = EarningView.Where(e => !e.earningTypeName.Contains("Salary")).Sum(e => e.earningAmount);
            SalarySum = EarningView.Where(e => e.earningTypeName.Contains("Salary")).Sum(e => e.earningAmount);
        }
    }
}
