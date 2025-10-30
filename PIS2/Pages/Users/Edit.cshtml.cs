using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace PIS2.Pages.Users
{
    [Authorize(Roles = "MIE\\PMS_ADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        public EditModel(PISContext context) => _context = context;

        [BindProperty] public userModel userModel { get; set; } = new();
        public List<userHistoryModel> userHistory { get; set; } = new();
        public List<accessModel> UserAccesses { get; set; } = new();
        public List<companyModel> CompanySelectList { get; set; } = new();
        [BindProperty] public accessModel AccessInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            userModel = await _context.Users
                .Include(u => u.personModel)
                .Include(u => u.UserHistories)
                .FirstOrDefaultAsync(u => u.userID == id);

            if (userModel == null) return NotFound();

            userHistory = userModel.UserHistories?.ToList() ?? new List<userHistoryModel>();
            UserAccesses = await _context.Accesses
                .Include(a => a.CompanyModel)
                .Where(a => a.userID == id)
                .ToListAsync() ?? new List<accessModel>();

            var companies = await _context.Companies.Where(c => c.companyStatus == mainStatus.Active).ToListAsync() ?? new List<companyModel>();
            CompanySelectList = await _context.Companies.Where(c => c.companyStatus == mainStatus.Active)
                .ToListAsync();

            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_ADMIN")) return RedirectToPage("/Shared/AccessDenied");
            userModel.modifiedBy=User.Identity?.Name ?? "SYSTEM";

            var userInDb = await _context.Users.FindAsync(userModel.userID);
            if (userInDb == null) return NotFound();

            
            userInDb.userName = userModel.userName;
            userInDb.userStatus = userModel.userStatus;
            userInDb.modifiedBy = userModel.modifiedBy;

            ModelState.Clear();
            
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
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
                if (!_context.Users.Any(u => u.userID == userModel.userID))
                    return new JsonResult(new { success = false, message = "User not found (concurrency)." });
                else
                    throw;
            }


            // Return success + redirect URL
            return new JsonResult(new
            {
                success = true,
                message = "User updated successfully.",
                redirectUrl = Url.Page("Edit", new { id = userInDb.userID })
            });
        }

        public IActionResult OnPostTest()
        {
            return new JsonResult(new { message = "Handler reached!" });
        }
        [ValidateAntiForgeryToken]
        public IActionResult OnPostSaveAccess()
        {

            try
            {
                // Update existing
                if (AccessInput.accessID > 0)
                {
                    var existing = _context.Accesses.Find(AccessInput.accessID);
                    if (existing == null)
                        return new JsonResult(new { success = false, message = "Access not found." });

                    existing.userGroups =(UserGroups) AccessInput.userGroups;
                    existing.companyID = AccessInput.companyID > 0? AccessInput.companyID :null;
                    existing.accessStatus =(mainStatus) AccessInput.accessStatus;
                    existing.modifiedBy = User.Identity?.Name ?? "SYSTEM";
                }
                else
                {
                    // Create new
                    var newAccess = new accessModel
                    {
                        userID = AccessInput.userID,
                        userGroups = (UserGroups) AccessInput.userGroups,
                        companyID = AccessInput.companyID >0? AccessInput.companyID : null,
                        accessStatus = (mainStatus) AccessInput.accessStatus,
                        modifiedBy = User.Identity?.Name ?? "SYSTEM"
                    };

                    _context.Accesses.Add(newAccess);
                }

                _context.SaveChanges();
                // Return success + redirect URL
                return new JsonResult(new
                {
                    success = true,
                    message = "Access saved successfully.",
                    redirectUrl = Url.Page("Edit", new { id = AccessInput.userID })
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
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
