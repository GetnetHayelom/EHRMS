using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Leave
{
    public class EditModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public EditModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public leaveModel leaveModel { get; set; } = default!;
        public bool isSelf { get; set; } = false;
        public bool isHold { get; set; } = false;
        public List<leaveTypeModel> AllowedLeaveTypes { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            LoadPageData(id);
            if(leaveModel == null)
            {
                return NotFound();
            }
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid)
            {
                return Page();
                
            }

            _context.Attach(leaveModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                LoadPageData(leaveModel.leaveID);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!leaveModelExists(leaveModel.leaveID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //return Page();
            return RedirectToPage(new { id =leaveModel.leaveID});
        }

        private bool leaveModelExists(int id)
        {
            return _context.Leaves.Any(e => e.leaveID == id);
        }
        private void LoadPageData(int? id)
        {
            var leavemodel = _context.Leaves.Include(l => l.employmentModel).ThenInclude(e => e.personModel).FirstOrDefault(m => m.leaveID == id);
            
            leaveModel = leavemodel;
            isHold = leaveModel.leaveStatus == leaveStatus.Hold ? true : false;

            
            var leaveTypes = new List<leaveTypeModel>();

            if (User.IsInRole("MIE\\PMS_CLINIC"))
            {
                leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Clinic" || lt.leaveAvailability == "Everyone").ToList();
            }
            else if (User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveAvailability == "HR" || lt.leaveAvailability == "Everyone").ToList();
            }
            else
            {
                leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveAvailability == "Everyone").ToList();
            }
            leaveTypes = leaveTypes.Where(lt => lt.leaveTypeStatus == mainStatus.Active).ToList();
            AllowedLeaveTypes = leaveTypes;
            ViewData["leaveTypeID"] = new SelectList(AllowedLeaveTypes, "leaveTypeID", "leaveTypeName");

            if (_context.Users.First(u => u.userName == User.Identity.Name).personID == _context.Employments.First(e => e.employmentID == leaveModel.employmentID).personID)
            {
                isSelf = true;
            }
        }
    }
}
