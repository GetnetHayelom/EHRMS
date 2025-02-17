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
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<leaveModel> leaveModel { get;set; } = default!;
        [BindProperty]
        public double totalUnposted {  get; set; }= default!;

        public async Task OnGetAsync()
        {
            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel)
                .Where( l=> l.leaveStatus == leaveStatus.Hold).ToListAsync();
            totalUnposted = leaveModel.Count();
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
            leave.leaveStatus = leaveStatus.Posted;
            _context.Update(leave);
            await _context.SaveChangesAsync();
            leaveModel = await _context.Leaves
                .Include(l => l.employmentModel)
                .Include(l => l.leaveTypeModel).ToListAsync();
            return Page();
        }
    }
}
