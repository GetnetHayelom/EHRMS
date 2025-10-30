using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.EmployeeService
{
    public class EarningModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EarningModel(PIS2.Models.PISContext context)
        {
            _context = context;
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

        public decimal Gross { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (!string.IsNullOrEmpty(givenID) && id == null)
            {
                var emp = await _context.Employments
                    .FirstOrDefaultAsync(e => e.givenID == givenID);

                if (emp != null)
                {
                    
                    // Redirect to the Details page with employmentID
                    Console.WriteLine("############## The ID is == " + id);
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
            personModel = _context.Persons.Include(p => p.addressModel).FirstOrDefault( p=> p.personID == employmentModel.personID) ?? new personModel();
            AllowanceAssignments = _context.AllowanceAssignments
                .Include(aa=> aa.allowanceModel).Where(aa=>aa.employmentID == employmentModel.employmentID && aa.allowanceStatus == mainStatus.Active).ToList();
            OvertimeRecords = _context.OvertimeRecords
                .Include(aa => aa.overtimeModel).Where(otr => otr.employmentID == employmentModel.employmentID && otr.overtimeRecordStatus == overtimeStatus.Posted).ToList();

            Gross =(decimal) AllowanceAssignments.Sum(aa => aa.allowanceAssignmentAmount) + (decimal) OvertimeRecords.Sum(otr => otr.GetOtCost) + jobPlacementModel.jobPlacementSalary;

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
