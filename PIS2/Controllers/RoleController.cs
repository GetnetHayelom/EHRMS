using Microsoft.AspNetCore.Mvc;
using PIS2.Models;

namespace PIS2.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = "Admin")] // Only existing admins can assign roles
    public class RoleAdminController : Controller
    {
        private readonly UserManager<userModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleAdminController(UserManager<userModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Role does not exist");

            // Add user to the specified role in AspNetUserRoles table
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction("ManageUsers");
        }
    }

    // Whole controller restricted to HRManager role
    [Authorize(Roles = "HRManager")]
    public class HRController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Accessible by EITHER HRManager OR Admin
        [Authorize(Roles = "HRManager, Admin")]
        public IActionResult Reports()
        {
            return View();
        }
    }
}
