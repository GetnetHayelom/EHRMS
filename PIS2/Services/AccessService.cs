using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PIS2.Data;
using PIS2.Models;
using System.Data;

namespace PIS2.Services
{
    public class AccessService
    {
        private readonly PISContext _context;
        private readonly UserManager<userModel> _userManager;
        private readonly RoleManager<IdentityRole<int>> _role;

        public AccessService(PISContext context, UserManager<userModel> userManager, RoleManager<IdentityRole<int>> role)
        {
            _context = context;
            _userManager = userManager;
            _role = role;
        }

        // =========================================================
        // CHECK ACCESS
        // =========================================================
        public async Task<bool> HasAccessAsync(int userID, int companyID, string roleName)
        {
            return await _context.Accesses
                .AnyAsync(a =>
                    a.userID == userID &&
                    a.companyID == companyID &&
                    a.accessStatus == Enums.mainStatus.Active &&
                    a.Role != null &&
                    a.Role.Name == roleName);
        }

        // =========================================================
        // GET USER ACCESS LIST
        // =========================================================
        public async Task<List<AccessKey>> GetAccessList(int userID)
        {
            return await _context.Accesses
                .Where(a =>
                    a.userID == userID &&
                    a.accessStatus == Enums.mainStatus.Active)
                .Select(a => new AccessKey
                {
                    AccessID = a.accessID,

                    RoleID = a.roleID,
                    RoleName = a.Role!.Name!,

                    CompanyID = a.companyID,
                    CompanyName = a.companyID == null
                        ? "All Companies"
                        : a.CompanyModel!.companyName
                })
                .ToListAsync();
        }


        // =========================================================
        // ADD ROLE ACCESS
        // =========================================================
        public async Task<(bool Success, string Message)> AddAccessAsync(int userID, int roleID, int? companyID, string modifiedBy)
        {
            // -----------------------------------------------------
            // Check user
            // -----------------------------------------------------

            var user = await _userManager.FindByIdAsync(userID.ToString());

            if (user == null)
            {
                return (false, "User does not exist.");
            }

            // -----------------------------------------------------
            // Check role
            // -----------------------------------------------------

            var role = await _role.FindByIdAsync(roleID.ToString());

            if (role == null)
            {
                return (false, "Role does not exist.");
            }

            // -----------------------------------------------------
            // Check company if companyID was supplied
            // -----------------------------------------------------

            

            if (companyID.HasValue)
            {
                if (companyID > 0) 
                {
                    var companyExists = await _context.Companies.AnyAsync(c => c.companyID == companyID.Value);

                    if (!companyExists)
                    {
                        return (false, "Company does not exist.");
                    }
                }
                else
                {
                    companyID = null;
                }
                
            }


            // ---------------------------------------------
            // Add Identity role membership if not present
            // ---------------------------------------------

            if (!await _userManager.IsInRoleAsync(user, role.Name))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role.Name);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

                    return (false, $"Failed to add role: {errors}");
                }
            }

            // -----------------------------------------------------
            // Check whether access already exists
            // -----------------------------------------------------

            var existingAccess = await _context.Accesses.FirstOrDefaultAsync(a => a.userID == userID && a.roleID == roleID && a.companyID == companyID);

            if (existingAccess != null)
            {
                // If it exists but was inactive, reactivate it
                if (existingAccess.accessStatus != Enums.mainStatus.Active)
                {
                    existingAccess.accessStatus =
                        Enums.mainStatus.Active;

                    existingAccess.modifiedBy = modifiedBy;
                    existingAccess.modifiedDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return (
                        true,
                        "Existing access has been reactivated.");
                }

                return (
                    false,
                    "User already has this role for the selected company.");
            }

            // -----------------------------------------------------
            // Create access
            // -----------------------------------------------------

            var access = new accessModel
            {
                userID = userID,
                roleID = roleID,

                // NULL = all companies
                companyID = companyID,

                accessStatus = Enums.mainStatus.Active,

                modifiedBy = modifiedBy,
                modifiedDate = DateTime.Now
            };

            _context.Accesses.Add(access);

            await _context.SaveChangesAsync();

            return (
                true,
                "Role access added successfully.");
        }

        // =========================================================
        // REMOVE ROLE ACCESS
        // =========================================================

        public async Task<(bool Success, string Message)> RemoveAccessAsync(int accessID, string modifiedBy)
        {
            var access = await _context.Accesses
                .FirstOrDefaultAsync(a =>
                    a.accessID == accessID);

            if (access == null)
            {
                return (
                    false,
                    "Access record does not exist.");
            }

            // -----------------------------------------------------
            // Don't physically delete it.
            // Mark it inactive so you retain history.
            // -----------------------------------------------------

            access.accessStatus =
                Enums.mainStatus.Inactive;

            access.modifiedBy = modifiedBy;
            access.modifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return (
                true,
                "Role access removed successfully.");
        }
        // =========================================================
        // REMOVE ROLE ACCESS
        // =========================================================
        public async Task<(bool Success, string Message)> RemoveFromRoleAsync(int userID, int roleID)
        {
            // -----------------------------------------------------
            // Check user
            // -----------------------------------------------------

            var user = await _userManager.FindByIdAsync(userID.ToString());

            if (user == null)
            {
                return (false, "User does not exist.");
            }

            // -----------------------------------------------------
            // Check role
            // -----------------------------------------------------

            var role = await _role.FindByIdAsync(roleID.ToString());

            if (role == null)
            {
                return (false, "Role does not exist.");
            }

            // ---------------------------------------------
            // Add Identity role membership if not present
            // ---------------------------------------------

            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                var roleResult = await _userManager.RemoveFromRoleAsync(user, role.Name);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

                    return (false, $"Failed to add role: {errors}");
                }
            }

            return (false, "User removed from role successfully.");

        }

    }
    public class AccessKey
    {
        public int AccessID { get; set; }

        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public int? CompanyID { get; set; }
        public string CompanyName { get; set; } = "All Companies";
    }
}
