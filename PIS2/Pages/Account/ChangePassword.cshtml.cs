using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS2.Models.Foundation;

namespace PIS2.Pages.Account
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<userModel> _userManager;
        private readonly SignInManager<userModel> _signInManager;

        public ChangePasswordModel(
            UserManager<userModel> userManager,
            SignInManager<userModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [BindProperty]
        public InputModel Input { get; set; } = new();


        public string? Message { get; set; }


        public class InputModel
        {
            [Required(
                ErrorMessage = "Please enter your current password.")]
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string CurrentPassword { get; set; } = "";


            [Required(
                ErrorMessage = "Please enter a new password.")]
            [StringLength(
                100,
                ErrorMessage =
                    "The {0} must be at least {2} and at most {1} characters long.",
                MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; } = "";


            [Required(
                ErrorMessage =
                    "Please confirm your new password.")]
            [DataType(DataType.Password)]
            [Compare(
                "NewPassword",
                ErrorMessage =
                    "The new password and confirmation password do not match.")]
            [Display(Name = "Confirm New Password")]
            public string ConfirmPassword { get; set; } = "";
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }


            // ==========================================
            // CHANGE PASSWORD
            // ==========================================

            var result =
                await _userManager.ChangePasswordAsync(
                    user,
                    Input.CurrentPassword,
                    Input.NewPassword);


            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return Page();
            }


            // ==========================================
            // PASSWORD CHANGED SUCCESSFULLY
            // ==========================================

            user.MustChangePassword = false;

            var updateResult =
                await _userManager.UpdateAsync(user);


            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return Page();
            }


            // Refresh authentication cookie because
            // ChangePasswordAsync updates the security stamp.
            await _signInManager.RefreshSignInAsync(user);


            // ==========================================
            // REDIRECT
            // ==========================================

            return RedirectToPage("/Index");
        }
    }
}