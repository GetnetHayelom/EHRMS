using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using PIS2.Models;

namespace PIS2.Pages.Notice
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK,MIE\\PMS_HRMANAGER")]
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
        public SelectList PersonList { get; set; }
        // Method to fetch the existing notice based on ID
        public IActionResult OnGet(int id)
        {
            
            PersonList = new SelectList(_context.Persons, "personFullName", "personFullName");

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
            if (Notice.noticeStatus != NoticeStatus.Pending)
            {
                return RedirectToPage("./Details", new { id = Notice.noticeID });
            }
            return Page();
        }

        // Method to handle saving the changes
        public async Task<IActionResult> OnPostAsync()
        {
            var existing = await _context.Notices.Where(n => n.noticeID == Notice.noticeID).FirstOrDefaultAsync();

            if (existing == null) {return NotFound();}

            if (Notice.noticeStatus != NoticeStatus.Pending)
            {
                TempData["message"] = ("Error","Only pending notice can be edited!");
                return Page();
            }

            bool isHR = User.IsInRole("MIE\\PMS_HRMANAGER");
            bool isPending = existing.noticeStatus == NoticeStatus.Pending;

            if (!isPending && !isHR)
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            if (Notice.noticeStatus == NoticeStatus.Posted && !isHR)
            {
                StatusMessage = "Only HR manager can approve notice post!";
                return Page();
            }
            ModelState.Remove("Notice.modifiedBy");
            Notice.modifiedDate = DateTime.Now;
            Notice.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                
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

            existing.noticeTitle = Notice.noticeTitle;
            existing.noticeSubTitle = Notice.noticeSubTitle;
            existing.noticeContent = Notice.noticeContent;
            existing.noticePriority = Notice.noticePriority;
            existing.ExpiryDate = Notice.ExpiryDate;
            existing.noticeFrom = Notice.noticeFrom;
            existing.noticeTo = Notice.noticeTo;
            

            if (isHR)
            {
                existing.IsActive = Notice.IsActive;
                existing.DatePosted = Notice.DatePosted;
                existing.noticeSignedBy = Notice.noticeSignedBy;
            }

            existing.modifiedBy = User.Identity?.Name;
            existing.modifiedDate = DateTime.Now;


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
            StatusMessage = $"Success! Notice ID {Notice.noticeID} ('{Notice.noticeTitle}') has been updated." ;

            // Redirect to a details or list page (redirecting to the edit page for simplicity here)
            return RedirectToPage("./Details", new { id = Notice.noticeID });
        }
        private bool NoticeExists(int id)
        {
            return _context.Notices.Any(e => e.noticeID == id);
        }

    }
}

