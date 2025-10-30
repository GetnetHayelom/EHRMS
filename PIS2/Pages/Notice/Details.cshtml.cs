using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    }
}
