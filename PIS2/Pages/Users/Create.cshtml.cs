using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Foundation;
using PIS2.Pages.Shared;
using PIS2.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PIS2.Pages.Users
{
    [Authorize(Roles = "ADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<userModel> _userManager;

        public CreateModel(PISContext context, RoleManager<IdentityRole<int>> roleManager, UserManager<userModel> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public SelectList PersonsList { get; set; } = default!;

        public IActionResult OnGet()
        {
            LoadPersons();
            return Page();
        }

        [BindProperty]
        public userModel userModel { get; set; } = default!;        

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("userModel.modifiedBy");
            ModelState.Remove("userModel.userStatus");
            userModel.modifiedBy = User.Identity.Name!;
            userModel.userStatus = mainStatus.Active;
            userModel.MustChangePassword = true;

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                    Console.WriteLine(kv.ToString());
                }
                LoadPersons();
                return Page();
            }

            var exists = await _context.Users.AnyAsync(e =>(e.personID != null && e.personID == userModel.personID) || e.UserName == userModel.UserName);
            if (exists) {
                LoadPersons();
                TempData["message"] = ("Error", "Person or username already exists in users list!");
                return Page();
            }

            // Get USER role from Identity
            var userRole =
                await _roleManager.FindByNameAsync(
                    AppRoles.User);

            if (userRole == null)
            {
                TempData["message"] =
                    ("Error",
                    $"Identity role '{AppRoles.User}' does not exist.");

                LoadPersons();

                return Page();
            }

            // Add user
            var result = await _userManager.CreateAsync(userModel, "123456.aA");

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                LoadPersons();

                return Page();
            }

            // Create company-specific access
            var access = new accessModel
            {
                userID = userModel.Id,

                roleID = userRole.Id,

                // NULL = all companies
                companyID = null,

                accessStatus = mainStatus.Active,

                modifiedBy = User.Identity?.Name ?? "System",

                modifiedDate = DateTime.Now
            };

            _context.Accesses.Add(access);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void LoadPersons()
        {
            var selectEmps = _context.Persons
                .OrderBy(p => p.personFirstName)
                .ThenBy(p => p.personFatherName)
                .ThenBy(p => p.personLastName)
                .Where(p => !_context.Users.Any(u => u.personID == p.personID))
                .Select(p => new
                {
                    Value = p.personID,
                    Text = p.Employments
                            .Where(e => e.employmentStatus == mainStatus.Active)
                            .Select(e => e.givenID.ToString() + " - " + p.personFullName)
                            .FirstOrDefault()
                           ?? p.personFullName
                })
                .ToList();

            PersonsList = new SelectList(selectEmps, "Value", "Text");
        }
    }
}
