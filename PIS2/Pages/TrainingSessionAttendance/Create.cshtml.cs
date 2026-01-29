using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.TrainingSessionAttendance
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        public CreateModel(PISContext context) => _context = context;

        [BindProperty]
        public trainingAttendanceModel Attendance { get; set; } = default!;
        public SelectList SessionList { get; set; } = default!;
        public SelectList EmployeeList { get; set; } = default!;

        public IActionResult OnGet(int? sessionId)
        {
            SessionList = new SelectList(_context.TrainingSessions.Include(s => s.Training), "trainingSessionID", "Training.trainingTitle", sessionId);
            
            EmployeeList = new SelectList(_context.Employments.Include(e => e.personModel), "employmentID", "personModel.personFullName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Attendance.modifiedBy = User.Identity.Name;
            Attendance.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid) return Page();

            _context.TrainingAttendances.Add(Attendance);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index", new { sessionId = Attendance.trainingSessionID });
        }
    }
}