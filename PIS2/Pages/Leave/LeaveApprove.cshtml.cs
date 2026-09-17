using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.HR;
namespace PIS2.Pages.Leave
{
    public class LeaveApproveModel : PageModel
    {
        private readonly PISContext _context;

        public LeaveApproveModel(PISContext context)
        {
            _context = context;
        }

        public IList<leaveModel> leaveModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            leaveModel = await _context.Leaves.Where(l=> l.leaveStatus == leaveStatus.Hold)
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel).ToListAsync();
            
        }
        // Post handler
        [BindProperty]
        public int leaveId { get; set; }
        [HttpPost]
        public async Task<IActionResult> OnPostApprove(int leaveId)
        {

            Console.WriteLine($"Received ID: {leaveId}");
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound();
            }

            // Update the leaveStatus
            leave.leaveStatus = leaveStatus.Approved;
            _context.Update(leave);
            await _context.SaveChangesAsync();
            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel).ToListAsync();
            return Page();
        }
        [HttpPost]
        public async Task<IActionResult> OnPostReject(int leaveId)
        {
            Console.WriteLine($"Received ID: {leaveId}");
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound();
            }

            // Update the leaveStatus
            leave.leaveStatus = leaveStatus.Declined;
            _context.Update(leave);
            await _context.SaveChangesAsync();
            leaveModel = await _context.Leaves.Where(l => l.leaveStatus == leaveStatus.Hold)
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel).ToListAsync();
            return Page();
        }
    }
}
