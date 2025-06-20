using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.Absentism
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


        [BindProperty]
        public List<personModel>? People { get; set; } = default!;
        [BindProperty]
        [ValidateNever]
        public List<employmentModel>? Employments { get; set; } = default!;
        [BindProperty]
        public employmentModel? Employment { get; set; } = default!;
        [BindProperty]
        public leaveModel leaveModel { get; set; } = default!;
        public List<leaveModel> Absentism { get; set; } = default!;
        public personModel Person { get; set; }
        public List<leaveTypeModel> AllowedLeaveTypes { get; set; }
        public int aCount {  get; set; }
        public double aSum { get; set; }
        [BindProperty]
        public string searchID { get; set; } = default!;
        [BindProperty]
        public string Department { get; set; } = default!;
        [BindProperty]
        public DateTime minDate { get; set; }
        public IActionResult OnGet()
        {

            //Console.WriteLine($"Search ID on Get: {searchID}");

            
            if (!string.IsNullOrEmpty(searchID))
            {
                var leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveGroup == leaveGroup.Absentism && lt.leaveTypeStatus == mainStatus.Active).ToList();

                AllowedLeaveTypes = leaveTypes;
                ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes, "leaveTypeID", "leaveTypeName");

                Person = _context.Persons.Where(p => p.Employments.Any(e => e.givenID == searchID))
                    .FirstOrDefault();

                if (Person == null)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                    return Page();
                }
                else
                {
                    TempData["PersonID"] = Person.personID;
                    Employment =_context.Employments
                        .Include(e => e.Leaves).ThenInclude(l => l.leaveTypeModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .FirstOrDefault(e => e.personID == Person.personID);

                    if (Employment == null)
                    {
                        TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                        return Page();

                    }
                    else
                    {
                        Absentism = Employment.Leaves.Where(l => l.leaveTypeModel.leaveGroup == leaveGroup.Absentism).OrderByDescending(l => l.leaveStartDate).ToList();
                        minDate = Absentism.Min(a => a.leaveStartDate);
                        aCount = Absentism.Count();
                        aSum = Math.Round(Absentism.Sum(a => a.leaveDays), 2);
                        leaveModel.employmentID = Employment.employmentID;
                    }

                }

            }
            else
            {
                People = _context.Persons.ToList();
                Employments = _context.Employments.ToList();
                Person = new personModel();
                Employment = new employmentModel();
                ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
                var leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveGroup == leaveGroup.Absentism && lt.leaveTypeStatus == mainStatus.Active).ToList();

                AllowedLeaveTypes = leaveTypes;
                ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes, "leaveTypeID", "leaveTypeName");
            }
            People = _context.Persons.ToList();
            Employments = _context.Employments.ToList();
            return Page();
        }

       
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostCreateAsync()
        {
            //ModelState.Clear();
            leaveModel.leaveStatus = leaveStatus.Hold;
            leaveModel.ratePerHour = _context.JobPlacements.FirstOrDefault(jp => jp.jobPlacementStatus == mainStatus.Active && jp.employmentID == leaveModel.employmentID)?.getJobRate() ?? 0;
            ModelState.Remove("givenID");
            ModelState.Remove("searchID");
            ModelState.Remove("Department");
            ModelState.Remove("employmentPosition");
            ModelState.Remove("modifiedBy");
            if (!ModelState.IsValid)
            {
              
                Console.WriteLine("Invalid model:");
                
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                }

                return Page();
            }
            if(leaveModel.leaveTypeID == 58)
            {
                leaveModel.leaveDays = leaveModel.leaveStartDate.DayOfWeek == DayOfWeek.Saturday?leaveModel.leaveDays/240: leaveModel.leaveDays/480;
            }
            
            _context.Leaves.Add(leaveModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("Details", new { id = leaveModel.leaveID });
        }
        
        
        public async Task<IActionResult> OnPostSearchID()
        {
            Console.WriteLine($"Search ID: {searchID}");

            if (!string.IsNullOrEmpty(searchID))
            {
                var leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveGroup == leaveGroup.Absentism && lt.leaveTypeStatus == mainStatus.Active).ToList();

                AllowedLeaveTypes = leaveTypes;
                ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes, "leaveTypeID", "leaveTypeName");

                Person = await _context.Persons.Where(p => p.Employments.Any(e => e.givenID == searchID))
                    .FirstOrDefaultAsync();

                if (Person == null)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                    return Page();
                }
                else
                {
                    TempData["PersonID"] = Person.personID;
                    Employment = await _context.Employments
                        .Include(e => e.Leaves).ThenInclude(l => l.leaveTypeModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .FirstOrDefaultAsync(e => e.personID == Person.personID);
                    
                    if (Employment == null)
                    {
                        TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                        return Page();

                    }
                    else
                    {
                        Absentism = Employment.Leaves.Where(l => l.leaveTypeModel.leaveGroup == leaveGroup.Absentism).OrderByDescending(l => l.leaveStartDate).ToList();
                        minDate = Absentism.Min(a => a.leaveStartDate);
                        aCount = Absentism.Count();
                        aSum = Math.Round(Absentism.Sum(a => a.leaveDays),2);
                        leaveModel.employmentID = Employment.employmentID;
                    }

                }

            }
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();

            return Page();
        }

        public JsonResult OnPostCalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            return new JsonResult(_core.WorkingDays(startDate, endDate));
        }
        public JsonResult OnPostSummary()
        {
            var startDate = Request.Form["start"];
            var endDate = Request.Form["end"];
            var leaveType = Request.Form["type"];
            var employmentID = _core.empByID(Request.Form["empId"]).employmentID;
            
            string table="";
            
            if (DateTime.TryParse(startDate, out DateTime start) && DateTime.TryParse(endDate, out DateTime end) && int.TryParse(leaveType, out int type))
            {
                if(type == 0)
                {
                    Absentism = _context.Leaves
                        .Include(l=>l.leaveTypeModel)
                        .Where(l => l.leaveStartDate >= start && l.leaveEndDate <= end && l.employmentID == employmentID && l.leaveTypeModel.leaveGroup == leaveGroup.Absentism).OrderByDescending(l => l.leaveStartDate).ToList();
                }
                else 
                {
                    Absentism = _context.Leaves
                        .Include(l => l.leaveTypeModel)
                        .Where(l => l.leaveStartDate >= start && l.leaveEndDate <= end && l.leaveTypeID == type && l.employmentID == employmentID).OrderByDescending(l =>l.leaveStartDate).ToList();
                }
                aCount = Absentism.Count();
                aSum = Math.Round(Absentism.Sum(l =>l.leaveDays),2);

                var url = "/Absentism/Details?id=";
                foreach (var l in Absentism)
                {
                    table += $"<tr onclick=\"location.href='{url}{l.leaveID}'\" style=\"cursor:pointer;\">" +
                        $"<td>{l.leaveStartDate.ToString("MMM dd, yyyy")}</td>" +
                        $"<td>{l.leaveEndDate.ToString("MMM dd, yyyy")}</td>" +
                        $"<td>{l.leaveTypeModel.leaveTypeName}</td>" +
                        $"<td>{Math.Round(l.leaveDays, 5)}</td>" +
                        $"<td>{l.leaveStatus}</td></tr>";
                }
            }
            
            return new JsonResult(new {CountIs = aCount, SumIs = aSum, tableHtml=table});
        }
    }
}
