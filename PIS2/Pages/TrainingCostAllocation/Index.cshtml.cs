using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.TrainingCostAllocation;

public class IndexModel : PageModel
{
    private readonly PISContext _context;
    public IndexModel(PISContext context) => _context = context;

    public IList<trainingCostAllocationModel> Allocations { get; set; } = default!;
    public decimal TotalCost { get; set; }

    public async Task OnGetAsync()
    {
        Allocations = await _context.TrainingCostAllocations
            .Include(a => a.Attendance)
                .ThenInclude(att => att.EmploymentModel).ThenInclude(e => e.personModel)
            .Include(a => a.Attendance)
                .ThenInclude(att => att.TrainingSession)
                    .ThenInclude(s => s.Training)
            .ToListAsync();

        TotalCost = Allocations.Sum(a => a.allocatedCost);
    }
}