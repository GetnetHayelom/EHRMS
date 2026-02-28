using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;
using PIS2.Views;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PIS2.Pages
{
    public class IndexModel : PageModel
        
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly IWebHostEnvironment _environment;
        [BindProperty]
        public List<personModel>? People { get; set; } = default!;
       
        //[BindProperty]
        public employmentModel? Employment { get; set; } = default!;
        [BindProperty]
        public List<employmentModel>? Employments { get; set; } = default!;
        //[BindProperty]
        public personModel Person { get; set; } = default!;
        [BindProperty]
        public List<employmentModel>? PersonEmployments { get; set; } = default!;
        [BindProperty]
        public List<employmentHistoryModel>? EmploymentHistories { get; set; } = default!;
        [BindProperty]
        public ICollection<leaveModel>? Leaves { get; set; } = new List<leaveModel>();
        [BindProperty]
        public List<jobPlacementModel>? JobPlacements { get; set; } = default!;
        public employmentModel ActiveEmployment { get; set; } = default!;
      
        public jobPlacementModel JobPlacement { get; set; }
        public List<overtimeRecordModel>? Overtimes { get; set; }=default!;
        public leaveDetail? LeaveDetail { get; set; } = new leaveDetail();
        public List<leaveTypeModel> AllowedLeaveTypes { get; set; } = new List<leaveTypeModel>();
        public bool isSelf { get; set; } = false;
        public Core methods { get; set; } = default!;
        public bool PhotoExists { get; set; }
        public bool IsLeaveAllow { get; set; } = true;
        public bool IsOvertimeAllow { get; set; } = true;
        public bool IsGuarantyAllow { get; set; } = true;
        public List<NoticeModel> ActiveNoticesForCarousel { get; set; } = new List<NoticeModel>();
        public List<EvalSingleEmployeeReport> EvalReport { get; set; }
        public List<serviceRequestModel> Requests { get; set; } = new List<serviceRequestModel>();
        public IndexModel(PISContext ctx, Core methods, IWebHostEnvironment environment)
        {
            _context = ctx;
            _core = methods;
            _environment = environment;
        }
        //public IList<personModel> Persons { get; set; }
        public async Task OnGetAsync()
        {
           
            //Person = new personModel();
            People = await _context.Persons.OrderBy(p => p.personFirstName).ThenBy(p => p.personFatherName).ThenBy(p => p.personLastName).ToListAsync();
            Employments = await _context.Employments.ToListAsync();

            ActiveNoticesForCarousel = await _context.Notices
                .Where(n => n.IsActive &&
                            (!n.ExpiryDate.HasValue || n.ExpiryDate.Value.Date >= DateTime.Now.Date))
                .OrderByDescending(n => n.noticePriority)
                .ThenByDescending(n => n.DatePosted)
                .ToListAsync();

            if (!searchID.IsNullOrEmpty() || !searchName.IsNullOrEmpty())
            {
                var emplymnt = await _context.Employments.FirstOrDefaultAsync(e => e.givenID == searchID);
                //var PersonID = int.Parse(TempData["PersonID"].ToString());
               
                Person = await _context.Persons.FirstOrDefaultAsync(p => p.personID == emplymnt.employmentID);
                if (Person != null)
                {
                    await getPerson();
                }
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.userName.ToLower() == User.Identity.Name!.ToLower());
                
                if(user != null)
                {   
                    Person = await _context.Persons?.FirstOrDefaultAsync(p => p.personID == user.personID) ?? new personModel();
                    Employment = await _context.Employments.FirstOrDefaultAsync(e => e.personID == Person.personID);
                    
                    await getPerson();
                    
                }
                else { Person = new personModel(); }
                
            }
            if(Employment != null)
            {
                EvalReport = _core.GetSingleEvaluationReport(Employment.employmentID);
                Requests = await _context.ServiceRequests.Where(s => s.employmentID == Employment.employmentID).ToListAsync();
            }
            

        }
        [BindProperty]
        public string searchID { get; set; } = default!;
        [BindProperty]
        public string Department { get; set; } = default!;
        public async Task<IActionResult> OnPostSearchID()
        {
            await getNotices();
            
            if (!string.IsNullOrEmpty(searchID))
            {
                Person = await _context.Persons.Where(p => p.Employments.Any(e => e.givenID == searchID))
                    .FirstOrDefaultAsync();
               
                if (Person == null)
                {
                    TempData["message"] = ("Error",$"No employment found with employment ID {searchID}");
                }
                else
                {
                    await getPerson();
                }
                await SetOptionsAsync();
            }
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            
            return Page();
        }
        [BindProperty]
        public string searchName { get; set; } = default!;
        public async Task<IActionResult> OnPostSearchName()
        {
            await getNotices();

            if (!searchName.IsNullOrEmpty())
            {

                Person = await _context.Persons
                    .FirstOrDefaultAsync(p => (p.personFirstName+" "+p.personFatherName+" "+p.personLastName).Contains(searchName));
                
                if (Person == null)
                {
                    TempData["message"] = ("Error", $"No person found with Name {searchName}");
                }
                else
                {
                   await getPerson();
                }
               await SetOptionsAsync();
            }
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            
            return Page();
        }

        //Leave Request Save
       
        [BindProperty]
        public int employmentID { get; set; }
        [BindProperty]
        public leaveModel Leave { get; set; } = default!;
        [BindProperty]
        public int personID { get; set; }
       
        [BindProperty]
        public overtimeRecordModel overtimeRecordModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
    
        public async Task SetOptionsAsync()
        {
            var leaveTypes = new List<leaveTypeModel>();
            if (User.IsInRole("MIE\\PMS_CLINIC"))
            {
                leaveTypes =await _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Clinic" || lt.leaveAvailability == "Everyone").ToListAsync();
            }
            else if (User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                leaveTypes =await _context.LeaveTypes.Where(lt => lt.leaveAvailability == "HR" || lt.leaveAvailability == "Everyone").ToListAsync();
            }
            else
            {
                leaveTypes =await _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Everyone").ToListAsync();
            }
            leaveTypes = leaveTypes.Where(lt => lt.leaveTypeStatus == mainStatus.Active).ToList();
            AllowedLeaveTypes = leaveTypes;
            var empsList = await _context.Employments.ToListAsync();
            var ots = await _context.Overtimes.ToListAsync();

            ViewData["employmentID"] = new SelectList(empsList, "employmentID", "givenID");
            ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes, "leaveTypeID", "leaveTypeName");
            ViewData["overtimeID"] = new SelectList(ots, "overtimeID", "overtimeName");
        }
        public async Task<IActionResult> GetEmploymentHistory(int personID)
                {
                    List<employmentModel> employment= await _context.Employments.Where(p => p.personID == personID).ToListAsync();
            
                    if (employment == null)
                    {
                        return NotFound();
                    }

                    return Page();
                }
        //handle service request

        [BindProperty]
        public serviceRequestModel requestModel { get; set; }
        public class RequestDto
        {
            public string requestType { get; set; }
        }

        /// <summary>
        /// Handles Employment Service Requests
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostHandleRequestAsync([FromBody] RequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.requestType))
            {
                return new JsonResult(new { success = false, message = "Invalid request type." });
            }

            var currentUserName = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUserName))
                return new JsonResult(new { success = false, message = "User not authenticated." });

            var usr = await _context.Users.FirstOrDefaultAsync(u => u.userName.ToLower() == currentUserName.ToLower());
            if (usr == null)
                return new JsonResult(new { success = false, message = "User record not found." });

            var emp = await _context.Employments.FirstOrDefaultAsync(e => e.personID == usr.personID && e.employmentStatus == mainStatus.Active);
            if (emp == null || emp.employmentID == 0)
                return new JsonResult(new { success = false, message = "Employment ID not found." });

            var employmentID = emp.employmentID;

            if (!Enum.TryParse<ServiceRequestTypes>(request.requestType, true, out var service))
                return new JsonResult(new { success = false, message = "Invalid request type." });

            bool exists =await _context.ServiceRequests.AnyAsync(r =>
                r.employmentID == emp.employmentID &&
                r.requestedService == service &&
                r.serviceRequestStatus == ServiceRequestStatus.Hold);

            if (exists)
            {
                return new JsonResult(new { success = false, message = "You already have a pending request." });
            }

            requestModel = new serviceRequestModel {
                employmentID = employmentID,
                serviceRequestDate = DateTime.Now,
                requestedService = service,
                serviceRequestStatus = ServiceRequestStatus.Hold, 
                modifiedBy = currentUserName };

            _context.ServiceRequests.Add(requestModel);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        public bool checkPic(int personID)
        {
            //Check if photo is available
            var imagesFolder = Path.Combine(_environment.WebRootPath, "images");
            var fileName = $"{personID}.jpg";
            var filePath = Path.Combine(imagesFolder, fileName);

            return System.IO.File.Exists(filePath);
        }

        public async Task getPerson()
        {
            PhotoExists = checkPic(Person.personID);

            TempData["PersonID"] = Person.personID;

            var theSelf = await _context.Users.FirstOrDefaultAsync(u => u.userName == User.Identity.Name);
            isSelf = theSelf?.personID == Person.personID ? true : false;
            PersonEmployments = await _context.Employments
                .Include(e => e.Leaves)
                .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                .Include(e => e.EmploymentHistories).Where(e => e.personID == Person.personID).ToListAsync();

            if (PersonEmployments != null && PersonEmployments.Any())
            {
                Employment = PersonEmployments.Any(p => p.employmentStatus == mainStatus.Active)?
                    PersonEmployments.First(p => p.employmentStatus == mainStatus.Active) : PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                
                if (Employment != null)
                {
                    searchID = string.IsNullOrEmpty(Employment.givenID) ? Employment.givenID : "";
                    Leaves = Employment?.Leaves?.OrderByDescending(l => l.leaveRequestDate).ToList();

                    LeaveDetail =await _core.GetLeaveSummary(Employment.employmentID);
                    Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                        .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                    JobPlacement = _context.JobPlacements
                        .Include(jp => jp.departmentModel)
                        .Include(jp => jp.jobModel)
                        .Include(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                        .OrderBy(jp => jp.jobPlacementDate).LastOrDefault(jp => jp.employmentID == Employment.employmentID) ?? new jobPlacementModel();
                   
                    if (Employment.employmentStatus == mainStatus.Active)
                    {
                        IsLeaveAllow = await _core.CheckProhibition(Employment.employmentID, ProhibitionType.Leave) ? false : true;

                        IsOvertimeAllow = await _core.CheckProhibition(Employment.employmentID, ProhibitionType.Overtime) ? false : true;

                        IsGuarantyAllow = await _core.CheckProhibition(Employment.employmentID, ProhibitionType.Guaranty) ? false : true;

                    }
                   
                }

            }

        }

        public async Task getNotices()
        {
            ActiveNoticesForCarousel = await _context.Notices
                .Where(n => n.IsActive &&
                            (!n.ExpiryDate.HasValue || n.ExpiryDate.Value.Date >= DateTime.Now.Date) && n.noticeStatus == NoticeStatus.Posted)
                .OrderByDescending(n => n.noticePriority)
                .ThenByDescending(n => n.DatePosted)
                .ToListAsync();
        }
    }
}