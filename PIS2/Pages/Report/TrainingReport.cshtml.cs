using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.Report
{
    public class TrainingReport : PageModel
    {
        private readonly PISContext _context;

        public TrainingReport(PISContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? From { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? To { get; set; }

        public int TotalTrainings { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public decimal TotalCost { get; set; }

        public Dictionary<string, int> StatusStats { get; set; } = new();
        public Dictionary<string, int> ResultStats { get; set; } = new();
        public Dictionary<string, int> CategoryStats { get; set; } = new();

        public int Compliant { get; set; }
        public int ExpiringSoon { get; set; }
        public int Overdue { get; set; }

        public async Task OnGetAsync()
        {
            var sessions = _context.TrainingSessions.AsQueryable();
            var attendance = _context.TrainingAttendances
                                    .Include(a => a.TrainingSession)
                                    .ThenInclude(s => s.Training)
                                    .AsQueryable();

            if (From.HasValue)
            {
                sessions = sessions.Where(s => s.startDate >= From);
                attendance = attendance.Where(a => a.TrainingSession.startDate >= From);
            }

            if (To.HasValue)
            {
                sessions = sessions.Where(s => s.endDate <= To);
                attendance = attendance.Where(a => a.TrainingSession.endDate <= To);
            }

            TotalTrainings = await _context.Trainings.CountAsync();
            TotalSessions = await sessions.CountAsync();
            CompletedSessions = await sessions.CountAsync(s => s.sessionStatus == trainingStatus.Completed);

            TotalCost = await _context.TrainingCostAllocations
                            .SumAsync(c => (decimal?)c.allocatedCost) ?? 0;

            StatusStats = await sessions
                .GroupBy(s => s.sessionStatus)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

            ResultStats = await attendance
                .GroupBy(a => a.result)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

            CategoryStats = await _context.Trainings
                .GroupBy(t => t.category)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

            var today = DateTime.Today;

            Compliant = await attendance.CountAsync(a =>
                a.certificateExpiryDate == null || a.certificateExpiryDate > today.AddMonths(1));

            ExpiringSoon = await attendance.CountAsync(a =>
                a.certificateExpiryDate != null &&
                a.certificateExpiryDate <= today.AddMonths(1) &&
                a.certificateExpiryDate > today);

            Overdue = await attendance.CountAsync(a =>
                a.certificateExpiryDate != null &&
                a.certificateExpiryDate <= today);
        }
    }
 }
