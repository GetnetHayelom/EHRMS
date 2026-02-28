using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace PIS2.Pages.AuditLog
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        // Inside your OnGetAsync
        public List<HistoryModel> History { get; set; }
        public List<SelectListItem> TableList { get; set; }
  
        public static List<SelectListItem> GetTableSelectList()
        {
            return new List<SelectListItem>
            {
                new ("Accounts", "Accounts"),
                new ("Addresses", "Addresses"),
                new ("Allowances", "Allowances"),
                new ("Applicants", "Applicants"),
                new ("Attachments", "Attachements"),
                new ("Bank Information", "BankInfos"),
                new ("Business Units", "BusinessUnits"),
                new ("Companies", "Companies"),
                new ("Departments", "Departments"),
                new ("Deductions", "Deductions"),
                new ("Deduction Types", "DeductionTypes"),
                new ("Earnings", "Earnings"),
                new ("Earning Types", "EarningTypes"),
                new ("Education Levels", "EducationLevels"),

                new ("Evaluations", "Evaluations"),
                new ("Evaluation Scores", "EvaluationValuations"),
                new ("Evaluation Sub Tasks", "EvaluationSubTasks"),
                new ("Evaluation Tasks", "EvaluationTasks"),
                new ("Evaluation Types", "EvaluationTypes"),

                new ("Experiences", "Experiences"),
                new ("Jobs", "Jobs"),
                new ("Job Categories", "JobCategories"),
                new ("Job Grades", "JobGrades"),
                new ("Leave Types", "LeaveTypes"),
                new ("Other Payments", "OtherPayments"),
                new ("Overtime Types", "Overtimes"),

                new ("Payroll Payments", "PayrollPays"),
                new ("Payroll Runs", "Payrolls"),

                new ("Penalty Types", "PenaltyTypes"),
                new ("Shifts", "Shifts"),
                new ("Shift Assignments", "ShiftAssignments"),
                new ("Tax Rates", "TaxRates"),
                new ("Terminations", "Terminations"),
                new ("Vacancies", "Vacancies"),
            };
        }

        public async Task OnGetAsync(string? tableName, int? recordId)
        {
            TableList = GetTableSelectList();
            var logs = await _context.AuditLogs
                .Where(x => x.TableName == tableName && x.RecordID == recordId)
                .OrderByDescending(x => x.ModifiedDate)
                .ToListAsync();

            // Group by User and Time (to the nearest minute) to treat simultaneous changes as one "Action"
            History = logs.GroupBy(l => new {
                l.ModifiedBy,
                Date = new DateTime(l.ModifiedDate.Year, l.ModifiedDate.Month, l.ModifiedDate.Day, l.ModifiedDate.Hour, l.ModifiedDate.Minute, 0)
            })
                .Select(g => new HistoryModel
                {
                    User = g.Key.ModifiedBy,
                    Date = g.First().ModifiedDate,
                    Changes = g.ToList()
                }).ToList();
        }
    }
    public class HistoryModel
    {
        public string User { get; set; }
        public DateTime Date { get; set; }
        public List<Models.AuditLog> Changes { get; set; } = new();
    }

}
