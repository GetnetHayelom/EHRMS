using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PIS2.Pages
{
    public class IndexModel : PageModel
        
    {
       
        private readonly ILogger<IndexModel> _logger;
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
            

            if (!searchID.IsNullOrEmpty() || !searchName.IsNullOrEmpty())
            {
                var PersonID = int.Parse(TempData["PersonID"].ToString());
               
                Person = _context.Persons.Find(PersonID);
                if (Person != null)
                {
                    

                    //TempData["SuccessMessage"] = $"No person found with Name {searchName}";
                    isSelf = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.personID == Person.personID ? true : false;
                    searchName = Person.personFullName;
                    PersonEmployments = await _context.Employments
                        .Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)                       
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel).ThenInclude(d => d.companyModel)
                        .Include(e => e.EmploymentHistories)
                        .Include(e => e.OvertimeRecords).Where(e => e.personID == Person.personID).ToListAsync();
                    if (PersonEmployments != null && PersonEmployments.Any())
                    {
                        Employment = PersonEmployments.First(pe => pe.employmentStatus == mainStatus.Active)?? PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault(); 
                        //Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                        if (Employment != null)
                        {
                            Leaves = Employment.Leaves.OrderByDescending(l => l.leaveRequestDate).ToList();
                            JobPlacements = await _context.JobPlacements.OrderBy(j => j.jobPlacementDate).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
                            Overtimes =await _context.OvertimeRecords.Include(ot=> ot.overtimeModel)
                                .Include(ot=>ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                            JobPlacement = _context.JobPlacements
                                .Include(jp => jp.departmentModel)
                                .Include(jp => jp.jobModel)
                                .Include(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                                .OrderBy(jp => jp.jobPlacementDate).LastOrDefault(jp => jp.employmentID == Employment.employmentID) ?? new jobPlacementModel();

                            if (Employment.employmentStatus == mainStatus.Active)
                            {
                                if(_context.Prohibitions.Where(p => p.prohibitionType == ProhibitionType.Leave && p.prohibitionStatus== mainStatus.Active).Any(p => p.employmentID == Employment.employmentID))
                                {
                                    IsLeaveAllow = false;
                                }
                                if (_context.Prohibitions.Where(p => p.prohibitionType == ProhibitionType.Overtime && p.prohibitionStatus == mainStatus.Active).Any(p => p.employmentID == Employment.employmentID))
                                {
                                    IsOvertimeAllow = false;
                                }
                                if (_context.Prohibitions.Where(p => p.prohibitionType == ProhibitionType.Guaranty && p.prohibitionStatus == mainStatus.Active).Any(p => p.employmentID == Employment.employmentID))
                                {
                                    IsGuarantyAllow = false;
                                }
                            }
                        }

                        

                    }
                    PhotoExists = checkPic(Person.personID);

                }
            }
            else
            {
                var user = _context.Users.FirstOrDefault(u => u.userName.ToLower() == User.Identity.Name!.ToLower());
                if(user != null)
                {
                    Person = _context.Users.Where(u => u.userName.ToLower() == User.Identity.Name!.ToLower()).Select(u => u.personModel)?.First()?? new personModel();
                    isSelf = _context.Users.FirstOrDefault(u => u.userName.ToLower() == User.Identity.Name!.ToLower())?.personID == Person.personID ? true : false;
                    PersonEmployments = _context.Employments.OrderBy(e => e.employmentDate).Include(e => e.Leaves)
                            .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                            .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                            .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                            .Include(e => e.EmploymentHistories).Where(e => e.personID == Person.personID).ToList();
                    if (PersonEmployments != null && PersonEmployments.Any())
                    {
                        Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                    
                        if (Employment != null)
                        {
                            Leaves = Employment.Leaves.ToList();

                            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
                            Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                                .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                            JobPlacement = _context.JobPlacements
                                .Include(jp => jp.departmentModel)
                                .Include(jp => jp.jobModel)
                                .Include(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                                .OrderBy(jp => jp.jobPlacementDate).LastOrDefault(jp => jp.employmentID == Employment.employmentID) ?? new jobPlacementModel();

                        }

                    }
                    PhotoExists = checkPic(Person.personID);
                }
                else { Person = new personModel(); }
                
            }
         
        }
        [BindProperty]
        public string searchID { get; set; } = default!;
        [BindProperty]
        public string Department { get; set; } = default!;
        public async Task<IActionResult> OnPostSearchID()
        {
            Console.WriteLine($"Search ID: {searchID}");
            

            if (!string.IsNullOrEmpty(searchID))
            {

                
                Person = await _context.Persons.Where(p => p.Employments.Any(e => e.givenID == searchID))
                    .FirstOrDefaultAsync();
               
                if (Person == null)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                }
                else
                {
                    TempData["PersonID"] = Person.personID;
                    isSelf = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.personID == Person.personID ? true : false;
                    PersonEmployments = await _context.Employments
                        .Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                        .Include(e => e.EmploymentHistories).Where(e => e.personID == Person.personID).ToListAsync(); 
                    if (PersonEmployments != null && PersonEmployments.Any())
                    {
                        Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                        if (Employment != null)
                        {                                        
                            Leaves = Employment.Leaves.OrderByDescending(l=> l.leaveRequestDate).ToList();
                           
                            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
                            Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                                .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                            JobPlacement = _context.JobPlacements
                                .Include(jp => jp.departmentModel)
                                .Include(jp => jp.jobModel)
                                .Include(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                                .OrderBy(jp => jp.jobPlacementDate).LastOrDefault(jp => jp.employmentID == Employment.employmentID) ?? new jobPlacementModel();
                        }
                        

                    }

                }
                
            }
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            
            return Page();
        }
        [BindProperty]
        public string searchName { get; set; } = default!;
        public async Task<IActionResult> OnPostSearchName()
        {           

            if (!searchName.IsNullOrEmpty())
            {

                Person = await _context.Persons
                    .FirstOrDefaultAsync(p => (p.personFirstName+" "+p.personFatherName+" "+p.personLastName).Contains(searchName));
                
                if (Person == null)
                {
                    TempData["SuccessMessage"] = $"No person found with Name {searchName}";
                }
                else
                {
                    //Check if photo is available
                    var imagesFolder = Path.Combine(_environment.WebRootPath, "images");
                    var fileName = $"{Person.personID}.jpg";
                    var filePath = Path.Combine(imagesFolder, fileName);

                    PhotoExists = System.IO.File.Exists(filePath);
                    TempData["PersonID"] = Person.personID;
                    isSelf = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name)?.personID == Person.personID? true :false;
                    PersonEmployments = await _context.Employments
                        .Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                        .Include(e => e.EmploymentHistories).Where(e => e.personID == Person.personID).ToListAsync();
                    if (PersonEmployments != null && PersonEmployments.Any())
                    {
                        Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                        if (Employment != null)
                        {
                            JobPlacements = await _context.JobPlacements.Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                            Leaves = await _context.Leaves.Where(l => l.employmentID == Employment.employmentID).OrderByDescending(l => l.leaveRequestDate).ToListAsync(); //Employment.Leaves.ToList();
                            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
                            Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                                .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();

                            JobPlacement = _context.JobPlacements
                                .Include(jp => jp.departmentModel)
                                .Include(jp => jp.jobModel)
                                .Include(j => j.jobStepModel).ThenInclude(js => js.jobGradeModel)
                                .OrderBy(jp => jp.jobPlacementDate).LastOrDefault(jp => jp.employmentID == Employment.employmentID) ?? new jobPlacementModel();
                        }
                        

                    }

                }
                SetOptions();
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
    
        public void SetOptions()
        {
            var leaveTypes = new List<leaveTypeModel>();
            if (User.IsInRole("MIE\\PMS_CLINIC"))
            {
                leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Clinic" || lt.leaveAvailability == "Everyone").ToList();
            }
            else if (User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveAvailability == "HR" || lt.leaveAvailability == "Everyone").ToList();
            }
            else
            {
                leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Everyone").ToList();
            }
            leaveTypes = leaveTypes.Where(lt => lt.leaveTypeStatus == mainStatus.Active).ToList();
            AllowedLeaveTypes = leaveTypes;
            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes, "leaveTypeID", "leaveTypeName");
            ViewData["overtimeID"] = new SelectList(_context.Overtimes, "overtimeID", "overtimeName");
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
        public async Task<IActionResult> OnPostHandleRequestAsync([FromBody] RequestDto request)
        {
            var currentUserName = User.Identity?.Name;
            var requestType = request.requestType;
            var employmentID = _context.Employments
                .Where(e => e.personID == _context.Users
                    .Where(u => u.userName == currentUserName)
                    .Select(u => u.personID)
                    .FirstOrDefault())
                .Select(e => e.employmentID)
                .FirstOrDefault();

            if (employmentID == 0)
            {
                return new JsonResult(new { success = false, message = "Employment ID not found." });
            }

            requestModel = new serviceRequestModel
            {
                employmentID = employmentID,
                serviceRequestDate = DateTime.Now,
                serviceRequestStatus = ServiceRequestStatus.Hold,
                modifiedBy = currentUserName
            };

            if (requestType == "Exprience")
            {
                requestModel.requestedService = ServiceRequestTypes.Exprience;
            }
            else if (requestType == "Termination")
            {
                requestModel.requestedService = ServiceRequestTypes.Termination;
            }
            else
            {
                return new JsonResult(new { success = false, message = "Invalid request type." });
            }

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

    }
}