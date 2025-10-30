using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Notices
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public List<NoticeModel> Notices { get; set; }

        public async Task OnGetAsync()
        {
            Notices = await _context.Notices
                .Where(n => n.IsActive &&
                           (!n.ExpiryDate.HasValue || n.ExpiryDate > DateTime.Now))
                .OrderByDescending(n => n.DatePosted)
                .ToListAsync();
        }
    }
}
