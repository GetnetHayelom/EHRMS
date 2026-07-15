using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Data;
using PIS2.Enums;


namespace PIS2.Pages.TrainingSession
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        public trainingSessionModel Session { get; set; } = default!;

        [BindProperty]
        public trainingAttendanceModel NewAttendance { get; set; } = new();
        public SelectList EmpList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            
            Session = await _context.TrainingSessions
                .Include(s => s.Training)
                .Include(s => s.PersonModel)
                .Include(s => s.Attendances)
                    .ThenInclude(a => a.EmploymentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.trainingSessionID == id);

            var inAttendance = Session?.Attendances?.Select(a => a.employmentID).ToList();
            // 1. Get the data from the database first
            var managerData = await _context.Employments
                .Include(e => e.personModel)
                .Where(e => e.employmentStatus == mainStatus.Active && !inAttendance.Contains(e.employmentID))
                .Select(e => new
                {
                    EmpID = e.employmentID,
                    // Combine ID and Name for the dropdown display
                    FullName = e.givenID + " - " + e.personModel.personFullName
                })
                .ToListAsync();

            // 2. Assign it to the SelectList
            // Parameters: (Items, DataValueField, DataTextField)
            EmpList = new SelectList(managerData, "EmpID", "FullName");
            if (Session == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAddAttendanceAsync()
        {

            ModelState.Clear();

            NewAttendance.modifiedBy = User.Identity.Name;
            NewAttendance.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");

                    }
                }
                TempData["message"] = ("Error", $"Missing field!");
                // Reload data if validation fails
                await OnGetAsync(NewAttendance.trainingSessionID);
                return Page();
            }
           

            _context.TrainingAttendances.Add(NewAttendance);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = NewAttendance.trainingSessionID });
            
        }
    }
}