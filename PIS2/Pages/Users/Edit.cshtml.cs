using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Foundation;
using PIS2.Models.Organization;
using PIS2.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PIS2.Pages.Users
{
    [Authorize(Roles = "ADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly UserManager<userModel> _userManager;
        private readonly RoleManager<IdentityRole<int>> _role;
        private readonly AccessService _accessService;
        public EditModel(PISContext context, UserManager<userModel> userManager, RoleManager<IdentityRole<int>> role, AccessService accessService) { _context = context; _userManager = userManager; _role = role; _accessService = accessService; }

        [BindProperty] public userModel userModel { get; set; } = new();
        public List<userHistoryModel> userHistory { get; set; } = new();
        public List<accessModel> UserAccesses { get; set; } = new();
        public List<companyModel> CompanySelectList { get; set; } = new();
        [BindProperty] public accessModel AccessInput { get; set; } = new();
        public SelectList RolesList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            userModel = await _context.Users
                .Include(u => u.personModel)
                .Include(u => u.UserHistories)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userModel == null) return NotFound();

            await LoadRoles();

            userHistory = userModel.UserHistories?.ToList() ?? new List<userHistoryModel>();
            UserAccesses = await _context.Accesses
                .Include(a => a.Role)
                .Include(a => a.CompanyModel)
                .Where(a => a.userID == id)  
                .ToListAsync() ?? new List<accessModel>();

            var companies = await _context.Companies.Where(c => c.companyStatus == mainStatus.Active).OrderBy(c => c.companyName).ToListAsync() ?? new List<companyModel>();
            CompanySelectList = await _context.Companies.Where(c => c.companyStatus == mainStatus.Active)
                .ToListAsync();

            
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("ADMIN")) return RedirectToPage("/Shared/AccessDenied");

            ModelState.Remove("userModel.modifiedBy");

            var userInDb = await _context.Users.FindAsync(userModel.Id);
            if (userInDb == null) return NotFound();

 
            userInDb.UserName = userModel.UserName;
            userInDb.userStatus = userModel.userStatus;
            userInDb.modifiedBy = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}"); TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                return new JsonResult(new { success = false, message = "Data not valid." });
            }
 
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == userModel.Id))
                    return new JsonResult(new { success = false, message = "User not found (concurrency)." });
                else
                    throw;
            }


            // Return success + redirect URL
            return new JsonResult(new
            {
                success = true,
                message = "User updated successfully.",
                redirectUrl = Url.Page("Edit", new { id = userInDb.Id })
            });
        }

        public IActionResult OnPostTest()
        {
            return new JsonResult(new { message = "Handler reached!" });
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSaveAccessAsync()
        {
            //try
            //{
                var result = await _accessService.AddAccessAsync(AccessInput.userID, AccessInput.roleID, AccessInput.companyID, User.Identity?.Name);

                if (!result.Success)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = result.Message
                    });
                }

                return new JsonResult(new
                {
                    success = true,
                    message = result.Message,
                    redirectUrl = Url.Page(
                        "/Users/Edit",
                        new { id = AccessInput.userID })
                });

            //}
            //catch (Exception ex)
            //{
            //    return new JsonResult(new { success = false, message = ex.Message });
            //}
        }

        public IActionResult OnPostUpdateAccess(int accessID)
        {
            var access = _context.Accesses.FirstOrDefault(a => a.accessID == accessID);
            if(access != null)access.accessStatus = access.accessStatus == mainStatus.Active? access.accessStatus = mainStatus.Inactive: access.accessStatus = mainStatus.Active;

            try
            {
                _context.SaveChanges();

                return new JsonResult(new
                {
                    success=true,
                    message ="Access Updated!",
                    redirectUrl ="Edit?id=" + access?.accessID
                });

            }
            catch (Exception ex)
            {

                return new JsonResult(new
                {
                    success = false,
                    message = "Access Update Failed!"
                });
            }
            
        }
        public IActionResult OnGetAccessTablePartial(int userID)
        {
            // Fetch access data for the given user
            var accessList = _context.Accesses
                .Where(a => a.userID == userID)
                .Include(a => a.CompanyModel) // optional, if you have related data
                .Include(a => a.userModel)
                .ToList();

            // Return the partial view with the data
            return Partial("_AccessTablePartial", accessList);
        }

       
        public async Task<IActionResult> OnPostResetPasswordAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("./Index");
            }

            // Prevent admin from resetting his/her own password
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && user.Id == currentUser.Id)
            {
                TempData["ErrorMessage"] =
                    "You cannot reset your own password from this page. " +
                    "Use Change Password instead.";

                return RedirectToPage("./Edit", new { id });
            }

            const string temporaryPassword = "123456.aA";

            // Generate a password reset token
            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password
            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    token,
                    temporaryPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                TempData["ErrorMessage"] =
                    $"Password reset failed: {errors}";

                return RedirectToPage("./Edit", new { id });
            }

            // Force password change at next login
            user.MustChangePassword = true;

            // Optional: unlock the account
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;

            var updateResult =
                await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    updateResult.Errors.Select(e => e.Description));

                TempData["ErrorMessage"] =
                    $"Password was reset, but user status could not be updated: {errors}";

                return RedirectToPage("./Edit", new { id });
            }

            TempData["SuccessMessage"] =
                $"Password for '{user.UserName}' has been reset successfully. " +
                "The user must change the password at the next login.";

            return RedirectToPage("./Edit", new { id });
        }

        private async Task LoadRoles()
        {
            var excludedRoles = new[]
            {
                AppRoles.User
            };

                var roles = await _role.Roles
                    .Where(r => r.Name != null &&
                                !excludedRoles.Contains(r.Name))
                    .OrderBy(r => r.Name)
                    .Select(r => new
                    {
                        Value = r.Id,
                        Text = r.Name
                    })
                    .ToListAsync();

                RolesList = new SelectList(
                    roles,
                    "Value",
                    "Text");
            }
    }
    public class AccessInputDto
    {
        public int accessID { get; set; }
        public int userGroups { get; set; } // or int if you send enum value
        public int? companyID { get; set; }
        public int accessStatus { get; set; } // or int
        public int userID { get; set; }
    }

}
