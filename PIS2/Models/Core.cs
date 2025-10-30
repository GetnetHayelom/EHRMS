using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Plugins;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PIS2.Models;
using System;
using PIS2.Views;

namespace PIS2.Models
{
    public class Core
    {
        private readonly PISContext _context;

        public Core(PISContext context)
        {
            _context = context;
        }

        //working days in a year based on company policy
        const int workingDayPerYear = 312;
        const decimal weeksInAMonth = 4.33m;
        const int daysPerMonth = 26;
        const int workHoursPerDay = 8;

        
        //IS Self
        public int getUserEmp(string userName)
        {
            
            var personID = _context.Users.FirstOrDefault(u => u.userName.ToLower() == userName.ToLower())?.personID;
            var emp =_context.Employments.Where(e => e.personID == personID).FirstOrDefault()?.employmentID ?? 0;
            
            return emp;
        }
        //calculate leave balance in a given time interval
        public leaveDetail leaveSummary(int empID)
        {
            DateTime startDate = GetLeaveStart(empID);
            DateTime endDate = GetLeaveEnd(empID);
            List<leaveModel> leaves = new List<leaveModel>();
            leaves = _context.Leaves.Where(l => l.employmentID == empID).Include(l => l.leaveTypeModel).ToList();
            decimal usedLeave = leaves.Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).Sum(l => l.leaveDays);
            decimal accruedLeave = leaves.Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).Sum(l => l.leaveDays);
            //total number of days between given date
            int days = (endDate - startDate).Days;
            //initialize elapsed year
            int years = endDate.Year - startDate.Year;
            //number of days that are extra after allocating the total day per year
            int spareDays = 0;
            //the last amount incremented, initialised to the base rate
            //get the id of 'New Year Balance' Leave Type and use it for selecting parameter
            decimal lastAnnualLeaveIncrement = leaves.OrderBy(l => l.leaveRequestDate)
                .LastOrDefault(l => l.leaveTypeModel.leaveTypeName == "New Year Balance" && l.leaveStartDate <= DateTime.Now && l.leaveEndDate >= DateTime.Now)?.leaveDays ?? 0;
            //daily accrual rate by deviding last annual increment rate to the number of working days
            decimal dailyAccrualRate = DateTime.IsLeapYear(endDate.Year) ? lastAnnualLeaveIncrement / 366 : lastAnnualLeaveIncrement / 365;
            //total amount of leave until the given end time
            decimal totalLeave = 0;


            //amount of leave that can be utilised
            decimal allowedLeave = 0;
            spareDays = Enumerable.Range(0, ((endDate - (startDate).AddYears(years - 1))).Days + 1)
        .Select(offset => startDate.AddDays(offset))
        .Count(date => date.DayOfWeek != DayOfWeek.Sunday);

            totalLeave = accruedLeave - usedLeave;
            allowedLeave = totalLeave - (lastAnnualLeaveIncrement - (spareDays * dailyAccrualRate));
            var job = _context.JobPlacements?.Include(j => j.departmentModel).SingleOrDefault(j => j.employmentID == empID && j.jobPlacementStatus == mainStatus.Active)?? new jobPlacementModel();
            var dep = job.departmentModel;
            decimal leaveCost = (job.jobPlacementSalary / 26)*(allowedLeave);
            leaveDetail leaveSummary = new leaveDetail(totalLeave, allowedLeave, lastAnnualLeaveIncrement, startDate, endDate,leaveCost, dep);
            return leaveSummary;
        }
        public leaveDetail getAllLeaveSummary(string selectBy, int ID)
        {
            List<leaveDetail> leaveDetails = new List<leaveDetail>();
            var leaveDetail= new leaveDetail();
            var leaveSum = new leaveDetail();
            var emp = _context.Employments.Include(e => e.JobPlacements.Where(j => j.jobPlacementStatus == mainStatus.Active)).ThenInclude(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Where(e =>e.employmentStatus == mainStatus.Active).ToList();

            switch (selectBy) {

                case "Comp":
                    var comEmp = emp.Where(e => e.JobPlacements != null && e.employmentStatus == mainStatus.Active
                          && e.JobPlacements.Any()
                          && e.JobPlacements.First().departmentModel?.companyModel?.companyID == ID).Select(e => e.employmentID).ToList();
                    foreach (var e in comEmp)
                    {
                        leaveDetail = leaveSummary(e);
                        leaveDetails.Add(leaveDetail);
                    }
                    break;
                case "Dep":
                    var depEmp = emp.Where(e => e.JobPlacements != null && e.employmentStatus == mainStatus.Active
                        && e.JobPlacements.Any()
                        && e.JobPlacements.First().departmentModel?.departmentID == ID).Select(e => e.employmentID).ToList();
                    foreach (var e in depEmp)
                    {
                        leaveDetail = leaveSummary(e);
                        leaveDetails.Add(leaveDetail);
                    }
                    break;
                    default:
                    foreach (var e in emp)
                    {
                        leaveDetail = leaveSummary(e.employmentID);
                        leaveDetails.Add(leaveDetail);
                    }
                    break;
            }

            leaveSum.AllowedLeave = leaveDetails.Sum(ld=>ld.AllowedLeave);
            leaveSum.leaveCost = leaveDetails.Sum(ld => ld.leaveCost);
            leaveSum.LastAccrualIncrement = leaveDetails.Sum(ld => ld.LastAccrualIncrement);

            return leaveSum;
        }

        public List<overtimeRecordModel> getAllOvertime(string selectBy, int ID)
        {
            List<overtimeRecordModel> overtimes = new List<overtimeRecordModel>();
            
           

            switch (selectBy)
            {

                case "Comp":
                    overtimes = _context.OvertimeRecords
                        .Where(ot => ot.employmentModel.JobPlacements
                            .Any(jp => jp.departmentModel.companyID == ID && jp.jobPlacementStatus == mainStatus.Active)).ToList();

                    break;
                case "Dep":
                    overtimes = _context.OvertimeRecords
                        .Where(ot => ot.employmentModel.JobPlacements
                            .Any(jp => jp.departmentID == ID && jp.jobPlacementStatus == mainStatus.Active)).ToList();
                    break;
                default:
                    
                    break;
            }

            

            return overtimes;
        }
        public leaveDetail GetLeaveSummary(int empID)
        {
            mainStatus empStatus = _context.Employments.Where(e => e.employmentID == empID).FirstOrDefault().employmentStatus;
            decimal hRate = 0;
            if (_context.JobPlacements.Any( jp=> jp.employmentID == empID))
            {
                hRate = (decimal)_context.JobPlacements.OrderByDescending(js => js.jobPlacementDate).First(js => js.employmentID == empID).jobPlacementSalary / 26;
            }
            DateTime startDate = GetLeaveStart(empID);
            DateTime endDate = GetLeaveEnd(empID);
            List<leaveModel> leaves = new List<leaveModel>();
            leaves = _context.Leaves.Where(l=>l.employmentID == empID).Include(l => l.leaveTypeModel).ToList();
            decimal usedLeave = leaves.Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).Sum(l => l.leaveDays);
            decimal accruedLeave = leaves.Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).Sum(l => l.leaveDays);
            //total number of days between given date
            int days = (endDate - startDate).Days;
            //initialize elapsed year
            int years = endDate.Year - startDate.Year;
            //number of days that are extra after allocating the total day per year
            int spareDays = 0;
            //the last amount incremented, initialised to the base rate
            //get the id of 'New Year Balance' Leave Type and use it for selecting parameter
            decimal lastAnnualLeaveIncrement = leaves.OrderBy(l => l.leaveRequestDate)
                .LastOrDefault(l => l.leaveTypeID == 63)?.leaveDays ?? 0;
            //daily accrual rate by deviding last annual increment rate to the number of working days
            decimal dailyAccrualRate = DateTime.IsLeapYear(endDate.Year)? lastAnnualLeaveIncrement / 366 : lastAnnualLeaveIncrement / 365;
            //total amount of leave until the given end time
            decimal totalLeave = 0;

            DateTime lastDate = endDate > DateTime.Now ? DateTime.Now : endDate;
            //amount of leave that can be utilised
            decimal allowedLeave = 0;
            
            if (endDate < startDate.AddYears(years))
                years--;

            DateTime partialYearStart = startDate.AddYears(years);

            spareDays = Enumerable.Range(0, (endDate - partialYearStart).Days + 1)
            .Select(offset => partialYearStart.AddDays(offset))
            .Count();
            var allocatedForGrant = lastAnnualLeaveIncrement - (spareDays * dailyAccrualRate);

            totalLeave = accruedLeave - usedLeave;
            if (empStatus == mainStatus.Inactive)
            {
                totalLeave -= allocatedForGrant;
            }
            allowedLeave = totalLeave - (lastAnnualLeaveIncrement - (spareDays * dailyAccrualRate));
            allowedLeave = allowedLeave < 0 ? 0 : allowedLeave;

            
            leaveDetail leaveSummary = new leaveDetail(Math.Round(totalLeave,2), allowedLeave, lastAnnualLeaveIncrement, startDate, endDate, LeavesPerYear(empID));
            leaveSummary.leaveCost = allowedLeave * hRate;
            return leaveSummary;
        }
      
        public List<leavePerYear> LeavesPerYear(int empID)
        {
            //get employee hourly rate
            decimal hRate = 0;
            if (_context.JobPlacements.Any(jp => jp.employmentID == empID))
            {
                hRate = _context.JobPlacements.OrderByDescending(js => js.jobPlacementDate).First(js => js.employmentID == empID).jobPlacementSalary / 26;
            }
            //Returns all leaves of the employee
            List<leaveModel> Leaves = GetLeaves(empID) ?? new List<leaveModel>();
            List<leaveModel> accrued = GetAccruedLeaves(empID)?? new List<leaveModel>();
            List<leaveModel> used = GetUsedLeaves(empID) ?? new List<leaveModel>();
            DateTime startDate = GetLeaveStart(empID);
            DateTime endDate = GetLeaveEnd(empID);
            int years = endDate.Year - startDate.Year;
            
            if (startDate.AddYears(years) < endDate)
            {
                years++;
            }
            List<leavePerYear> leavesPerYear = new List<leavePerYear>();
            leavePerYear leavePerYear = new leavePerYear();

            decimal lastIncrement =accrued.IsNullOrEmpty()? 0 : accrued.OrderBy(l => l.leaveRequestDate)
                .LastOrDefault(l => l.leaveTypeID == 63 && l.leaveStartDate <= endDate && l.leaveEndDate >= endDate)?.leaveDays ?? 0;
            decimal totalUsedLeaves = used.Sum(l => l.leaveDays);
            decimal totalAccruedLeaves = accrued.Sum(l => l.leaveDays);
            decimal balance = totalAccruedLeaves - totalUsedLeaves;
            decimal usedLeaves = used.Sum(l => l.leaveDays);
            decimal accruedLeaves = 0;
            decimal startingLeavePerYear = 0;
            DateTime dateCounter = startDate;
            decimal rollOverLeave = 0;
            //decimal accrualCounter = baseLeave;

            for (int i = 0; i < years; i++)
            {
                
                usedLeaves = used.Where(l => l.leaveRequestDate >= dateCounter && l.leaveRequestDate < dateCounter.AddYears(1)).Sum(l =>l.leaveDays);
                accruedLeaves = accrued.Where(l => (l.leaveRequestDate >= dateCounter && l.leaveRequestDate < dateCounter.AddYears(1))).Sum(l => l.leaveDays);
               

                leavePerYear = new leavePerYear();
                leavePerYear.startingLeaveAmount = startingLeavePerYear;
                leavePerYear.startDate = dateCounter;
                leavePerYear.endDate = dateCounter.AddYears(1);
                leavePerYear.accruedLeaveAmount = accruedLeaves;
                leavePerYear.usedLeaveAmount = usedLeaves;
                leavePerYear.rollOverLeave = rollOverLeave;
                

                if(dateCounter > new DateTime(2013,1,1)) { leavesPerYear.Add(leavePerYear);}
                            

                dateCounter = dateCounter.AddYears(1);
                startingLeavePerYear += (accruedLeaves - usedLeaves);
                lastIncrement = accrued.OrderByDescending(l => l.leaveRequestDate).FirstOrDefault()?.leaveDays ?? 0;
                balance = totalAccruedLeaves - totalUsedLeaves;
            }
            var carryOverTotal = balance;
            for(int i=leavesPerYear.Count-1; i>=0; i--)
            {
                if(carryOverTotal > leavesPerYear[i].accruedLeaveAmount)
                {
                    carryOverTotal -= leavesPerYear[i].accruedLeaveAmount;
                    leavesPerYear[i].rollOverLeave = leavesPerYear[i].accruedLeaveAmount;
                    leavesPerYear[i].remainingLeaveCost = Math.Round((leavesPerYear[i].rollOverLeave * hRate), 2);
                }
                else
                {
                    leavesPerYear[i].rollOverLeave = carryOverTotal;
                    leavesPerYear[i].remainingLeaveCost = Math.Round((leavesPerYear[i].rollOverLeave * hRate), 2);
                    break;
                }
                 
            }
            
            return leavesPerYear;
        }
          public leaveDetail GetLeaveSummary2(int empID)
        {
            List<leaveModel> leaves = new List<leaveModel>();
            leaves = _context.Leaves.Where(l=>l.employmentID == empID).ToList();
            DateTime startDate = leaves.Min(l => l.leaveRequestDate);
            DateTime endDate = DateTime.Now;
            //total number of days between given date
            int days = (endDate - startDate).Days;
            //initialize elapsed year to 0
            int years;
            //number of days that are extra after allocating the total day per year
            int spareDays = 0;
            //the last amount incremented, initialised to the base rate
            decimal lastAnnualLeaveIncrement = leaves.Where(l => l.leaveTypeID == 2017).FirstOrDefault().leaveDays;
            //daily accrual rate by deviding last annual increment rate to the number of working days
            decimal dailyAccrualRate = 0;
            //total amount of leave until the given end time
            decimal totalLeave = 0;
            //amount of leave that can be utilised
            decimal allowedLeave = 0;
            decimal usedLeave =leaves.Where(l => l.leaveTypeModel != null && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).Sum(l => l.leaveDays);
            decimal accruedleave = leaves.Where(l => l.leaveTypeModel != null && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).Sum(l => l.leaveDays);
            decimal leaveBalance = accruedleave - usedLeave;
                
            //set the given time interval into years and spare days 
            if (days > 365)
            {
                years = endDate.Year - startDate.Year;
                spareDays = (endDate - startDate).Days % 365;
                dailyAccrualRate = lastAnnualLeaveIncrement / workingDayPerYear;
                totalLeave = leaves.Where(l => l.leaveTypeID == 2017).Sum(l => l.leaveDays);
                //totalLeave = ((lastAnnualLeaveIncrement - baseLeave + 1) / 2) * (baseLeave + lastAnnualLeaveIncrement);
                allowedLeave =leaveBalance + (lastAnnualLeaveIncrement - (spareDays*dailyAccrualRate));
            }
            else
            {
                years = 1;
                spareDays = days;
                totalLeave = lastAnnualLeaveIncrement;
                dailyAccrualRate = lastAnnualLeaveIncrement / workingDayPerYear;
                allowedLeave = spareDays * dailyAccrualRate;
            }
            List<leaveModel> accrued = leaves.Where(l => l.leaveTypeModel != null && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).ToList();
            List<leaveModel> used = leaves.Where(l => l.leaveTypeModel != null && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).ToList();


            List<leavePerYear> leavesPerYear = new List<leavePerYear>();
            leavePerYear leavePerYear = new leavePerYear();

            decimal usedLeaves = used.Sum(l => l.leaveDays);
            decimal accruedLeaves = accrued.Sum(l => l.leaveDays);
            
            DateTime dateCounter = startDate;
            //decimal accrualCounter = baseLeave;
            for (int i = 0; i < years; i++)
            {
                usedLeaves = used.Where(l => l.leaveStartDate >= dateCounter && l.leaveEndDate < dateCounter.AddYears(1)).Sum(l => l.leaveDays);
                accruedLeaves = accrued.Where(l => l.leaveStartDate >= dateCounter && l.leaveEndDate < dateCounter.AddYears(1)).Sum(l => l.leaveDays);
                leavePerYear = new leavePerYear();
                leavePerYear.startDate = dateCounter;
                leavePerYear.endDate = dateCounter.AddYears(1);
                leavePerYear.accruedLeaveAmount = accruedLeaves;
                leavePerYear.usedLeaveAmount = usedLeaves;
                leavesPerYear.Add(leavePerYear);
                dateCounter = dateCounter.AddYears(1);
            }
            leaveDetail leaveSummary = new leaveDetail(totalLeave, allowedLeave, lastAnnualLeaveIncrement, startDate, endDate, leavesPerYear);
            return leaveSummary;
        }
        //get employment history to get where leave count starts and ends
        private List<employmentHistoryModel> GetEmpHist(int empID)
        {
            List<employmentHistoryModel> histories = new List<employmentHistoryModel>();
            histories = _context.EmploymentHistories.Include(e=> e.employmentTypeModel).Where(e => e.employmentID == empID).ToList();
            return histories;
        }
        private DateTime GetLeaveStart(int empID)
        {
            employmentModel employment = new employmentModel();
            employment = _context.Employments.Where(e => e.employmentID == empID).First();
            DateTime leaveCountStartDate= employment.employmentDate;
            
            return leaveCountStartDate;
        }
        private DateTime GetLeaveEnd(int empID)
        {
            employmentModel employment = new employmentModel();
            employment = _context.Employments.Where(e => e.employmentID == empID).First();
            DateTime leaveCountEndDate;// = DateTime.Now;
            if(employment.employmentStatus == mainStatus.Inactive)
            {
                leaveCountEndDate = _context.Terminations.First(t => t.employmentID == empID)?.terminationDate ?? DateTime.Now;
                //leaveCountEndDate = GetEmpHist(empID).OrderByDescending(eh=> eh.modifiedDate).First().modifiedDate;
            }
            else
            {
                leaveCountEndDate=DateTime.Now;
            }
            if (leaveCountEndDate == GetLeaveStart(empID))
            {
                leaveCountEndDate = DateTime.Now;
            }
            return leaveCountEndDate;
        }
        private List<leaveModel> GetLeaves(int empID)
        {
            List<leaveModel> leaves = _context.Leaves.Include(l=>l.leaveTypeModel).Where(l=> l.employmentID == empID).ToList();
            return leaves;
        }
        private List<leaveModel> GetUsedLeaves(int empID) {
            List<leaveModel> usedLeaves = GetLeaves(empID).Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative && (l.leaveStatus == leaveStatus.Posted || l.leaveStatus == leaveStatus.Comleted)).ToList();
            return usedLeaves;
        }
        private List<leaveModel> GetAccruedLeaves(int empID)
        {
            List<leaveModel> accruedLeaves = GetLeaves(empID).Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).ToList(); ;
            return accruedLeaves;
        }
        public Tuple<int, int> GetYearsAndMonths(DateTime startDate, DateTime endDate)
        {
            int years = endDate.Year - startDate.Year;
            int months = endDate.Month - startDate.Month;

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return Tuple.Create(years, months);
        }
        public decimal WorkingDays(DateTime startDate, DateTime endDate)
        {

            // Example: List of holidays – ideally from a database or config
            var holidays = _context.Holidays.Where(h => h.holidayStatus == mainStatus.Active).ToList();


            decimal workingDays = 0;

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (IsHoliday(date))
                    continue;

                if (date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (date.DayOfWeek == DayOfWeek.Saturday)
                    workingDays += 0.5m;
                else
                    workingDays += 1;
            }

            return workingDays;
        }
        public bool IsHoliday(DateTime date)
        {
            List<holidayModel> holidays = _context.Holidays.Where(h => h.holidayStatus == mainStatus.Active).ToList();
            return holidays.Any(h =>
                date.Date >= h.holidayStart.Date &&
                date.Date <= (h.holidayEnd == default ? h.holidayStart.Date : h.holidayEnd.Date)
            );
        }
        public employmentModel empByID(string givenID)
        {
            employmentModel model = new employmentModel();
            if (!string.IsNullOrEmpty(givenID))
            {
                model = _context.Employments.FirstOrDefault(e => e.givenID == givenID);
            }
            return model;
        }



        /// <summary>
        /// Get number of active employees in a company.
        /// </summary>
        public int GetCompanyEmployees(int companyId)
        {
            return _context.JobPlacements
                .Where(d => d.departmentModel.companyID == companyId &&
                            d.jobPlacementStatus == mainStatus.Active)
                .Count();
        }
        /// <summary>
        /// Get number of active employees in a department.
        /// </summary>
        public int GetDepartmentEmployees(int departmentID)
        {
            return _context.JobPlacements
                .Where(d => d.departmentID == departmentID &&
                            d.jobPlacementStatus == mainStatus.Active)
                .Count();
        }
        /// <summary>
        /// Calculate the number of working days (excluding Sundays & active holidays)
        /// multiplied by the number of active employees in the company.
        /// </summary>
        public decimal GetWorkingDays(DateTime StartDate, DateTime EndDate)
        {
            var holidays = _context.Holidays
                .Where(h => h.holidayStatus == mainStatus.Active &&
                            h.holidayStart <= EndDate &&
                            h.holidayEnd >= StartDate) // overlaps
                .ToList();

            int workingDays = 0;

            for (var day = StartDate; day <= EndDate; day = day.AddDays(1))
            {
                bool isSunday = day.DayOfWeek == DayOfWeek.Sunday;
                bool isHoliday = holidays.Any(h => day >= h.holidayStart && day <= h.holidayEnd);

                if (!isSunday && !isHoliday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }

        public bool IsSelf(string UserName, int? empID)
        {
            if(empID == null)
            {
                return false;
            }

            string useN = _context.Employments
                .Where(e => e.employmentID == empID)
                .Select(e => e.personModel != null && e.personModel.userModel != null
                    ? e.personModel.userModel.userName
                    : null)
                .FirstOrDefault();

            
            if (UserName == useN)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Retrieves a user's access entries (roles and companies) by username.
        /// </summary>
        /// <param name="username">Windows username (e.g., DOMAIN\user)</param>
        /// <returns>
        /// List of tuples: (UserGroup role, CompanyID company)
        /// </returns>
        public async Task<List<(UserGroups Role, int? CompanyID)>> GetUserAccessAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new List<(UserGroups, int?)>();

            
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.userName == username);

            if (user == null)
                return new List<(UserGroups, int?)>();

            var accessList = await _context.Accesses
                .AsNoTracking()
                .Where(a => a.userID == user.userID && a.accessStatus == mainStatus.Active)
                .Select(a => new { a.userGroups, a.companyID })
                .ToListAsync();

            return accessList
                .Select(a => (a.userGroups, a.companyID))
                .ToList();
        }
        public Core() { }

    }
    // Helper class for the data returned by the AJAX handler
   
}
