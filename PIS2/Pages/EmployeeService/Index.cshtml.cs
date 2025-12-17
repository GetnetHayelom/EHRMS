using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PIS2.Pages.Leave.IndexModel;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.EmployeeService
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly PIS2.Models.Core _core;
        private readonly PIS2.Models.PayrollService _payrollService;
        public IndexModel(PIS2.Models.PISContext context, Models.Core core, PayrollService payrollService)
        {
            _context = context;
            _core = core;
            _payrollService = payrollService;
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
        public List<payrollPay> payrollPays { get; set; }
        public List<string> Company { get; set; }
        public List<earningType> EarningTypes { get; set; }
        public List<deductionType> DeductionTypes { get; set; }
  
        public async Task OnGetAsync()
        {
            
            EarningTypes = await _context.EarningTypes.ToListAsync();
            DeductionTypes = await _context.DeductionTypes.ToListAsync();
            var userID = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.userID ?? 0;

            var empID = _core.getUserEmp(User.Identity.Name);

            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;

            var accessibleCompanies = _context.Accesses
                .Where(a => a.userID == userID && a.accessStatus == mainStatus.Active) // 1 = active access
                .Select(a => a.companyID)
                .ToList();

            var allowedCompanies = accessibleCompanies
                .Append(company)
                .Where(c => c != null)
                .Distinct()
                .ToList();
            
            Company = _context.Companies.Where(e => allowedCompanies.Contains(e.companyID)).Select(e => e.companyName).ToList();
            var employees = _context.Employments
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel)
                .Where(e => e.employmentStatus == mainStatus.Active
                && e.JobPlacements.Any(j => j.jobPlacementStatus == mainStatus.Active && allowedCompanies.Contains(j.departmentModel.companyID)))
                .OrderBy(e => e.givenID)
                .Select(e => new
                {
                    empID=e.employmentID,
                    EmployeeID = e.givenID,
                    Name = e.personModel.personFullName,

                    // Pick the active job placement once
                    ActiveJobPlacement = e.JobPlacements
                        .Where(jp => jp.jobPlacementStatus == mainStatus.Active)
                        .OrderByDescending(jp => jp.jobPlacementDate) // if multiple active, take latest
                        .FirstOrDefault(),

                    // Sum active allowances safely
                    Allowance = e.AllowanceAssignments
                        .Where(aa => aa.allowanceStatus == mainStatus.Active)
                        .Select(aa => (decimal?)aa.allowanceAssignmentAmount) // cast to nullable
                        .Sum() ?? 0
                })
                .AsEnumerable() // switch to LINQ-to-objects to safely use null-conditional
                .Select(e => new EarningView
                {
                    empID =e.empID,
                    EmployeeID =e.EmployeeID,
                    EmployeeName=e.Name,
                    JobTitle = e.ActiveJobPlacement?.jobModel?.jobTitle,
                    Department = e.ActiveJobPlacement?.departmentModel?.departmentName,
                    Company = e.ActiveJobPlacement?.departmentModel?.companyModel?.companyName,
                    Salary = e.ActiveJobPlacement?.jobPlacementSalary ?? 0,
                    Allowance = e.Allowance
                })
                .ToList();

            EarningView =employees.ToList();

            GroupedEarning = EarningView.GroupBy(l => l.Department)
                .Select(g => new GroupedEarningByDep
                {
                    Name = g.Key ?? "Unknown",
                    Salary = g.Sum(e => e.Salary),
                    Allowance = g.Sum(e => e.Allowance),
                    Records = g.ToList()
                   
                }).ToList();

            
            allowanceSum = EarningView.Sum(e => e.Allowance);
            SalarySum = EarningView.Sum(e => e.Salary);


            //Payroll Pay
            var today = DateTime.Today;
            var start = new DateTime(today.Year, today.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var payroll = new payrollModel
            {
                StartDate = start,
                EndDate = end,
                payrollName = $"{today:MM}-{Company.First()}",
                payrollStatus = payrollStatus.PENDING,
                modifiedBy = User.Identity.Name
            };


            var totalGross = 0.0m;
            var totalNet = 0.0m;
            var totalTax = 0.0m;
            var totalPenEmp = 0.0m;
            var totalEmployees = 0;
            payrollPays = new List<payrollPay>();
            foreach (var emp in employees)
            {
                var emplModel = _context.Employments.FirstOrDefault(e => e.employmentID == emp.empID);
                var pay = await _payrollService.CalculateEmployeePayAsync(emplModel, payroll);
               
                payrollPays.Add(pay);
                totalGross += pay.GrossPay;
                totalNet += pay.NetPay;
                totalTax += pay.DeductionRecords.Where(d => d.DeductionType != null && d.DeductionType.deductionName.ToLower().Contains("tax")).Sum(d => d.deductionAmount ?? 0);
                totalPenEmp += pay.DeductionRecords.Where(d => d.DeductionType != null && d.DeductionType.deductionName.ToLower().Contains("pension")).Sum(d => d.deductionAmount ?? 0);
                totalEmployees++;
            }

            payroll.totalGross = totalGross;
            payroll.totalNet = totalNet;
            payroll.totalTax = totalTax;
            payroll.totalPensionEmployee = totalPenEmp;
            payroll.totalEmployees = totalEmployees;
     
            //
            //
        }
    }
}
