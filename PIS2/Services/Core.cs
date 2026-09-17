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
using System.Threading.Tasks;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Foundation;
using PIS2.Models.HR;

namespace PIS2.Services
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
        const int severanceStartYear = 5;
        
        //IS Self
        public int getUserEmp(string userName)
        {
            
            var personID = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower())?.personID;
            var emp =_context.Employments.Where(e => e.personID == personID).FirstOrDefault()?.employmentID ?? 0;
            
            return emp;
        }
        //calculate leave balance in a given time interval
        public async Task<leaveDetail> leaveSummary(int empID)
        {
            DateTime startDate =await GetLeaveStart(empID);
            DateTime endDate =await GetLeaveEndAsync(empID);
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
            spareDays = Enumerable.Range(0, (endDate - startDate.AddYears(years - 1)).Days + 1)
        .Select(offset => startDate.AddDays(offset))
        .Count(date => date.DayOfWeek != DayOfWeek.Sunday);

            totalLeave = accruedLeave - usedLeave;
            allowedLeave = totalLeave - (lastAnnualLeaveIncrement - spareDays * dailyAccrualRate);
            var job =await _context.JobPlacements?.Include(j => j.departmentModel).ThenInclude(d => d.companyModel).SingleOrDefaultAsync(j => j.employmentID == empID && j.jobPlacementStatus == mainStatus.Active)?? new jobPlacementModel();
            var dep = job.departmentModel;
            decimal leaveCost = job.jobPlacementSalary / 26*allowedLeave;
            leaveDetail leaveSummary = new leaveDetail(totalLeave, allowedLeave, lastAnnualLeaveIncrement, startDate, endDate,leaveCost, dep);
            return leaveSummary;
        }
        public async Task<leaveDetail> getAllLeaveSummary(string selectBy, int ID)
        {
            List<leaveDetail> leaveDetails = new List<leaveDetail>();
            var leaveDetail= new leaveDetail();
            var leaveSum = new leaveDetail();
            var emp =await _context.Employments.Include(e => e.JobPlacements.Where(j => j.jobPlacementStatus == mainStatus.Active)).ThenInclude(j => j.departmentModel).ThenInclude(d => d.companyModel)
                .Where(e =>e.employmentStatus == mainStatus.Active).ToListAsync();

            switch (selectBy) {

                case "Comp":
                    var comEmp = emp.Where(e => e.JobPlacements != null && e.employmentStatus == mainStatus.Active
                          && e.JobPlacements.Any()
                          && e.JobPlacements.First().departmentModel?.companyModel?.companyID == ID).Select(e => e.employmentID).ToList();
                    foreach (var e in comEmp)
                    {
                        leaveDetail = await leaveSummary(e);
                        leaveDetails.Add(leaveDetail);
                    }
                    break;
                case "Dep":
                    var depEmp = emp.Where(e => e.JobPlacements != null && e.employmentStatus == mainStatus.Active
                        && e.JobPlacements.Any()
                        && e.JobPlacements.First().departmentModel?.departmentID == ID).Select(e => e.employmentID).ToList();
                    foreach (var e in depEmp)
                    {
                        leaveDetail =await leaveSummary(e);
                        leaveDetails.Add(leaveDetail);
                    }
                    break;
                    default:
                    foreach (var e in emp)
                    {
                        leaveDetail =await leaveSummary(e.employmentID);
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
        public async Task<leaveDetail> GetLeaveSummary(int empID)
        {
            
             
                var employment = await _context.Employments.Where(e => e.employmentID == empID).FirstOrDefaultAsync();
                decimal hRate = 0;
                if (_context.JobPlacements.Any( jp=> jp.employmentID == empID))
                {
                    hRate = _context.JobPlacements.OrderByDescending(js => js.jobPlacementDate).First(js => js.employmentID == empID).jobPlacementSalary / 26;
                }
                DateTime startDate =await GetLeaveStart(empID);
                DateTime endDate =await GetLeaveEndAsync(empID);
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
                var allocatedForGrant = lastAnnualLeaveIncrement - spareDays * dailyAccrualRate;

                totalLeave = accruedLeave - usedLeave;
                if (employment.employmentStatus == mainStatus.Inactive)
                {
                    totalLeave -= allocatedForGrant;
                }
                allowedLeave = totalLeave - (lastAnnualLeaveIncrement - spareDays * dailyAccrualRate);
                allowedLeave = allowedLeave < 0 ? 0 : allowedLeave;
                var lpy = await LeavesPerYear(empID);
                leaveDetail leaveSummary = new leaveDetail(Math.Round(totalLeave,2), allowedLeave, lastAnnualLeaveIncrement, startDate, endDate, lpy);
                leaveSummary.leaveCost = allowedLeave * hRate;
                return leaveSummary;
            
            return new leaveDetail();
            
            
        }
      
        public async Task<List<leavePerYear>> LeavesPerYear(int empID)
        {
            //get employee hourly rate
            decimal hRate = 0;

            var hasJob = await _context.JobPlacements.AnyAsync(jp => jp.employmentID == empID);
            if (hasJob)
            {
                var latestJob = await _context.JobPlacements
                    .Where(js => js.employmentID == empID)
                    .OrderByDescending(js => js.jobPlacementDate)
                    .FirstOrDefaultAsync();

                hRate = latestJob?.jobPlacementSalary / 26 ?? 0;
            }

            //Returns all leaves of the employee
            List<leaveModel> Leaves =await GetLeaves(empID) ?? new List<leaveModel>();
            List<leaveModel> accrued = await GetAccruedLeaves(empID)?? new List<leaveModel>();
            List<leaveModel> used = await GetUsedLeaves(empID) ?? new List<leaveModel>();
            DateTime startDate = await GetLeaveStart(empID);
            DateTime endDate =await GetLeaveEndAsync(empID);
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
                accruedLeaves = accrued.Where(l => l.leaveRequestDate >= dateCounter && l.leaveRequestDate < dateCounter.AddYears(1)).Sum(l => l.leaveDays);
               

                leavePerYear = new leavePerYear();
                leavePerYear.startingLeaveAmount = startingLeavePerYear;
                leavePerYear.startDate = dateCounter;
                leavePerYear.endDate = dateCounter.AddYears(1);
                leavePerYear.accruedLeaveAmount = accruedLeaves;
                leavePerYear.usedLeaveAmount = usedLeaves;
                leavePerYear.rollOverLeave = rollOverLeave;
                

                if(dateCounter > new DateTime(2013,1,1)) { leavesPerYear.Add(leavePerYear);}
                            

                dateCounter = dateCounter.AddYears(1);
                startingLeavePerYear += accruedLeaves - usedLeaves;
                var accruedSorted = accrued.OrderBy(l => l.leaveRequestDate).ToList();
                lastIncrement = accruedSorted.LastOrDefault()?.leaveDays ?? 0;
                balance = totalAccruedLeaves - totalUsedLeaves;
            }
            var carryOverTotal = balance;
            for(int i=leavesPerYear.Count-1; i>=0; i--)
            {
                if(carryOverTotal > leavesPerYear[i].accruedLeaveAmount)
                {
                    carryOverTotal -= leavesPerYear[i].accruedLeaveAmount;
                    leavesPerYear[i].rollOverLeave = leavesPerYear[i].accruedLeaveAmount;
                    leavesPerYear[i].remainingLeaveCost = Math.Round(leavesPerYear[i].rollOverLeave * hRate, 2);
                }
                else
                {
                    leavesPerYear[i].rollOverLeave = carryOverTotal;
                    leavesPerYear[i].remainingLeaveCost = Math.Round(leavesPerYear[i].rollOverLeave * hRate, 2);
                    break;
                }
                 
            }
            
            return leavesPerYear;
        }
        public async Task<leaveDetail> GetLeaveSummary2(int empID)
        {
            List<leaveModel> leaves = new List<leaveModel>();
            leaves =await _context.Leaves.Where(l=>l.employmentID == empID).ToListAsync();
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
                allowedLeave =leaveBalance + (lastAnnualLeaveIncrement - spareDays*dailyAccrualRate);
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
        private async Task<DateTime> GetLeaveStart(int empID)
        {
            employmentModel employment = new employmentModel();
            employment = await _context.Employments.Where(e => e.employmentID == empID).FirstOrDefaultAsync();
    
            DateTime leaveCountStartDate= employment.employmentDate;
            
            return leaveCountStartDate;
        }
        private async Task<DateTime> GetLeaveEndAsync(int empID)
        {
            // Fetch employment asynchronously
            var employment = await _context.Employments
                .FirstOrDefaultAsync(e => e.employmentID == empID);

            if (employment == null)
                return DateTime.Now; // fallback if employment not found

            DateTime leaveCountEndDate;

            if (employment.employmentStatus == mainStatus.Inactive)
            {
                // Fetch termination asynchronously
                var termination = await _context.Terminations
                    .FirstOrDefaultAsync(t => t.employmentID == empID);

                leaveCountEndDate = termination?.terminationDate ?? DateTime.Now;
            }
            else
            {
                leaveCountEndDate = DateTime.Now;
            }

            // Assuming GetLeaveStart is now async
            var leaveStart =await GetLeaveStart(empID);

            if (leaveCountEndDate == leaveStart)
            {
                leaveCountEndDate = DateTime.Now;
            }

            return leaveCountEndDate;
        }

        private async Task<List<leaveModel>> GetLeaves(int empID)
        {
            List<leaveModel> leaves =await _context.Leaves.Include(l=>l.leaveTypeModel).Where(l=> l.employmentID == empID).ToListAsync();
            return leaves;
        }
        private async Task<List<leaveModel>> GetUsedLeaves(int empID) {
            return await _context.Leaves
            .Include(l => l.leaveTypeModel)
            .Where(l => l.employmentID == empID
                     && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative
                     && (l.leaveStatus == leaveStatus.Posted || l.leaveStatus == leaveStatus.Completed))
            .ToListAsync();
        }
        private async Task<List<leaveModel>> GetAccruedLeaves(int empID)
        {
            var accrLeaves =await GetLeaves(empID);
            List<leaveModel> accruedLeaves = accrLeaves.Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).ToList(); ;
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
        /// <summary>
        /// Calculate the number of working days (excluding Sundays & active holidays)
        /// multiplied by the number of active employees in the company.
        /// </summary>
        public decimal GetWorkingDays(DateTime startDate, DateTime endDate)
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
            workingDays = workingDays == 0 ? 1 : workingDays;
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
        /// CHECK IF LOGGED IN USER MATCHES SELECTED EMPLOYEE
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="empID"></param>
        /// <returns>bool</returns>
        public bool IsSelf(string UserName, int? empID)
        {
            if(empID == null)
            {
                return false;
            }

            string useN = _context.Employments
                .Where(e => e.employmentID == empID)
                .Select(e => e.personModel != null && e.personModel.userModel != null
                    ? e.personModel.userModel.UserName
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
        public async Task<List<(int roleID, int? CompanyID)>> GetUserAccessAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new List<(int, int?)>();

            
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return new List<(int, int?)>();

            var accessList = await _context.Accesses
                .AsNoTracking()
                .Where(a => a.userID == user.Id && a.accessStatus == mainStatus.Active)
                .Select(a => new { a.roleID, a.companyID })
                .ToListAsync();
            var grantedAccesses = accessList;
            return grantedAccesses
                .Select(a => (a.roleID, a.companyID))
                .ToList();
        }


        /// <summary>
        /// GET SEVERANCE PAY
        /// </summary>
        /// <param name="empID"></param>
        /// <returns>decimal</returns>
   
        public async Task<decimal> GetSeverance(int empID)
        {
            var employment =await _context.Employments.FirstOrDefaultAsync(e => e.employmentID == empID);
            if (employment == null) return 0;

            var termination = await _context.Terminations.FirstOrDefaultAsync(e => e.employmentID == empID);
            var jobPlacement = await _context.JobPlacements
                .Where(j => j.employmentID == empID)
                .OrderByDescending(j => j.jobPlacementStatus == mainStatus.Active) // Active first
                .ThenByDescending(j => j.jobPlacementDate) // Then latest
                .FirstOrDefaultAsync();

            if (jobPlacement == null || jobPlacement.jobPlacementSalary <= 0) return 0;

            DateTime startDate = employment.employmentDate;
            DateTime endDate = termination?.terminationDate ?? DateTime.Now;

            if (startDate >= endDate) return 0;
            
            decimal salary = jobPlacement.jobPlacementSalary;
            decimal yearsOfService = (decimal) (endDate - startDate).TotalDays/365.25m;

            if (yearsOfService < Global_C.SEVERANCE_LEGIBILITY_YEAR) return 0;
            var severance = 0.0m;

            if(yearsOfService >= 1)
            {
                var additionalYears = Math.Max(0, Math.Floor(yearsOfService) - 1);
                severance = salary + additionalYears * (salary / 3);
            }

            severance = Math.Min(severance, salary * 12);
            return severance;
        }

        /// <summary>
        /// CHECK PROHIBITIONS
        /// </summary>
        /// 
        public async Task<bool> CheckProhibition(int empID, ProhibitionType type)
        {
            return await _context.Prohibitions.AnyAsync(p =>
                p.employmentID == empID &&
                p.prohibitionType == type &&
                p.prohibitionStatus == mainStatus.Active);
        }
        public List<EvalSingleEmployeeReport> GetSingleEvaluationReport(int empId)
        {
            // 1. Fetch data from the view for the specific evaluation
            var viewData = _context.EvaluationSummaryView
                .Where(v => v.employmentID == empId)
                .ToList();

            if (!viewData.Any()) return null;

            // 2. Build the structured report using LINQ GroupBy
            var report = viewData
                .GroupBy(v => new { v.evaluationID })
                .Select(eGroup => new EvalSingleEmployeeReport
                {
                    evaluationID = eGroup.Key.evaluationID,
                    evaluationName = eGroup.FirstOrDefault().evaluationName,
                    startDate = eGroup.FirstOrDefault().evaluationStartDate,
                    endDate = eGroup.FirstOrDefault().evaluationEndDate,

                    // Total of all weighted subtask scores
                    FinalGrandTotal = eGroup.Sum(x => x.WeightedSubTaskScore),

                    Types = eGroup.GroupBy(t => new { t.evaluationTypeName, t.evaluationTypeWeight })
                        .Select(tGroup => new EvalTypeSummary
                        {
                            typeName = tGroup.Key.evaluationTypeName,
                            typeWeight = tGroup.Key.evaluationTypeWeight,

                            Tasks = tGroup.GroupBy(tk => new { tk.evaluationTaskName, tk.evaluationTaskWeight })
                                .Select(tkGroup => new EvalTaskSummary
                                {
                                    taskName = tkGroup.Key.evaluationTaskName,
                                    taskWeight = tkGroup.Key.evaluationTaskWeight,
                                    avgTime = tkGroup.Average(x => x.timeValuation),
                                    avgResource = tkGroup.Average(x => x.resourceValuation),
                                    avgPerformance = tkGroup.Average(x => x.performanceValuation),

                                    // Group by SubTaskID to get unique subtasks
                                    SubTasks = tkGroup.GroupBy(st => st.evaluationSubTaskID)
                                        .Select(stGroup => new EvalSubTaskSummary
                                        {
                                            subtaskName = stGroup.First().evaluationSubTaskName,
                                            subtaskWeight = stGroup.First().evaluationSubTaskWeight,
                                            subTime = stGroup.First().timeValuation,
                                            subResource = stGroup.First().resourceValuation,
                                            subPerformance = stGroup.First().performanceValuation
                                        }).ToList()

                                }).ToList()


                        }).ToList()
                }).ToList();

            return report;
        }

        /// <summary>
        /// GET SUGGESTION WHEn FILLING ADDRESS
        /// </summary>
        /// <param name="level"></param>
        /// <param name="country"></param>
        /// <param name="region"></param>
        /// <param name="zone"></param>
        /// <param name="woreda"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetAddressSuggestions(
        string level,
        Country country,
        string region,
        string zone,
        string woreda,
        string term)
        {
            IQueryable<addressModel> q = _context.Addresses.AsNoTracking();

            q = q.Where(a => a.addressCountry == country);

            if (!string.IsNullOrEmpty(region))
                q = q.Where(a => a.addressRegion == region);

            if (!string.IsNullOrEmpty(zone))
                q = q.Where(a => a.addressZone == zone);

            if (!string.IsNullOrEmpty(woreda))
                q = q.Where(a => a.addressWoreda == woreda);

            return level switch
            {
                "region" => new JsonResult(await q
                    .Where(a => a.addressRegion.Contains(term))
                    .Select(a => a.addressRegion)
                    .Distinct()
                    .Take(10)
                    .ToListAsync()),

                "zone" => new JsonResult(await q
                    .Where(a => a.addressZone.Contains(term))
                    .Select(a => a.addressZone)
                    .Distinct()
                    .Take(10)
                    .ToListAsync()),

                "woreda" => new JsonResult(await q
                    .Where(a => a.addressWoreda.Contains(term))
                    .Select(a => a.addressWoreda)
                    .Distinct()
                    .Take(10)
                    .ToListAsync()),

                "tabya" => new JsonResult(await q
                    .Where(a => a.addressTabya.Contains(term))
                    .Select(a => a.addressTabya)
                    .Distinct()
                    .Take(10)
                    .ToListAsync()),

                _ => new JsonResult(new List<string>())
            };
        }

        /// <summary>
        /// DETERMINE JOB GRADE PROMOTION/DEMOTION
        /// </summary>
        /// 
        public bool IsPromotion(jobGradeModel? currentGrade, int newGradeId)
        {
            var temp = currentGrade?.NextJobGrade;
            while (temp != null)
            {
                if (temp.jobGradeID == newGradeId) return true;
                temp = temp.NextJobGrade; // Move one step higher
            }
            return false;
        }

        /// <summary>
        /// PERCENTILE/QUARETILES
        /// </summary>
        /// 
        double Percentile(double[] sortedData, double p)
        {
            if (p < 0 || p > 100) throw new ArgumentException("p must be between 0 and 100");
            double pos = p / 100 * (sortedData.Length - 1);
            int lower = (int)Math.Floor(pos);
            int upper = (int)Math.Ceiling(pos);
            if (lower == upper) return sortedData[lower];
            return sortedData[lower] + (sortedData[upper] - sortedData[lower]) * (pos - lower);
        }
        /// <summary>
        /// GET WORKING DAYS
        /// </summary>
        public int GetWorkingDaysInPeriod(DateTime start, DateTime end)
        {
            // implement business calendar logic (exclude weekends)
            int days = (end.Date - start.Date).Days + 1;

            if (start > end) { return 0; }

            int workingDays = 0;
            for (int i = 0; i < days; i++)
            {
                var day = start.AddDays(i);
                if (day.DayOfWeek == DayOfWeek.Sunday) continue; // exclude Sundays 
                workingDays++;
            }
            return workingDays;
        }
        public Core() { }

    }
    

}
