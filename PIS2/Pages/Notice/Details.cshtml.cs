using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Notice
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;

        public DetailsModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NoticeModel Notice { get; set; }

        public IActionResult OnGet(int id)
        {
            Notice = _context.Notices.FirstOrDefault(n => n.noticeID == id);

            if (Notice == null)
            {
                return NotFound();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostApproveAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER"))
            {
                TempData["message"] = ("Error", "Access Denied!");
            }

            var notice = await _context.Notices.FirstOrDefaultAsync(l => l.noticeID == Notice.noticeID);
            if (notice == null) { return NotFound(); }
            if (notice.noticeStatus != NoticeStatus.Pending)
            {
                TempData["message"] = ("Error", "Notice Not Approved!");
                return Page();
            }

            if (notice == null)
            {
                return NotFound();
            }

            notice.noticeStatus = NoticeStatus.Approved;   // change status here
            notice.modifiedDate = DateTime.Now;
            notice.modifiedBy = User.Identity.Name;

            TempData["message"] = ("Success", "Letter Approved!");

            await _context.SaveChangesAsync();

            var letter = new letterModel();
            letter.letterBody = notice.noticeContent;
            letter.letterTitle = notice.noticeTitle ?? "";
            letter.letterDate = notice.DatePosted.Date;
            letter.letterSender = notice.noticeFrom ?? "";
            letter.letterReceiver = notice.noticeTo ?? "";
            letter.letterSignedBy = notice.noticeSignedBy ?? "";
            letter.letterStatus = LetterStatus.Approved;
            var letterType = await _context.LetterTypes
                .FirstOrDefaultAsync(t => t.letterTypeName.Contains("Notice"));
            
            if (letterType == null)
            {
                TempData["message"] = ("Error", "Letter not sent because notice letter type is not configured!");return Page();
            }
            letter.letterTypeID = letterType.letterTypeID;
            letter.letterGroup = LetterGroup.Outgoing;
            letter.letterSubject = notice.noticeSubTitle ?? "";
            letter.modifiedBy = User.Identity.Name;
            letter.modifiedDate = DateTime.Now;

            _context.Letters.Add(letter);
            await _context.SaveChangesAsync();

            TempData["message"] = ("Success", "Notice Approved and New Letter Created Successfully!");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExpireAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER") || !User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                TempData["message"] = ("Error", "Access Denied!");
            }

            var notice = await _context.Notices.FirstOrDefaultAsync(l => l.noticeID == Notice.noticeID);
            if (notice == null) { return NotFound(); }

            if (notice.noticeStatus == NoticeStatus.Void)
            {
                TempData["message"] = ("Error", "Notice is void!");
                return Page();
            }

            notice.noticeStatus = NoticeStatus.Expired;   // change status here
            notice.modifiedDate = DateTime.Now;
            notice.modifiedBy = User.Identity.Name;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Details", new { id = notice.noticeID});
        }
    }

    
}
