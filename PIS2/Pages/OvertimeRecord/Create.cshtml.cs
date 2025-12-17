using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.OvertimeRecord
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        public employmentModel Employment { get; set; } = new employmentModel();
        public int EmployeeID;
        public personModel Person { get; set; } = new personModel();
        [BindProperty(SupportsGet = true)]
        public string givenID { get; set; }
        public string message { get; set; }
        public List<overtimeRecordModel> OtRecords { get; set; } = new List<overtimeRecordModel>();
       
        public IActionResult OnGet(int id)
        {

            if (id == 0)
            {
                id = _context.Users.Include(u => u.personModel)
                    .ThenInclude(p => p.Employments).First(u => u.userName == User.Identity.Name)
                    .personModel.Employments.FirstOrDefault(e => e.employmentStatus == mainStatus.Active).employmentID;
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
                Employment = _context.Employments.FirstOrDefault(e => e.employmentID == id) ?? new employmentModel();
                if (Employment.employmentID == 0)
                {
                    return NotFound();
                }
            }
            if (!string.IsNullOrEmpty(givenID))
            {
                Employment = _context.Employments.FirstOrDefault(e => e.givenID == givenID) ?? new employmentModel();
                Console.WriteLine("This is right here!!!!!!!!!!!");
                if (Employment != null)
                {

                    id = Employment.employmentID;
                    EmployeeID = id;
                    Console.WriteLine("ID is set from givenID" + id);
                    // Redirect to the Details page with employmentID
                    return RedirectToPage("Create", new { id = Employment.employmentID });
                }
            }


            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            ViewData["overtimeID"] = new SelectList(_context.Overtimes, "overtimeID", "overtimeName");
            EmployeeID = Employment.employmentID;
            TempData["MyNumber"] = EmployeeID;
            Person = _context.Persons.FirstOrDefault(e => e.personID == Employment.personID) ?? new personModel();
            OtRecords = _context.OvertimeRecords.Where(e => e.overtimeRecordStatus == overtimeStatus.Hold && e.employmentID == EmployeeID)
                       .Include(otr => otr.overtimeModel).ToList();
            return Page();
        }

        [BindProperty]
        public overtimeRecordModel overtimeRecordModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            
            int empID = Convert.ToInt32(TempData["MyNumber"]);
            TempData.Keep();
            if (overtimeRecordModel.overtimeRecordReason == "")
            {
                ModelState.AddModelError("", "Overtime end time must be later than start time!");
                LoadPageData(empID);
                return Page();
            }
            var ol = HasOverlappingOvertime(empID, overtimeRecordModel.overtimeRecordDate, overtimeRecordModel.overtimeRecordStartTime, overtimeRecordModel.overtimeRecordEndTime);
            if(ol != null)
            {
                TempData["ErrorMessage"] = $"Overlapping overtime record existed: Batch number={ol.overtimeRecordID}" +
                    $" Start={ol.overtimeRecordStartTime}" +
                    $" End={ol.overtimeRecordEndTime}";
                LoadPageData(empID);
                return Page();
                
            }
            int depID;
            
            var job = _context.JobPlacements
                
                .FirstOrDefault(j => j.employmentID == empID && j.jobPlacementStatus == mainStatus.Active) ?? new jobPlacementModel();

            var shift = _context.ShiftAssignments.OrderByDescending(sa => sa.modifiedDate).FirstOrDefault(sa => sa.employmentID == empID)?.shiftModel ?? _context.Shifts.FirstOrDefault();
            Employment = _context.Employments.FirstOrDefault(e => e.employmentID == empID) ?? new employmentModel();
            depID = _context.JobPlacements.FirstOrDefault(jp => jp.employmentID == Employment.employmentID)?.departmentID ?? 0;

            var nightOt = _context.Overtimes.FirstOrDefault(o => o.overtimeName.ToLower() == "night" && o.overtimeStatus == mainStatus.Active);
            var normalOt = _context.Overtimes.FirstOrDefault(o => o.overtimeName.ToLower() == "normal" && o.overtimeStatus == mainStatus.Active);
            var sundayOt = _context.Overtimes.FirstOrDefault(o => o.overtimeName.ToLower() == "sunday" && o.overtimeStatus == mainStatus.Active);
            var holidayOt = _context.Overtimes.FirstOrDefault(o => o.overtimeName.ToLower() == "holiday" && o.overtimeStatus == mainStatus.Active);

            TimeSpan otStart = overtimeRecordModel.overtimeRecordStartTime;
            TimeSpan otEnd = overtimeRecordModel.overtimeRecordEndTime;
            TimeSpan shiftStart = shift.shiftStart;
            TimeSpan shiftEnd = shift.shiftEnd;
            TimeSpan shiftEnd2 = shift.shiftEnd;

            if (otStart > otEnd)
            {
                ModelState.AddModelError("","Overtime end time must be later than start time!");
                LoadPageData(empID);
                return Page();
            }

            if (overtimeRecordModel.overtimeRecordDate.DayOfWeek == DayOfWeek.Saturday && shift.shiftName?.ToLower() == "normal")
            {
                shiftEnd = new TimeSpan(12, 0, 0);
            }

            
            var otEndTemp = otEnd;
            TrimOvertime(ref otStart, ref otEnd, shiftStart, shiftEnd);

            var savedIds = new List<int>();
            savedIds.AddRange(await AddOtRecords(overtimeRecordModel.overtimeRecordDate, otStart, otEnd, empID));
            if (otStart < shiftStart && otEndTemp > shiftEnd)
            {
                var otStartTemp = otEnd;
                savedIds.AddRange(await AddOtRecords(overtimeRecordModel.overtimeRecordDate, shiftEnd, otEndTemp, empID));
            }

            if (savedIds.Count > 0)
            {
                return RedirectToPage("./PersonOTR", new { otrID = savedIds, empID = 0 });
            }
            else
            {
                LoadPageData(empID);
                return Page();
            }
        }

        public void TrimOvertime(ref TimeSpan otStart, ref TimeSpan otEnd, TimeSpan shiftStart, TimeSpan shiftEnd)
        {
            // Fully within shift — discard overtime
            if (otStart >= shiftStart && otEnd <= shiftEnd)
            {
                otStart = otEnd = TimeSpan.Zero;
                message = "Overtime can not be with in shift period.";
            }
            // Fully before shift — no change
            else if (otEnd <= shiftStart)
            {
                // keep as is
            }
            // Fully after shift — no change
            else if (otStart >= shiftEnd)
            {
                // keep as is
            }
            // Starts before shift, ends during shift — trim end
            else if (otStart < shiftStart && otEnd > shiftStart && otEnd <= shiftEnd)
            {
                otEnd = shiftStart;
                message = "Overtime ends with in shift period, and is adjusted to the start of shift.";
            }
            // Starts during shift, ends after shift — trim start
            else if (otStart >= shiftStart && otStart < shiftEnd && otEnd > shiftEnd)
            {
                otStart = shiftEnd;
                message = "Overtime starts with in shift period, and is set to the end of shift";
            }
            // Overlaps both sides — keep only part after shift (optionally split)
            else if (otStart < shiftStart && otEnd > shiftEnd)
            {
                otEnd = shiftStart;
                message = "Overtime overlaps shift period and is trimmed to the start of shift.";
                
            }
        }
        public bool IsHoliday(DateTime date)
        {
            List<holidayModel> holidays = _context.Holidays.Where(h => h.holidayStatus == mainStatus.Active).ToList();
            return holidays.Any(h =>
                date.Date >= h.holidayStart.Date &&
                date.Date <= (h.holidayEnd == default ? h.holidayStart.Date : h.holidayEnd.Date)
            );
        }

        private void LoadPageData(int employmentId)
        {
            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            ViewData["overtimeID"] = new SelectList(_context.Overtimes, "overtimeID", "overtimeName");

            EmployeeID = employmentId;
            TempData["MyNumber"] = EmployeeID;

            Employment = _context.Employments.FirstOrDefault(e => e.employmentID == employmentId) ?? new employmentModel();
            Person = _context.Persons.FirstOrDefault(e => e.personID == Employment.personID) ?? new personModel();

            OtRecords = _context.OvertimeRecords
                .Where(e => e.overtimeRecordStatus == overtimeStatus.Hold && e.employmentID == EmployeeID)
                .Include(otr => otr.overtimeModel)
                .ToList();
        }


        public async Task<List<int>> AddOtRecords(DateTime date, TimeSpan otStart, TimeSpan otEnd, int empID)
        {
            var nightOt =await _context.Overtimes.FirstOrDefaultAsync(o => o.overtimeName.ToLower() == "night" && o.overtimeStatus == mainStatus.Active);
            var normalOt = await _context.Overtimes.FirstOrDefaultAsync(o => o.overtimeName.ToLower() == "normal" && o.overtimeStatus == mainStatus.Active);
            var sundayOt = await _context.Overtimes.FirstOrDefaultAsync(o => o.overtimeName.ToLower() == "sunday" && o.overtimeStatus == mainStatus.Active);
            var holidayOt = await _context.Overtimes.FirstOrDefaultAsync(o => o.overtimeName.ToLower() == "holiday" && o.overtimeStatus == mainStatus.Active);

            var job =await _context.JobPlacements
                .FirstOrDefaultAsync(j => j.employmentID == empID && j.jobPlacementStatus == mainStatus.Active) ?? new jobPlacementModel();
            
            var depID = job.departmentID;

            var records = new List<overtimeRecordModel>();
            //Sunday OT
            if (overtimeRecordModel.overtimeRecordDate.DayOfWeek == DayOfWeek.Sunday)
            {
                records.Add(new overtimeRecordModel
                {
                    employmentID = empID,
                    modifiedBy = User.Identity?.Name!,
                    overtimeRecordStatus = overtimeStatus.Hold,
                    overtimeRecordEmploymentRate = job?.getJobRate() ?? 0,
                    overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                    overtimeRecordStartTime = otStart,
                    overtimeRecordEndTime = otEnd,
                    overtimeID = sundayOt.overtimeID,
                    overtimeRate = sundayOt.overtimeRate,
                    overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                    departmentID = depID
                });
            }
            //Holiday OT
            else if (IsHoliday(overtimeRecordModel.overtimeRecordDate))
            {
                records.Add(new overtimeRecordModel
                {
                    employmentID = empID,
                    modifiedBy = User.Identity?.Name!,
                    overtimeRecordStatus = overtimeStatus.Hold,
                    overtimeRecordEmploymentRate = job?.getJobRate() ?? 0,
                    overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                    overtimeRecordStartTime = otStart,
                    overtimeRecordEndTime = otEnd,
                    overtimeID = holidayOt.overtimeID,
                    overtimeRate = holidayOt.overtimeRate,
                    overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                    departmentID = depID
                });
            }
            else
            {
                var nightStart = new TimeSpan(22, 0, 0);
                var nightEnd = new TimeSpan(6, 0, 0);
                //OT is Night
                if ((otStart >= nightStart || otStart < nightEnd) && (otEnd <= nightEnd || otEnd >= nightStart))
                {
                    records.Add(new overtimeRecordModel
                    {
                        employmentID = empID,
                        modifiedBy = User.Identity?.Name!,
                        overtimeRecordStatus = overtimeStatus.Hold,
                        overtimeRecordEmploymentRate = job.getJobRate(),
                        overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                        overtimeRecordStartTime = otStart,
                        overtimeRecordEndTime = otEnd,
                        overtimeID = nightOt.overtimeID,
                        overtimeRate = nightOt.overtimeRate,
                        overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                        departmentID = depID
                    });
                }
                //OT From normal To Night
                else if (otStart < nightStart && otEnd < new TimeSpan(23, 59, 59) && otEnd > nightStart)
                {
                    records.Add(new overtimeRecordModel
                    {
                        employmentID = empID,
                        modifiedBy = User.Identity?.Name!,
                        overtimeRecordStatus = overtimeStatus.Hold,
                        overtimeRecordEmploymentRate = job.getJobRate(),
                        overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                        overtimeRecordStartTime = otStart,
                        overtimeRecordEndTime = nightStart,
                        overtimeID = normalOt.overtimeID,
                        overtimeRate = normalOt.overtimeRate,
                        overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                        departmentID = depID
                    });

                    records.Add(new overtimeRecordModel
                    {
                        employmentID = empID,
                        modifiedBy = User.Identity?.Name!,
                        overtimeRecordStatus = overtimeStatus.Hold,
                        overtimeRecordEmploymentRate = job.getJobRate(),
                        overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                        overtimeRecordStartTime = nightStart,
                        overtimeRecordEndTime = otEnd,
                        overtimeID = nightOt.overtimeID,
                        overtimeRate = nightOt.overtimeRate,
                        overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                        departmentID = depID
                    });
                    Console.WriteLine(records.Count + "####### OT From Normal To Night !!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
                //OT from night, after mid-night to day
                else if (otStart < nightEnd && otEnd >= nightEnd)
                {
                    records.Add(new overtimeRecordModel
                    {
                        employmentID = empID,
                        modifiedBy = User.Identity?.Name!,
                        overtimeRecordStatus = overtimeStatus.Hold,
                        overtimeRecordEmploymentRate = job.getJobRate(),
                        overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                        overtimeRecordStartTime = otStart,
                        overtimeRecordEndTime = nightEnd,
                        overtimeID = nightOt.overtimeID,
                        overtimeRate = nightOt.overtimeRate,
                        overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                        departmentID = depID
                    });

                    records.Add(new overtimeRecordModel
                    {
                        employmentID = empID,
                        modifiedBy = User.Identity?.Name!,
                        overtimeRecordStatus = overtimeStatus.Hold,
                        overtimeRecordEmploymentRate = job.getJobRate(),
                        overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                        overtimeRecordStartTime = nightEnd,
                        overtimeRecordEndTime = otEnd,
                        overtimeID = normalOt.overtimeID,
                        overtimeRate = normalOt.overtimeRate,
                        overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                        departmentID = depID
                    });
                }
                else
                {
                    records.Add(new overtimeRecordModel
                    {
                        employmentID = empID,
                        modifiedBy = User.Identity?.Name!,
                        overtimeRecordStatus = overtimeStatus.Hold,
                        overtimeRecordEmploymentRate = job.getJobRate(),
                        overtimeRecordDate = overtimeRecordModel.overtimeRecordDate,
                        overtimeRecordStartTime = otStart,
                        overtimeRecordEndTime = otEnd,
                        overtimeID = normalOt.overtimeID,
                        overtimeRate = normalOt.overtimeRate,
                        overtimeRecordReason = overtimeRecordModel.overtimeRecordReason,
                        departmentID = depID
                    });
                }
            }

            var savedIds = new List<int>();
            if (records.Count > 0)
            {

                _context.OvertimeRecords.AddRange(records);
                try
                {
                    await _context.SaveChangesAsync();
                    savedIds.AddRange(records.Select(r => r.overtimeRecordID));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred while saving.");
                    
                }
            }
            return savedIds;
        }

        public overtimeRecordModel? HasOverlappingOvertime(
            int employmentID,
            DateTime date,
            TimeSpan newStart,
            TimeSpan newEnd,
            int? currentRecordID = null)
        {
            return _context.OvertimeRecords
                .Where(o => o.employmentID == employmentID &&
                            o.overtimeRecordDate.Date == date.Date &&
                            (currentRecordID == null || o.overtimeRecordID != currentRecordID))
                .FirstOrDefault(o =>
                    o.overtimeRecordStartTime < newEnd &&
                    o.overtimeRecordEndTime > newStart
                );
        }


    }
}
