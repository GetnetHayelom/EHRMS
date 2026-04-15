using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Pages.TrainingSessionAttendance
{
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        public EditModel(PISContext context) => _context = context;

        [BindProperty]
        public trainingAttendanceModel Attendance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Attendance = await _context.TrainingAttendances
                .Include(ts => ts.EmploymentModel).ThenInclude(e => e.personModel)
                .FirstOrDefaultAsync(m => m.trainingAttendanceID == id);
            if (Attendance == null) return NotFound();

            ViewData["EmployeeName"] = _context.Employments.Find(Attendance.employmentID).personModel.personFullName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Attendance.modifiedBy");
            Attendance.modifiedBy = User.Identity.Name;
            Attendance.modifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                var ms = "";
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        ms += $"{kv.Key},";
                    }
                }
                TempData["message"] = ("Error", $"{ms} --> Invalid Filed!");

                return Page();
            }

            _context.Attach(Attendance).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TrainingAttendances.Any(e => e.trainingAttendanceID == Attendance.trainingAttendanceID))
                    return NotFound();
                else throw;
            }

            return RedirectToPage("/TrainingSessionAttendance/Details", new { id = Attendance.trainingAttendanceID });
        }
    }
}