using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Foundation;

namespace PIS2.Pages.Notice
{
    [Authorize(Roles = "HRPERSONNEL")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NoticeModel Notice { get; set; } = new NoticeModel();

        // Added Status message to show the result of the post operation
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;
        public SelectList PersonList { get; set; }
        public void OnGet()
        {
            PersonList = new SelectList(_context.Persons, "personFullName", "personFullName");
            // Initialize the date fields and default personnel information
            Notice = new NoticeModel
            {
                DatePosted = DateTime.Today,
                noticePriority = Priority.Medium
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Notice.modifiedBy");
            Notice.modifiedDate = DateTime.Now;
            Notice.modifiedBy = User.Identity.Name;

            Notice.noticeStatus = NoticeStatus.Pending;

            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please check the required fields and try again.";
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }

                return Page();
            }

            _context.Notices.Add(Notice);
            await _context.SaveChangesAsync();

            
            // Redirect to the same page (or a list/details page) to clear the form and show the status message
            return RedirectToPage("Details", new {id =Notice.noticeID });
        }
    }
}
