using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Pages.Forms
{
    public class InterOfficeModel : PageModel
    {
        // 1. Bind the form model for POST requests
        [BindProperty]
        public MemoModel Memo { get; set; } = new MemoModel();
        public void OnGet()
        {
        }
    }
    // === VIEW MODEL DEFINITION ===
    public class MemoModel
    {
        // Header Fields
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "TO")]
        public string ToDepartment { get; set; }

        [Required]
        [Display(Name = "FROM")]
        public string FromDepartment { get; set; }

        [Required]
        [Display(Name = "SUBJECT")]
        public string Subject { get; set; }

        // Body Content
        [Required]
        public string MemoBody { get; set; }

        // Footer Signatures
        public string PreparedBy { get; set; }
        public string PreparedBySign { get; set; }

        public string ApprovedBy { get; set; }
        public string ApprovedBySign { get; set; }
    }
}
