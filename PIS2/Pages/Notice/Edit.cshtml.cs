using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Notice
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK,MIE\\PMS_HRCLERK")]
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }


        [BindProperty]
        public NoticeModel Notice { get; set; } = new NoticeModel();

        // Added Status message to show the result of the post operation
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        // Method to fetch the existing notice based on ID
        public IActionResult OnGet(int id)
        {
            

            if (id == 0)
            {
                StatusMessage = "Error: Invalid Notice ID provided for editing.";
                return RedirectToPage("/Notice/Index"); 
            }

            
            Notice = _context.Notices.Where(n => n.noticeID == id).FirstOrDefault();
            

            if (Notice == null)
            {
                StatusMessage = $"Error: Notice with ID {id} not found.";
                return RedirectToPage("/Notice/Index");
            }

            return Page();
        }

        // Method to handle saving the changes
        public async Task<IActionResult> OnPostAsync()
        {
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



            _context.Attach(Notice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoticeExists(Notice.noticeID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Show success message
            StatusMessage = $"Success! Notice ID {Notice.noticeID} ('{Notice.noticeTitle}') has been updated.";

            // Redirect to a details or list page (redirecting to the edit page for simplicity here)
            return RedirectToPage(new { id = Notice.noticeID });
        }
        private bool NoticeExists(int id)
        {
            return _context.Notices.Any(e => e.noticeID == id);
        }
    }
}

