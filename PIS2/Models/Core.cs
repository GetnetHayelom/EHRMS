using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        const double weeksInAMonth = 4.33;

        //calculate leave balance in a given time interval
        public leaveDetail leaveSummary(employmentModel employee)
        {
            List<employmentHistoryModel> employmentHistories = new List<employmentHistoryModel>();
            List<employmentTypeModel> empType=new List<employmentTypeModel>();

            DateTime startDate = employee.employmentDate;
            DateTime endDate;

            employmentHistories = _context.EmploymentHistories.Include(e => e.employmentTypeModel)
                .Where(e => e.employmentID == employee.employmentID).ToList();
            //check employment status and employment type
            startDate = employmentHistories.Where(eh => eh.employmentTypeModel.isLeaveCount == true).FirstOrDefault().employmentHistoryDate;
            endDate = employmentHistories.Where(eh => eh.employmentTypeModel.isLeaveCount == true).LastOrDefault().employmentHistoryDate;
                        
            if (startDate == endDate)
            { 
                endDate = DateTime.Now;
            }
                
            List<leaveModel>? Leaves = _context.Leaves.Where(l => l.employmentID==employee.employmentID).ToList();
            double baseLeave = employee.EmploymentHistories.LastOrDefault().employmentTypeModel.employmentBaseLeave;
            double annualAccrualRate = employee.EmploymentHistories.LastOrDefault().employmentTypeModel.annualAccrualRate;
            //total number of days between given date
            int days = (endDate - startDate).Days;
            //initialize elapsed year to 0
            int years = 0;
            //number of days that are extra after allocating the total day per year
            int spareDays = 0;
            //the last amount incremented, initialised to the base rate
            double lastAnnualLeaveIncrement = baseLeave;
            //daily accrual rate by deviding last annual increment rate to the number of working days
            double dailyAccrualRate = 0;
            //total amount of leave until the given end time
            double totalLeave = 0;
            //amount of leave that can be utilised
            double allowedLeave = 0;

            //set the given time interval into years and spare days 
            if (days > 365)
            {
                years= (endDate - startDate).Days / 365;
                spareDays = (endDate - startDate).Days % 365;
                lastAnnualLeaveIncrement = baseLeave + years;
                dailyAccrualRate = lastAnnualLeaveIncrement / workingDayPerYear;
                totalLeave = ((lastAnnualLeaveIncrement - baseLeave + 1)/2) * (baseLeave + lastAnnualLeaveIncrement);
                allowedLeave = (totalLeave - lastAnnualLeaveIncrement) + (spareDays * (spareDays + dailyAccrualRate) / 2);
            }
            else
            {
                years= 1;
                spareDays = days;
                lastAnnualLeaveIncrement = baseLeave;
                totalLeave = lastAnnualLeaveIncrement;
                allowedLeave = spareDays * (spareDays + dailyAccrualRate) / 2;
            }
 
            List<leavePerYear> leavesPerYear = new List<leavePerYear>();
            leavePerYear leavePerYear = new leavePerYear();
            List<leaveModel> usedLeaves = new List<leaveModel>();
            double startingLeavePerYear = 0;
            double remainingLeavePerYear = 0;
            DateTime dateCounter = startDate;
            double accrualCounter = baseLeave;
            for (int i = 0; i < years; i++)
            {
                usedLeaves = Leaves.Where(l => l.leaveStartDate >= dateCounter && l.leaveEndDate <= dateCounter.AddYears(1) && l.leaveStatus == leaveStatus.Posted && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).ToList();
                startingLeavePerYear += accrualCounter;
                leavePerYear = new leavePerYear();
                leavePerYear.startDate = dateCounter;
                leavePerYear.endDate = dateCounter.AddYears(1);
                leavePerYear.accruedLeaveAmount = accrualCounter;      
                leavePerYear.usedLeaveAmount = usedLeaves.Sum(l => l.leaveDays);
                leavesPerYear.Add(leavePerYear);
                dateCounter = dateCounter.AddYears(1);
                accrualCounter++;
            }
            
           
            leaveDetail leaveSummary = new leaveDetail(totalLeave, allowedLeave, lastAnnualLeaveIncrement, startDate, endDate, leavesPerYear);
            Console.WriteLine($"Error before leavesummary");
            if (leaveSummary == null) throw new InvalidOperationException("No leave summary.");
            return leaveSummary;
        }
        public leaveDetail GetLeaveSummary(int empID)
        {
            //List<employmentHistoryModel>? EmpHist = new List<employmentHistoryModel>();
            //EmpHist = GetEmpHist(empID);
            //if (EmpHist == null) throw new InvalidOperationException("No employment history found.Cant get base leave.");
            //double baseLeave = EmpHist.LastOrDefault().employmentTypeModel.employmentBaseLeave;
            //double annualAccrualRate = EmpHist.LastOrDefault().employmentTypeModel.annualAccrualRate;
            DateTime startDate = GetLeaveStart(empID);
            DateTime endDate = GetLeaveEnd(empID);
            List<leaveModel> leaves = new List<leaveModel>();
            leaves = _context.Leaves.Where(l=>l.employmentID == empID).Include(l => l.leaveTypeModel).ToList();
            double usedLeave = leaves.Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).Sum(l => l.leaveDays);
            double accruedLeave = leaves.Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).Sum(l => l.leaveDays);
            //total number of days between given date
            int days = (endDate - startDate).Days;
            //initialize elapsed year to 0
            int years = endDate.Year - startDate.Year;
            //number of days that are extra after allocating the total day per year
            int spareDays = 0;
            //the last amount incremented, initialised to the base rate
            double lastAnnualLeaveIncrement = leaves.OrderBy(l => l.leaveReaquestDate).LastOrDefault(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).leaveDays; 
            //daily accrual rate by deviding last annual increment rate to the number of working days
            double dailyAccrualRate =lastAnnualLeaveIncrement / workingDayPerYear;
            //total amount of leave until the given end time
            double totalLeave = 0;
            //amount of leave that can be utilised
            double allowedLeave = 0;
               spareDays = Enumerable.Range(0, ((endDate - (startDate).AddYears(years-1))).Days + 1)
           .Select(offset => startDate.AddDays(offset))
           .Count(date => date.DayOfWeek != DayOfWeek.Sunday);

                totalLeave = accruedLeave - usedLeave;
                allowedLeave = totalLeave - (lastAnnualLeaveIncrement - (spareDays * dailyAccrualRate));
            

            leaveDetail leaveSummary = new leaveDetail(totalLeave, allowedLeave, lastAnnualLeaveIncrement, startDate, endDate, LeavesPerYear(empID));
            return leaveSummary;
        }
      
        public List<leavePerYear> LeavesPerYear(int empID)
        {
            //Returns all leaves of the employee
            List<leaveModel> Leaves = GetLeaves(empID) ?? new List<leaveModel>();
            List<leaveModel> accrued = GetAccruedLeaves(empID)?? new List<leaveModel>();
            List<leaveModel> used = GetUsedLeaves(empID) ?? new List<leaveModel>();
            DateTime startDate = GetLeaveStart(empID);
            DateTime endDate = GetLeaveEnd(empID);
            int years = endDate.Year - startDate.Year;
            List<leavePerYear> leavesPerYear = new List<leavePerYear>();
            leavePerYear leavePerYear = new leavePerYear();

            double lastIncrement =accrued.IsNullOrEmpty()? 0 : accrued.OrderByDescending(l=>l.leaveReaquestDate).FirstOrDefault().leaveDays;
            double totalUsedLeaves = used.Sum(l => l.leaveDays);
            double totalAccruedLeaves = accrued.Sum(l => l.leaveDays);
            double balance = totalAccruedLeaves - totalUsedLeaves;
            double usedLeaves = used.Sum(l => l.leaveDays);
            double accruedLeaves = 0;
            double startingLeavePerYear = 0;
            DateTime dateCounter = startDate;
            double rollOverLeave = 0;
            //double accrualCounter = baseLeave;

            for (int i = 0; i < years; i++)
            {
                
                usedLeaves = used.Where(l => l.leaveReaquestDate >= dateCounter && l.leaveReaquestDate < dateCounter.AddYears(1)).Sum(l =>l.leaveDays);
                accruedLeaves = accrued.Where(l => (l.leaveReaquestDate >= dateCounter && l.leaveReaquestDate < dateCounter.AddYears(1))).Sum(l => l.leaveDays);
                while (lastIncrement > accruedLeaves)
                {
                    if (balance > lastIncrement)
                    {
                        
                        rollOverLeave = balance - lastIncrement;
                        if (rollOverLeave > accruedLeaves)
                        {
                            rollOverLeave = accruedLeaves;
                        }
                    }
                    else { rollOverLeave = 0;break; }
                    balance = balance - lastIncrement;
                    lastIncrement--;
                }
                //for (int j = 30; j > accruedLeaves; j--)
                //{


                //}

                leavePerYear = new leavePerYear();
                leavePerYear.startingLeaveAmount = startingLeavePerYear;
                leavePerYear.startDate = dateCounter;
                leavePerYear.endDate = dateCounter.AddYears(1);
                leavePerYear.accruedLeaveAmount = accruedLeaves;
                leavePerYear.usedLeaveAmount = usedLeaves;
                leavePerYear.rollOverLeave = rollOverLeave;
                
                            leavesPerYear.Add(leavePerYear);

                dateCounter = dateCounter.AddYears(1);
                startingLeavePerYear += (accruedLeaves - usedLeaves);
                lastIncrement = accrued.OrderByDescending(l => l.leaveReaquestDate).FirstOrDefault().leaveDays;
                balance = totalAccruedLeaves - totalUsedLeaves;
            }
            if (balance < lastIncrement)
            {
                leavesPerYear.Last().rollOverLeave = balance;
            }
            else
            {
                leavesPerYear.Last().rollOverLeave = lastIncrement;
            }
            return leavesPerYear;
        }
          public leaveDetail GetLeaveSummary2(int empID)
        {
            List<leaveModel> leaves = new List<leaveModel>();
            leaves = _context.Leaves.Where(l=>l.employmentID == empID).ToList();
            DateTime startDate = leaves.Min(l => l.leaveReaquestDate);
            DateTime endDate = DateTime.Now;
            //total number of days between given date
            int days = (endDate - startDate).Days;
            //initialize elapsed year to 0
            int years;
            //number of days that are extra after allocating the total day per year
            int spareDays = 0;
            //the last amount incremented, initialised to the base rate
            double lastAnnualLeaveIncrement = leaves.Where(l => l.leaveTypeID == 2017).FirstOrDefault().leaveDays;
            //daily accrual rate by deviding last annual increment rate to the number of working days
            double dailyAccrualRate = 0;
            //total amount of leave until the given end time
            double totalLeave = 0;
            //amount of leave that can be utilised
            double allowedLeave = 0;
            double usedLeave =leaves.Where(l => l.leaveTypeModel != null && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).Sum(l => l.leaveDays);
            double accruedleave = leaves.Where(l => l.leaveTypeModel != null && l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).Sum(l => l.leaveDays);
            double leaveBalance = accruedleave - usedLeave;
                
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

            double usedLeaves = used.Sum(l => l.leaveDays);
            double accruedLeaves = accrued.Sum(l => l.leaveDays);
            
            DateTime dateCounter = startDate;
            //double accrualCounter = baseLeave;
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
            //List<employmentHistoryModel> histories=GetEmpHist(empID).ToList();
            //if (histories == null) throw new InvalidOperationException("No employment history found. Cant get start date.");
            //leaveCountStartDate = histories.OrderBy(h =>h.employmentHistoryDate).FirstOrDefault(e => e.employmentTypeModel.isLeaveCount == true).employmentHistoryDate;
            return leaveCountStartDate;
        }
        private DateTime GetLeaveEnd(int empID)
        {
            employmentModel employment = new employmentModel();
            employment = _context.Employments.Where(e => e.employmentID == empID).First();
            DateTime leaveCountEndDate;// = DateTime.Now;
           if(employment.employmentStatus == mainStatus.Inactive)
            {
                leaveCountEndDate = GetEmpHist(empID).Last().employmentHistoryDate;
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
            List<leaveModel> usedLeaves = GetLeaves(empID).Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Negative).ToList();
            return usedLeaves;
        }
        private List<leaveModel> GetAccruedLeaves(int empID)
        {
            List<leaveModel> accruedLeaves = GetLeaves(empID).Where(l => l.leaveTypeModel.leaveTypeImpact == leaveTypeImpact.Positive).ToList(); ;
            return accruedLeaves;
        }
        public Core() { }

    }
}
