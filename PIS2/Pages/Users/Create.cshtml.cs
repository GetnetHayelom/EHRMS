using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Pages.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PIS2.Pages.Users
{
    [Authorize(Roles = "MIE\\PMS_ADMIN")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
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
                return Page();
            }

            var exists = await _context.Users.AnyAsync(e => e.personID == userModel.personID || e.userName == userModel.userName);
            if (exists) {
                LoadPersons();
                TempData["message"] = ("Error", "Person or username already exists in users list!");
                return Page();
            }

            
            _context.Users.Add(userModel);
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
