
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Services
{
    public class LeaveService
    {
        private readonly PISContext _db;
        private readonly ILogger<PayrollService> _logger;
        private const leaveTypeImpact POSITIVE_IMPACT = leaveTypeImpact.Positive;
        private const leaveTypeImpact NEGATIVE_IMPACT = leaveTypeImpact.Negative;

        public LeaveService(PISContext db, ILogger<PayrollService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<ExpiringLeaveDto>> GetAllExpiringLeaves(List<EmployeeDetailView>? emps, int years)
        {
            if (emps == null) 
            {
                emps = await _db.EmployeeDetailViews.Where(e => e.EmploymentStatus == mainStatus.Active).ToListAsync();
            }
            var empIDs = emps.Select(e => e.EmploymentID).ToList();
            var annualLeaves = await _db.LeaveReportView.Where(l => empIDs.Contains(l.EmploymentID) && l.LeaveTypeImpact != leaveTypeImpact.Neutral).ToListAsync();
            var activeLeavesEmps = annualLeaves
                .Where(l => l.LeaveStatus == leaveStatus.Posted || l.LeaveStatus == leaveStatus.Completed)
                    .GroupBy(o => o.EmploymentID)
                    .ToDictionary(g => g.Key, g => g.ToList());

            var ExLeaves =new List<ExpiringLeaveDto>();
            foreach (var emp in emps)
            {
                activeLeavesEmps.TryGetValue(emp.EmploymentID, out var leaves);
                leaves ??= new List<LeaveReportView>();
                var data = GetExpiringLeave(emp, years, leaves);
                ExLeaves.Add(data);
            }

                return ExLeaves;
        }

        public ExpiringLeaveDto GetExpiringLeave(EmployeeDetailView emp, int years, List<LeaveReportView> empLeaves)
        {
            if (emp == null)
            {
                _logger.LogWarning($"Employee not found.");
                return new ExpiringLeaveDto();
            }
            var empDate = emp.EmploymentDate;
            var yearsBetween = DateTime.Now.Year - empDate.Year;

            var cutDate = DateTime.Now.AddYears(-years);

            var leaves = empLeaves
                .Select(l => new {
                    l.LeaveRequestDate,
                    l.LeaveDays,
                    l.LeaveEnd,
                    l.LeaveTypeImpact
                })
                .OrderBy(l => l.LeaveEnd)
                .ToList();


            var givenLeaves = leaves.Where(l => l.LeaveTypeImpact == POSITIVE_IMPACT && l.LeaveRequestDate <= cutDate).ToList();

            var grantedLeave = givenLeaves.Sum(l => l.LeaveDays) ?? 0;

            var lastDate = DateTime.Now;

            if (givenLeaves.Any()) 
            { 
                lastDate = givenLeaves.Max(l => l.LeaveEnd);
                lastDate.AddYears(years);
            }

            var usedLeave = leaves
                .Where(l => l.LeaveTypeImpact == NEGATIVE_IMPACT)
                .Sum(l => l.LeaveDays) ?? 0;

            var expiringDays = Math.Max(0,grantedLeave - usedLeave);
            var expireDto = new ExpiringLeaveDto()
            {
                employmentID = emp.EmploymentID,
                givenID = emp.GivenID,
                FullName = emp.FullName,
                days =expiringDays,
                expDate = lastDate.AddYears(years),
                Gender = emp.PersonGender,
                CompanyID = emp.CompanyID,
                CompanyName = emp.CompanyName,
                DepartmentName = emp.DepartmentName,
                DepartmentID = emp.DepartmentID,
                Cost = expiringDays * emp.Salary/Global_C.DAYS_PER_MONTH
            };
            return expireDto;
        }
    }

    
}
