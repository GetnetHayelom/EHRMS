using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;

namespace PIS2.Pages.TrainingSessionAttendance
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        public IndexModel(PISContext context) => _context = context;

        public IList<trainingAttendanceModel> Attendances { get; set; } = default!;
        public string SessionInfo { get; set; } = "";
        public int? CurrentSessionId { get; set; }

        public async Task OnGetAsync(int? sessionId)
        {
            CurrentSessionId = sessionId;
            var query = _context.TrainingAttendances
                .Include(a => a.EmploymentModel)
                .Include(a => a.TrainingSession)
                .ThenInclude(s => s.Training)
                .AsQueryable();

            if (sessionId.HasValue)
            {
                query = query.Where(a => a.trainingSessionID == sessionId);
                var session = await _context.TrainingSessions.Include(s => s.Training).FirstOrDefaultAsync(s => s.trainingSessionID == sessionId);
                SessionInfo = session?.Training?.trainingTitle ?? "Unknown Session";
            }

            Attendances = await query.ToListAsync();
        }
    }
}