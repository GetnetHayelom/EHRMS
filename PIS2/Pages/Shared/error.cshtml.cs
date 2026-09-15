using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS2.Pages.Shared
{
    public class ErrorModel : PageModel
    {
        public string ErrorCode { get; set; } = "ERR-500";

        public string ErrorMessage { get; set; }
            = "An unexpected error occurred.";

        public string ErrorTitle { get; set; }
            = "Unable to Continue";

        public void OnGet(
            string? code,
            string? message,
            string? title)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                ErrorCode = code;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                ErrorMessage = message;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                ErrorTitle = title;
            }
        }
    }
}