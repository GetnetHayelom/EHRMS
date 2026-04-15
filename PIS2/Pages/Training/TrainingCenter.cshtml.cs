using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
namespace PIS2.Pages.Training
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER, MIE\\PMS_MANAGEMENT")]
    public class TrainingCenter : PageModel
{
    private readonly PISContext _context;

    public TrainingCenter(PISContext context)
    {
        _context = context;
    }

    // KPIs
    public int TotalTrainings { get; set; }
    public int TotalSessions { get; set; }
    public int CompletedSessions { get; set; }
    public int TotalParticipants { get; set; }
    public decimal TotalCost { get; set; }

    // Charts
    public Dictionary<string, int> SessionStatusStats { get; set; } = new();
    public Dictionary<string, int> ResultStats { get; set; } = new();
    public Dictionary<string, int> CategoryStats { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalTrainings = await _context.Trainings.CountAsync();
        TotalSessions = await _context.TrainingSessions.CountAsync();
        CompletedSessions = await _context.TrainingSessions
                                .CountAsync(s => s.sessionStatus == trainingStatus.Completed);

        TotalParticipants = await _context.TrainingAttendances.CountAsync();

        TotalCost = await _context.TrainingCostAllocations.SumAsync(c => (decimal?)c.allocatedCost) ?? 0;

        SessionStatusStats = await _context.TrainingSessions
            .GroupBy(s => s.sessionStatus)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

        ResultStats = await _context.TrainingAttendances
            .GroupBy(a => a.result)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

        CategoryStats = await _context.Trainings
            .GroupBy(t => t.category)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);
    }
}

}
