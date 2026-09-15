using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.EmployeeService
{
    [Authorize(Roles = "HRCLERK, HRMANAGER")]
    public class EarningModel : PageModel
    {
        private readonly PISContext _context;
        private readonly PayrollService _payrollService;
        private readonly Global_S _global;

        public EarningModel(PISContext context, PayrollService payrollService, Global_S global)
        {
            _context = context;
            _payrollService = payrollService;
            _global = global;
        }

        [BindProperty]
        public employmentModel employmentModel { get; set; } = default!;
        [BindProperty]
        public jobPlacementModel jobPlacementModel { get; set; } = default!;
        [BindProperty]
        public personModel personModel { get; set; } = default!;
  
        [BindProperty]
        public List<allowanceAssignmentModel> AllowanceAssignments { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string ErrorMessage { get; set; }
        int ? EmpID { get; set; }
        [BindProperty]
        public bool EmpSelected { get; set; } = true;

        public List<overtimeRecordModel> OvertimeRecords { get; set; }
        public payrollPay payrollPay { get; set; }
        public List<earningType> EarningTypes { get; set; }
        public List<deductionType> DeductionTypes { get; set; }
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!(User.IsInRole("HRCLERCK") || User.IsInRole("HRMANAGER"))) { return RedirectToPage("/Shared/AccessDenied"); }
            EarningTypes = await _context.EarningTypes.ToListAsync();
            DeductionTypes = await _context.DeductionTypes.ToListAsync();

            if (!string.IsNullOrEmpty(givenID) && id == null)
            {
                var emp = await _context.Employments
                    .FirstOrDefaultAsync(e => e.givenID == givenID);

                if (emp != null)
                {
                    // Redirect to the Details page with employmentID                    
                    return RedirectToPage("Earning", new { id = emp.employmentID });
                }

                ErrorMessage = "No employee found with that Given ID.";
            }
            if (id == null)
            {
                EmpSelected = false;
                return Page();
            }
            
            var employmentmodel =  await _context.Employments
                .Include(e => e.personModel).ThenInclude(p => p.addressModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.jobStepModel).ThenInclude(js=> js.jobGradeModel)
                .Include(e => e.JobPlacements).ThenInclude(jp => jp.departmentModel).ThenInclude(d => d.companyModel)
                .FirstOrDefaultAsync(m => m.employmentID == id);

 
            AllowanceAssignments = new List<allowanceAssignmentModel>();
            OvertimeRecords = new List<overtimeRecordModel>();
            if (employmentmodel == null)
            {
                EmpSelected = false;
                return Page();
            }
            

            employmentModel = employmentmodel ?? new employmentModel();
            jobPlacementModel = employmentModel.JobPlacements?.FirstOrDefault(j => j.jobPlacementStatus == mainStatus.Active) ?? new jobPlacementModel();
            personModel =await _context.Persons.Include(p => p.addressModel).FirstOrDefaultAsync( p=> p.personID == employmentModel.personID) ?? new personModel();

            AllowanceAssignments =await _context.AllowanceAssignments
                .Include(aa=> aa.allowanceModel).Where(aa=>aa.employmentID == employmentModel.employmentID && aa.allowanceStatus == mainStatus.Active).ToListAsync();

            OvertimeRecords =await _context.OvertimeRecords
                .Include(aa => aa.overtimeModel).Where(otr => otr.employmentID == employmentModel.employmentID && otr.overtimeRecordStatus == overtimeStatus.Posted).ToListAsync();

            
            //Payroll Pay
            var today = DateTime.Today;
            var start = new DateTime(today.Year, today.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var payroll = new payrollModel
            {
                StartDate = start,
                EndDate = end,
                payrollName = $"{today:MM}-{employmentmodel.givenID}",
                payrollStatus = payrollStatus.PENDING,
                modifiedBy = User.Identity.Name
            };
            var earnings = new List<earningRecordModel>();
            earnings.Add(new earningRecordModel
            {
                earningAmount = jobPlacementModel.jobPlacementSalary,
                earningType =_context.EarningTypes.FirstOrDefault(e => e.earningTypeCode == Global_C.SALARY_PAY_CODE),
                earningReference = "Salary"
            });
            
            foreach(var al in AllowanceAssignments)
            {
                earnings.Add(new earningRecordModel
                {
                    earningAmount = al.allowanceAssignmentAmount,
                    earningType = _context.EarningTypes.FirstOrDefault(e => e.earningTypeCode == Global_C.ALLOWANCE_TYPE_CODE),
                    earningReference = al.allowanceModel?.allowanceName
                });
            }
            
            var deductions = new List<deductionRecordModel>();
            deductions.Add(new deductionRecordModel
            {
                deductionAmount = jobPlacementModel.jobPlacementSalary * .07m,
                DeductionType = _context.DeductionTypes.FirstOrDefault(d => d.deductionCode == Global_C.PENSION_TYPE_CODE),
                deductionReference = "Pension"
            });
            var taxable = jobPlacementModel.jobPlacementSalary + (AllowanceAssignments.Sum(s => s.allowanceAssignmentAmount));
            var tax = _payrollService.GetTaxAmount(taxable);
            deductions.Add(new deductionRecordModel
            {
                deductionAmount = taxable - tax,
                DeductionType = _context.DeductionTypes.FirstOrDefault(d => d.deductionCode == Global_C.TAX_PAY_CODE),
                deductionReference = "Income-tax"
            });

            var otherDeducts = await _context.Deductions.Where(d => d.employmentID == employmentModel.employmentID).ToListAsync();
            foreach(var d in otherDeducts)
            {
                deductions.Add(new deductionRecordModel
                {
                    deductionAmount = d.deductionAmount,
                    DeductionType = _context.DeductionTypes.FirstOrDefault(d => d.deductionCode == Global_C.TAX_PAY_CODE),
                    deductionReference = _context.DeductionTypes?.FirstOrDefault(d => d.deductionCode == Global_C.TAX_PAY_CODE)?.deductionName ?? ""
                });
            }

            var otherEarnings = await _context.Earnings.Where(d => d.employmentID == employmentModel.employmentID && d.earningType.isRecurring).ToListAsync();

            payrollPay = new payrollPay();
            payrollPay.EarningRecords = earnings;
            payrollPay.DeductionRecords = deductions;

            Gross = earnings.Sum(e => e.earningAmount);
            var net = deductions?.Sum(d => d.deductionAmount) ?? 0;
            Net = Gross - net;
            return Page();
        }      

        private int getEmpIDFromPerson(int personID)
        {
            var empID = 0;
            empID = _context.Employments.FirstOrDefault(e => e.personID == personID && e.employmentStatus == mainStatus.Active).employmentID;
            if (empID == 0)
            {
                empID = _context.Employments.OrderBy(e => e.employmentDate).FirstOrDefault(e => e.personID == personID).employmentID;
            }

            return empID;
        }
        
    }
}
