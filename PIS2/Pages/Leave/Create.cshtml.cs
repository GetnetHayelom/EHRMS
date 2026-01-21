using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.Leave
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly Core _core;
        public CreateModel(PISContext ctx, Core methods)
        {
            _context = ctx;
            _core = methods;
        }
        [BindProperty(SupportsGet = true)]
        public employmentModel Employment { get; set; } = new employmentModel();
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public int EmployeeID { get; set; }
        public List<leaveTypeModel> AllowedLeaveTypes { get; set; }
        public personModel Person { get; set; } = new personModel();
        public leaveDetail LeaveDetail { get; set; } =new leaveDetail();
        public List<leaveModel> Leaves { get; set; }
        public bool CheckProhibition { get; set; } = false;
        
        public async Task<IActionResult> OnGet(int id)
        {
            if (id==0)
            {
                var tempID = await _context.Users.Include(u => u.personModel)
                    .ThenInclude(p => p.Employments).FirstOrDefaultAsync(u => u.userName == User.Identity.Name);

                id =tempID.personModel.Employments.FirstOrDefault(e => e.employmentStatus == mainStatus.Active).employmentID;
                Employment = _context.Employments.FirstOrDefault(e => e.employmentID == id) ?? new employmentModel();
                EmployeeID = id;
                if (id == 0)
                {
                    Console.WriteLine("ID is still 0");
                    return NotFound();
                }
                    
            }
            else
            {
                Employment = await _context.Employments.FirstOrDefaultAsync(e => e.employmentID == id) ?? new employmentModel();
                if (Employment.employmentID == 0)
                {
                    return NotFound();
                }
            }
            if (!string.IsNullOrEmpty(givenID))
            {
                Employment = await _context.Employments.FirstOrDefaultAsync(e => e.givenID == givenID) ?? new employmentModel();
                Console.WriteLine("This is right here!!!!!!!!!!!");
                if (Employment != null)
                {
                    
                    id = Employment.employmentID;
                    EmployeeID = id;
                    CheckProhibition = await _core.CheckProhibition(EmployeeID, ProhibitionType.Leave);
                    Console.WriteLine("ID is set from givenID" + id);
                    // Redirect to the Details page with employmentID
                    return RedirectToPage("Create", new { id = Employment.employmentID });
                }               
            }
            

            var leaveTypes = new List<leaveTypeModel>();
            
            if (User.IsInRole("MIE\\PMS_CLINIC"))
            {
                leaveTypes = await _context.LeaveTypes.Where(lt => (lt.leaveAvailability == "Clinic" || lt.leaveAvailability == "Everyone") && lt.leaveTypeStatus == mainStatus.Active).ToListAsync();
            }
            else if (User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                leaveTypes = await _context.LeaveTypes.Where(lt => (lt.leaveAvailability == "HR" || lt.leaveAvailability == "Everyone") && lt.leaveTypeStatus == mainStatus.Active).ToListAsync();
            }
            else
            {
                leaveTypes = await _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Everyone" && lt.leaveTypeStatus == mainStatus.Active).ToListAsync();
            }
            leaveTypes = leaveTypes.Where(lt => lt.leaveTypeStatus == mainStatus.Active).ToList();
            AllowedLeaveTypes = leaveTypes;
            ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes, "leaveTypeID", "leaveTypeName");
            var empsList = await _context.Employments.Include(e => e.personModel).Where(e => e.employmentStatus == mainStatus.Active).ToListAsync();
            ViewData["employmentID"] = new SelectList(empsList, "employmentID", "givenID");

            EmployeeID = Employment.employmentID;
            TempData["MyNumber"] = EmployeeID;
            
            LeaveDetail = await _core.GetLeaveSummary(id) != null? await _core.GetLeaveSummary(id):await _core.GetLeaveSummary(EmployeeID);
            Leaves =await _context.Leaves.OrderByDescending(l => l.leaveRequestDate).Where(e => e.employmentID == EmployeeID).ToListAsync();
            Person =await _context.Persons.FirstOrDefaultAsync(e => e.personID == Employment.personID)?? new personModel();
            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID", id);
            Console.WriteLine("################# Employee ID is " + EmployeeID);
            return Page();
        }
        
        [BindProperty]
        public leaveModel Leave { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var prohibition= await _context.Prohibitions.Where(p => p.employmentID == Leave.employmentID && p.prohibitionStatus == mainStatus.Active).ToListAsync();

            if(prohibition.Any(p => p.prohibitionType == ProhibitionType.Leave))
            {
                TempData["message"] = ("Error", $"Employee is prohibited from requesting a annual leave request!");
                return Page();
            }
           
            ModelState.Clear();
            decimal maxWorkingDays =_core.GetWorkingDays(Leave.leaveStartDate, Leave.leaveEndDate);

            if (maxWorkingDays > (Leave.leaveEndDate - Leave.leaveStartDate).Days) 
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

            if(await _core.CheckProhibition(Leave.employmentID, ProhibitionType.Leave)) return Page();
          
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
