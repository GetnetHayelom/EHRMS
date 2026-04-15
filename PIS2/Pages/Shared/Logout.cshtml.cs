using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS2.Pages.Shared
{
    public class LogoutModel : PageModel
    {

        public IActionResult OnPost()
        {
            return RedirectToPage("/LoggedOut");
        }
    }
}
