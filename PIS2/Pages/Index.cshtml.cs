using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Models.Foundation;
using PIS2.Models.HR;
using PIS2.Services;
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
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<userModel> _userManager;

        public IndexModel(PISContext ctx, Core methods, IWebHostEnvironment environment, ILogger<IndexModel> logger, UserManager<userModel> userManager)
        {
            _context = ctx;
            _core = methods;
            _environment = environment;
            _logger = logger;
            _userManager = userManager;
        }
        
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
        public List<serviceRequestTypeModel> RequestTypes { get; set; } = new List<serviceRequestTypeModel>();
        
        //public IList<personModel> Persons { get; set; }
        public async Task OnGetAsync()
        {

            await SetOptionsAsync();
 

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
                //var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == User.Identity.Name!.ToLower());
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
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
            People = await _context.Persons.OrderBy(p => p.personFirstName).ThenBy(p => p.personFatherName).ThenBy(p => p.personLastName).ToListAsync();
            Employments = await _context.Employments.ToListAsync();

            var reqTypes = await _context.ServiceRequestTypes.Where(s => s.serviceRequestTypeStatus == mainStatus.Active).ToListAsync();
            RequestTypes = reqTypes;
            
            var empsList = await _context.Employments.ToListAsync();

            ViewData["employmentID"] = new SelectList(empsList, "employmentID", "givenID");

            await getNotices();
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
            public int requestType { get; set; }
        }

        /// <summary>
        /// Handles Employment Service Requests
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostHandleRequestAsync([FromBody] RequestDto request)
        {
            if (request == null || request.requestType ==0)
            {
                return new JsonResult(new { success = false, message = "Invalid request type." });
            }

            
            if (string.IsNullOrEmpty(User!.Identity.Name))
                return new JsonResult(new { success = false, message = "User not authenticated." });

            var usr = await _userManager.GetUserAsync(User);
            //var usr = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName.ToLower() == currentUserName.ToLower());
            if (usr == null)
                return new JsonResult(new { success = false, message = "User record not found." });

            var emp = await _context.Employments.AsNoTracking().FirstOrDefaultAsync(e => e.personID == usr.personID && e.employmentStatus == mainStatus.Active);
            if (emp == null || emp.employmentID == 0)
                return new JsonResult(new { success = false, message = "Employment ID not found." });

            var employmentID = emp.employmentID;

            //if (!Enum.TryParse<serviceRequestTypeModel>(request.requestType, true, out var service))
            //    return new JsonResult(new { success = false, message = "Invalid request type." });

            bool exists =await _context.ServiceRequests.AnyAsync(r =>
                r.employmentID == emp.employmentID &&
                r.serviceRequestTypeID == request.requestType &&
                r.serviceRequestStatus == ServiceRequestStatus.Hold);

            if (exists)
            {
                return new JsonResult(new { success = false, message = "You already have a pending request." });
            }
            var reqType = await _context.ServiceRequestTypes.AsNoTracking().FirstOrDefaultAsync(s => s.serviceRequestTypeID == request.requestType);
            if (reqType == null)
            {
                return new JsonResult(new { success = false, message = "Request type not found." });
            }
            if (reqType.serviceRequestTypeName.Contains("Guaranty") && !IsGuarantyAllow)
            {
                return new JsonResult(new { success = false, message = "Guaranty service not allowed." });
            }
            requestModel = new serviceRequestModel {
                employmentID = employmentID,
                serviceRequestDate = DateTime.Now,
                serviceRequestTypeID = request.requestType,
                serviceRequestStatus = ServiceRequestStatus.Hold, 
                modifiedBy = User.Identity.Name };

            _context.ServiceRequests.Add(requestModel);
            await _context.SaveChangesAsync();          

            if (reqType.serviceRequestTypeName.Contains("Termination"))
            {
                return new JsonResult(new
                {
                    success = true,
                    redirect = Url.Page("/Termination/Create", new { id = employmentID })
                });
            }
            if (reqType.serviceRequestTypeName.Contains("Guaranty"))
            {
                return new JsonResult(new
                {
                    success = true,
                    redirect = Url.Page("/Guaranty/Create", new { id = employmentID })
                });
            }
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

            var theSelf = await _userManager.GetUserAsync(User);
            //var theSelf = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
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
            var items = await _context.Notices.AsNoTracking()
                .Where(n => n.IsActive &&
                            (!n.ExpiryDate.HasValue || n.ExpiryDate.Value.Date >= DateTime.Now.Date) && n.noticeStatus == NoticeStatus.Posted)
                .OrderByDescending(n => n.noticePriority)
                .ThenByDescending(n => n.DatePosted)
                .ToListAsync();
            ActiveNoticesForCarousel = items.ToList();
        }
    }
}