using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Services;
using static System.Formats.Asn1.AsnWriter;
using PIS2.Views;
using PIS2.Models.Foundation;
using PIS2.Models.HR;

namespace PIS2.Pages.Leave
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        public CreateModel(PISContext ctx, Core methods)
        {
            _context = ctx;
            _core = methods;
        }
        [BindProperty(SupportsGet = true)]
        public employmentModel? Employment { get; set; }
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public int EmployeeID { get; set; }
        public List<leaveTypeModel> AllowedLeaveTypes { get; set; }
        public personModel Person { get; set; } = new personModel();
        public leaveDetail LeaveDetail { get; set; } =new leaveDetail();
        public List<leaveModel> Leaves { get; set; }
        public bool CheckProhibition { get; set; } = false;
        
        public async Task<IActionResult> OnGet(int? id)
        {
            var emps = await _context.Employments.Where(e => e.employmentStatus == mainStatus.Active).ToListAsync();
            if (!emps.ToList().Any()) 
            {
                TempData["message"] = ("Error", $"There is no an active employment record!");
                var referenceId = Guid.NewGuid().ToString("N")[..8].ToUpper();
                return RedirectToPage("Error",
                new
                {
                    code = "LEAVE-001",
                    title = referenceId+"-Unable to Create Leave Request",
                    message = "No active employment record was found for this employee."
                });
            }

            if (!id.HasValue)
            {
                var tempID = _context.UserView.FirstOrDefault(u => u.userName == User.Identity.Name);

                if (tempID != null)
                {
                    EmployeeID = tempID.employmentID ?? 0;
                    Employment = _context.Employments.FirstOrDefault(e => e.employmentID == EmployeeID);
                }               
            }
            else
            {
                Employment = await _context.Employments.FirstOrDefaultAsync(e => e.employmentID == id);
            }
            if (!string.IsNullOrEmpty(givenID))
            {
                Employment = await _context.Employments.FirstOrDefaultAsync(e => e.givenID == givenID);
                
                if (Employment != null)
                {
                    EmployeeID = Employment.employmentID;
                    CheckProhibition = await _core.CheckProhibition(EmployeeID, ProhibitionType.Leave);
                    
                    // Redirect to the Details page with employmentID
                    return RedirectToPage("Create", new { id = Employment.employmentID });
                }               
            }
            

            var leaveTypes = new List<leaveTypeModel>();
            leaveTypes = await _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Everyone" && lt.leaveTypeStatus == mainStatus.Active).ToListAsync();

            if (User.IsInRole("CLINIC"))
            {
                leaveTypes.AddRange(await _context.LeaveTypes.Where(lt => (lt.leaveAvailability == "Clinic" || lt.leaveAvailability == "Everyone") && lt.leaveTypeStatus == mainStatus.Active).ToListAsync());
            }

            if (User.IsInRole("HRPERSONNEL"))
            { 
                var hrLeaves = await _context.LeaveTypes.Where(lt => lt.leaveAvailability == "HR" && lt.leaveTypeStatus == mainStatus.Active).ToListAsync();    
                leaveTypes.AddRange(hrLeaves);
            }
            else
            {
                
            }

            if (Employment != null)
            {
                var isLeaveProhibited = await _core.CheckProhibition(Employment.employmentID, ProhibitionType.Leave);
                AllowedLeaveTypes = isLeaveProhibited ? leaveTypes.Where(l => l.leaveGroup != leaveGroup.AnnualLeave).ToList() : leaveTypes;
                EmployeeID = Employment.employmentID;
                TempData["MyNumber"] = EmployeeID;
                LeaveDetail = await _core.GetLeaveSummary(EmployeeID) != null ? await _core.GetLeaveSummary(EmployeeID) : await _core.GetLeaveSummary(EmployeeID);
                Leaves = await _context.Leaves.OrderByDescending(l => l.leaveRequestDate).Where(e => e.employmentID == EmployeeID).ToListAsync();
                Person = await _context.Persons.FirstOrDefaultAsync(e => e.personID == Employment.personID) ?? new personModel();
            }
            else 
            {
                AllowedLeaveTypes = leaveTypes;
            }

            ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes.Distinct().OrderBy(l => l.leaveTypeName), "leaveTypeID", "leaveTypeName");
            var empsList = await _context.Employments.Include(e => e.personModel).Where(e => e.employmentStatus == mainStatus.Active).ToListAsync();
            ViewData["employmentID"] = new SelectList(empsList, "employmentID", "givenID", id);

            return Page();
        }
        
        [BindProperty]
        public leaveModel Leave { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var isLeaveProhibited = await _core.CheckProhibition(Leave.employmentID, ProhibitionType.Leave);
            var lvGroup = await _context.LeaveTypes.FirstOrDefaultAsync(l => l.leaveTypeID == Leave.leaveTypeID);
            if (isLeaveProhibited && lvGroup.leaveGroup == leaveGroup.AnnualLeave)
            {
                TempData["message"] = ("Error", $"Employee is prohibited from requesting a annual leave request!");
                return Page();
            }
           
            ModelState.Clear();
            decimal maxWorkingDays =_core.GetWorkingDays(Leave.leaveStartDate, Leave.leaveEndDate);
            var ltype = await _context.LeaveTypes.FirstAsync(l => l.leaveTypeID == Leave.leaveTypeID);

            if (maxWorkingDays < Leave.leaveDays && ltype.leaveAvailability != "HR") 
            {
                TempData["message"] = ("Error", $"Requested date must be less than or equal to maximum working days. Possible working days={maxWorkingDays}");
                return Page();
            }
            Leave.employmentID = Convert.ToInt32(TempData["MyNumber"]);  Console.WriteLine("This is right here" + Leave.employmentID);
            Leave.modifiedBy = User.Identity?.Name!;
            Leave.leaveStatus = leaveStatus.Hold;

            var jobPlacement = await _context.JobPlacements.FirstOrDefaultAsync(jp => jp.jobPlacementStatus == mainStatus.Active && jp.employmentID == Employment.employmentID);

            Leave.ratePerHour = jobPlacement?.getJobRate() ?? 0;
            Leave.leaveCost = Leave.leaveDays * jobPlacement?.jobPlacementSalary/26 ?? 0;
            Leave.departmentID = jobPlacement?.departmentID ?? 0;

            Employment =await _context.Employments.FirstOrDefaultAsync(e => e.employmentID == Leave.employmentID)?? new employmentModel();
          
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Leaves.Add(Leave);
            await _context.SaveChangesAsync();

            //return RedirectToPage("./Index");
            return RedirectToPage("Details", new { id = Leave.leaveID });
        }
        
        public JsonResult OnPostCalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            return new JsonResult(_core.GetWorkingDays(startDate, endDate));
        }

    }
}
