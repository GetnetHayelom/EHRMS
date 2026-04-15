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

            var company = _context.JobPlacements.Include(j => j.departmentModel)
                .FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active && j.employmentID == empID)?.departmentModel?.companyID;

            Company = _context.Companies.FirstOrDefault(c => c.companyID == company)?.companyName;
            var employees = _context.Employments
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel)
                .Where(e => e.employmentStatus == mainStatus.Active
                && e.JobPlacements.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active).departmentModel.companyID == company)
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
        }
    }
}
