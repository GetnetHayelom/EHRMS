using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Leave
{
    public class DetailsModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public DetailsModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public leaveModel leaveModel { get; set; } = default!;
        public jobPlacementModel Job { get; set; } = default!;
        public bool isSelf { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leavemodel = await _context.Leaves.Include(l => l.employmentModel).ThenInclude(e => e.personModel)
                .Include(l => l.leaveTypeModel)
                .Include(l => l.LeaveHistories)
                .FirstOrDefaultAsync(m => m.leaveID == id);
            if (leavemodel == null)
            {
                return NotFound();
            }
            else
            {
                leaveModel = leavemodel;
                Job = _context.JobPlacements.Include(j => j.shiftModel)
                    .Include(j => j.departmentModel).ThenInclude(d => d.companyModel)
                    .Include(j=> j.workSiteModel).OrderByDescending(j => j.jobPlacementDate).First(j => j.employmentID == leaveModel.employmentID);

                var currentUser = _context.Users.FirstOrDefault(u => u.userName == User.Identity.Name);

                if (currentUser != null && currentUser.personID == leaveModel.employmentModel.personID)
                {
                    isSelf = true;
                }

            }
            return Page();
        }
        // Post handler
        [BindProperty]
        public int leaveId { get; set; }
        [HttpPost]
        public async Task<IActionResult> OnPost(int leaveId)
        {
            Console.WriteLine($"Received ID: {leaveId}");
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound();
            }
            
            if(leave.leaveStatus == leaveStatus.Hold || leave.leaveStatus == leaveStatus.Approved)
            {
                // Update the leaveStatus
                leave.leaveStatus = leaveStatus.Posted;
                _context.Update(leave);
                await _context.SaveChangesAsync();
                return RedirectToPage(new {id = leaveId});
            }

            return Page();
   
        }
    }
}
