using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Pages.Shared
{
    public class AttachmentPartialModel : PageModel
    {
        [Display(Name = "Related Record")]
        public string tableName { get; set; }
        [Display(Name = "Related Record ID")]
        public int recordId { get; set; }
        [Display(Name = "Title\\Description")]
        public string fileTitle { get; set; }
        public void OnGet()
        {
        }
    }
}
