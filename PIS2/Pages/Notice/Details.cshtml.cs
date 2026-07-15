using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Enums;

namespace PIS2.Pages.Notice
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        private readonly ILogger<DetailsModel> _logger;
        public DetailsModel(PISContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
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
                return RedirectToPage("/Shared/AccessDenied");
            }

            var notice = await _context.Notices.FirstOrDefaultAsync(l => l.noticeID == Notice.noticeID);
            if (notice == null) { return NotFound(); }
            if (notice.noticeStatus != NoticeStatus.Pending)
            {
                return new JsonResult(new { success=false, message="Notice is Not Pending!"});
            }

            if (notice == null)
            {
                _logger.LogError($"Error: Notice not found #{Notice.noticeID}, User {User.Identity.Name}");
                return NotFound();
            }

            notice.noticeStatus = NoticeStatus.Approved;   // change status here
            notice.modifiedDate = DateTime.Now;
            notice.modifiedBy = User.Identity.Name;

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
                return new JsonResult(new {success=false, message= "Notice approved, but a letter not sent because notice letter type is not configured!"});
            }
            letter.letterTypeID = letterType.letterTypeID;
            letter.letterGroup = LetterGroup.Outgoing;
            letter.letterSubject = notice.noticeSubTitle ?? "";
            letter.modifiedBy = User.Identity.Name;
            letter.modifiedDate = DateTime.Now;

            _context.Letters.Add(letter);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Notice approved successfuly and letter created!" });
        }

        public async Task<IActionResult> OnPostExpireAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER") || !User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                return RedirectToPage("/Shared/AccessDenied");
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

        public async Task<IActionResult> OnPostPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRMANAGER") || !User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }

            var notice = await _context.Notices.FirstOrDefaultAsync(l => l.noticeID == Notice.noticeID);
            if (notice == null) { return NotFound(); }

            if (notice.noticeStatus == NoticeStatus.Void)
            {
                return new JsonResult(new { success=false, message="Notice is void!"});
            }

            notice.noticeStatus = NoticeStatus.Posted;   // change status here
            notice.modifiedDate = DateTime.Now;
            notice.modifiedBy = User.Identity.Name;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Details", new { id = notice.noticeID });
        }
    }

    
}
