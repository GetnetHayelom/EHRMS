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
        public List<overtimeRecordModel>? Overtimes { get; set; }=default!;
        public leaveDetail? LeaveDetail { get; set; } = new leaveDetail();
        public List<leaveTypeModel> AllowedLeaveTypes { get; set; } = new List<leaveTypeModel>();
        public userModel UserM { get; set; } = default;
        public Core methods { get; set; } = default!;
        public IndexModel(PISContext ctx, Core methods)
        {
            _context = ctx;
            _core = methods;
        }
        //public IList<personModel> Persons { get; set; }
        public async Task OnGetAsync()
        {
           
            //Person = new personModel();
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            

            if (!searchID.IsNullOrEmpty() || !searchName.IsNullOrEmpty())
            {
                var PersonID = int.Parse(TempData["PersonID"].ToString());
               
                Person = _context.Persons.Find(PersonID);
                if (Person != null)
                {
                    //TempData["SuccessMessage"] = $"No person found with Name {searchName}";
               
                    searchName = Person.personFullName;
                    PersonEmployments = await _context.Employments
                        .Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel).ThenInclude(d => d.companyModel)
                        .Include(e => e.EmploymentHistories)
                        .Include(e => e.OvertimeRecords).Where(e => e.personID == Person.personID).ToListAsync();
                    if (PersonEmployments != null && PersonEmployments.Any())
                    {
                        Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                        if (Employment != null)
                        {
                            Leaves = Employment.Leaves.ToList();
                            JobPlacements = await _context.JobPlacements.Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
                            Overtimes =await _context.OvertimeRecords.Include(ot=> ot.overtimeModel)
                                .Include(ot=>ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                        }

                    }


                }
            }
            else
            {
                Person = _context.Users.Where(u => u.userName.ToLower() == User.Identity.Name!.ToLower()).Select(u => u.personModel)?.First()?? new personModel();
                //UserM = _context.Users.Where(u => u.userName.ToLower() == User.Identity.Name!.ToLower()).First();
                PersonEmployments = _context.Employments.OrderBy(e => e.employmentDate).Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
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

                    }

                }
            }
            SetOptions();
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
                  
                    PersonEmployments = await _context.Employments
                        .Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(e => e.EmploymentHistories).Where(e => e.personID == Person.personID).ToListAsync(); 
                    if (PersonEmployments != null && PersonEmployments.Any())
                    {
                        Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                        if (Employment != null)
                        {
                            
                           // List<employmentHistoryModel> histories =_context.EmploymentHistories.Include(e => e.employmentTypeModel).Where(e => e.employmentID == Employment.employmentID).ToList();

                            Leaves = Employment.Leaves.ToList();// _context.Leaves.Where(l => l.employmentID == Employment.employmentID).ToList();
                            //JobPlacements = await _context.JobPlacements.OrderByDescending(jp => jp.jobPlacementDate).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                           
                            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
                            Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                                .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();

                        }

                    }


                }
                SetOptions();
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
                    TempData["PersonID"] = Person.personID;

                    PersonEmployments = await _context.Employments
                        .Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .Include(e => e.EmploymentHistories).Where(e => e.personID == Person.personID).ToListAsync();
                    if (PersonEmployments != null && PersonEmployments.Any())
                    {
                        Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
                        if (Employment != null)
                        {
                            JobPlacements = await _context.JobPlacements.Where(l => l.employmentID == Employment.employmentID).ToListAsync();
                            Leaves = await _context.Leaves.Where(l => l.employmentID == Employment.employmentID).ToListAsync(); //Employment.Leaves.ToList();
                            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
                            Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                                .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
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
        public async Task<IActionResult> OnPostCreateLeave()
        {
            if(employmentID == 0 || _context.Employments.FirstOrDefault(e => e.employmentID == employmentID) == null)
            {
                TempData["SuccessMessage"] = "Employment not found or provided";
                return Page();
            }
            if (_context.Employments.FirstOrDefault(e => e.employmentID == employmentID)?.employmentStatus == mainStatus.Inactive)
            {
                TempData["SuccessMessage"] = "Could not save leave reaquest. Employment status must me active.";
                return Page();
            }
            ModelState.Remove(nameof(searchID));
            ModelState.Remove(nameof(searchName));
            ModelState.Clear();
            Leave.modifiedBy = User.Identity?.Name!;
            Leave.employmentID = employmentID;
            Leave.leaveStatus = leaveStatus.Hold;
            Leave.ratePerHour = _context.JobPlacements.FirstOrDefault(jp => jp.jobPlacementStatus == mainStatus.Active && jp.employmentID == employmentID)?.getJobRate() ?? 0;
            //TempData["LeaveID"] = null;
            //if (!TryValidateModel(Leave, nameof(Leave)))
            if(!ModelState.IsValid)
            {
                // Log or display errors for debugging
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                // Optionally pass errors to the view for display
                TempData["SuccessMessage"] = "Leave is not valid.";            
            }
            else
            {
                try
                {
                    if (TempData["LeaveID"] == null)
                    {
                        //add leave to database
                        _context.Leaves.Add(Leave);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = $"Leave Saved with ID {Leave.leaveID}";
                        TempData["LeaveID"] = Leave.leaveID;
                    }    

                }
                catch(DbUpdateException ex)
                {
                    // Check if the exception is an inner SqlException
                    if (ex.InnerException is SqlException sqlEx)
                    {
                        // access the message from SQL Server
                        string sqlErrorMessage = sqlEx.Message;
                        if(sqlEx.Number ==2627 || sqlEx.Number == 2601)
                        {
                            ModelState.AddModelError(string.Empty, "Error: Duplicate Record!");
                        }
                       else if (sqlEx.Number == 547)
                        {
                            ModelState.AddModelError(string.Empty, "Error: Constraint Violation!");
                        }
                        else { // SQL error message to ModelState
                        ModelState.AddModelError(string.Empty, "Database error: " + sqlErrorMessage);
                        }
                        
                    }
                    else
                    {
                        // Handle other types of exceptions
                        ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                    }
                }
                TempData["PersonID"] = personID;
                //searchID = Employment.givenID;
            }           
            SetOptions();

            // Pass the Leave ID and keep the form open
            TempData["KeepLeaveRequest"] = true;
            Person= await _context.Persons.FirstOrDefaultAsync(p=> p.personID == personID);
            PersonEmployments = await _context.Employments
                .Include(e => e.EmploymentHistories).Where(e => e.personID == personID).ToListAsync();
            Employment = PersonEmployments.OrderBy(e => e.employmentDate).LastOrDefault();
            Leaves = await _context.Leaves.Where(l => l.employmentID == Employment.employmentID).ToListAsync();
            JobPlacements = await _context.JobPlacements.Where(l => l.employmentID == Employment.employmentID).ToListAsync();
            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                                .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
            return Page();
        }
        [BindProperty]
        public overtimeRecordModel overtimeRecordModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostCreateOvertimeRecord()
        {
            ModelState.Remove(nameof(searchID));
            ModelState.Remove(nameof(searchName));
            if (!ModelState.IsValid)
            {
                // Log or display errors for debugging
                foreach (var error in ModelState)
                    {
                        Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                TempData["SuccessMessage"] = "Overtime is not valid.";
                return Page();
            }
            else
            {
                overtimeRecordModel.modifiedBy = User.Identity?.Name!;
                _context.OvertimeRecords.Add(overtimeRecordModel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Overtime Saved with ID {overtimeRecordModel.overtimeRecordID}";
                TempData["OvertimeID"] = overtimeRecordModel.overtimeRecordID;
                TempData["PersonID"] = personID;
            }


            SetOptions();

            // Pass the Leave ID and keep the form open
            TempData["keepOvertime"] = true;
            Person = await _context.Persons.FirstOrDefaultAsync(p => p.personID == personID);
            PersonEmployments = await _context.Employments.Where(e => e.personID == personID).ToListAsync();
            Employment = PersonEmployments.FirstOrDefault(e => e.employmentStatus == mainStatus.Active);
            Leaves = await _context.Leaves.Where(l => l.employmentID == Employment.employmentID).ToListAsync();
            JobPlacements = await _context.JobPlacements.Where(l => l.employmentID == Employment.employmentID).ToListAsync();
            LeaveDetail = _core.GetLeaveSummary(Employment.employmentID);
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();
            Overtimes = await _context.OvertimeRecords.Include(ot => ot.overtimeModel)
                                .Include(ot => ot.OvertimeHistories).Where(l => l.employmentID == Employment.employmentID).ToListAsync();
            return Page();
        }
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
        //public async Task<ActionResult<IEnumerable<leaveModel>>> GetLeaves()
        //{
        //    //Leaves = await _context.Leaves.FirstOrDefaultAsync(p => p.personID == Person.personID);
        //    Leaves = await _context.Leaves.Where(p => p.employmentID == searchID).ToListAsync();
        //    if(Leaves == null)
        //    {
        //        return NotFound();
        //    }
            
        //    return Leaves;
        //}
        //public async void CalculateLeave(int id) 
        //{
        //    DateTime hireDate = await _context.Employments.Where(p => p.employmentID == id && p.employmentTypeID==100).OrderByDescending(p => p.employmentDate).Select(p => p.employmentDate).FirstOrDefaultAsync();
        //    Double initialLeave = 20;            
        //    Double usedLeave= await _context.Leaves.Where(p=>p.employmentID == id && p.leaveTypeID==100).SumAsync(p => p.leaveDays);
        //    Double grossLeave=0;
        //    Double leavePerDay=grossLeave/365;
        //    Double leaveBalance;
        //    while (hireDate < DateTime.Today)
        //    {
               
        //        if (hireDate < DateTime.Today) {                    
        //            grossLeave += initialLeave;
        //            initialLeave++;
        //        }
        //        else
        //        {
        //            grossLeave += (DateTime.Today-hireDate).Days * (initialLeave/365);
        //        }
        //        hireDate = hireDate.AddYears(1);
        //    }
        //    leaveBalance = grossLeave - usedLeave;
        //}
        //public async void CalculatePerTerm(int id, DateTime start, DateTime end)
        //{
        //    DateTime hireDate = await _context.Employments.Where(p => p.employmentID == id && p.employmentTypeID == 100).OrderByDescending(p => p.employmentDate).Select(p => p.employmentDate).FirstOrDefaultAsync();
        //    Double initialLeave = 20;
        //    while (hireDate < start){
        //        initialLeave++;
        //        hireDate.AddYears (1);
        //    }
        //    Double usedLeave = await _context.Leaves.Where(p => p.employmentID == id 
        //    && p.leaveTypeID == 100 
        //    && p.leaveStartDate >= start
        //    && p.leaveStartDate <= end).SumAsync(p => p.leaveDays);
        //}
    }
}