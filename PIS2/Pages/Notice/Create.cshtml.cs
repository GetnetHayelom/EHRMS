using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Notice
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NoticeModel Notice { get; set; } = new NoticeModel();

        // Added Status message to show the result of the post operation
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // Initialize the date fields and default personnel information
            Notice = new NoticeModel
            {
                DatePosted = DateTime.Today,
                // In a real app, this would be set using the current authenticated user's name
                noticePostedBy = "Authenticated User (HR Dept)",
                noticeApprovedBy = "Pending Approval",
                noticePriority = Priority.Medium
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Notice.DatePosted = DateTime.Now;
            Notice.noticePostedBy = User.Identity.Name;
 
            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please check the required fields and try again.";
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return Page();
            }

            _context.Notices.Add(Notice);
            await _context.SaveChangesAsync();

            // Show success message
            StatusMessage = $"Success! Notice '{Notice.noticeTitle}' has been created and submitted for approval (ID: {Notice.noticeID}).";

            // Redirect to the same page (or a list/details page) to clear the form and show the status message
            return RedirectToPage();
        }
    }
}
