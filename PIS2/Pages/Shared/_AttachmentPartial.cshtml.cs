using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS2.Pages.Shared
{
    public class AttachmentPartialModel : PageModel
    {
        public string tableName { get; set; }
        public int recordId { get; set; }
        public string fileTitle { get; set; }
        public void OnGet()
        {
        }
    }
}
