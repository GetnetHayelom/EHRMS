using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Termination
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        public IndexModel(PISContext context) => _context = context;

        // ------------------------------
        // Filter properties (bound)
        // ------------------------------
        [BindProperty(SupportsGet = true)] public int? CompanyID { get; set; }
        [BindProperty(SupportsGet = true)] public int? DepartmentID { get; set; }
        [BindProperty(SupportsGet = true)] public terminationStatus? Status { get; set; }
        [BindProperty(SupportsGet = true)] public string? Reason { get; set; }
        [BindProperty(SupportsGet = true)] public int? jClass { get; set; }
        [BindProperty(SupportsGet = true)] public int? Category { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? From { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? To { get; set; }

        public List<companyModel> Companies { get; set; } = new();
        // The filtered list
        public IList<TerminationDetailView> terminationModel { get; set; } = new List<TerminationDetailView>();
        public List<jobClassModel> JobClasses { get; set; } = new();
        public List<jobCategoryModel> JobCategories { get; set; } = new();
        public List<string> Reasons { get; set; } = new();
        // Summaries
        public int total { get; set; }
        public int male { get; set; }
        public int female { get; set; }
        public int management { get; set; }
        public int professional { get; set; }
       

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
            JobClasses = await _context.JobClasses.OrderBy(j => j.JobClassName).ToListAsync();
            JobCategories = await _context.JobCategories.OrderBy(j => j.jobCategoryName).ToListAsync();
            Reasons = await _context.Terminations.OrderBy(t => t.terminationReason).Select(t => t.terminationReason).Distinct().ToListAsync();
            // ------------------------------------
            // Base Query with all navigation
            // ------------------------------------
            var q = _context.TerminationDetailView.AsQueryable();

            // ------------------------------------
            // FILTERS
            // ------------------------------------
            if (CompanyID.HasValue && CompanyID > 0)
                q = q.Where(t => t.companyID == CompanyID);

            if (DepartmentID.HasValue && DepartmentID > 0)
                q = q.Where(t =>t.departmentID == DepartmentID);

            if (Status.HasValue)
                q = q.Where(t => t.terminationStatus ==(int) Status);

            if (!string.IsNullOrWhiteSpace(Reason))
                q = q.Where(t => t.terminationReason == Reason);

            if (From.HasValue)
                q = q.Where(t => t.terminationDate >= From.Value);

            if (To.HasValue)
                q = q.Where(t => t.terminationDate <= To.Value);

            // ------------------------------------
            // EXECUTE
            // ------------------------------------
            terminationModel = await q.OrderByDescending(t => t.terminationDate)
                .ToListAsync();

            // ------------------------------------
            // SUMMARIES (Safe)
            // ------------------------------------
            total = terminationModel.Any() ? terminationModel.Count :0;

            male = terminationModel.Any() ? terminationModel
                .Count(t => t.personGender == (int) Gender.Male) : 0;

            female = terminationModel.Any() ? terminationModel
                .Count(t => t.personGender == (int) Gender.Female) : 0;

            management = terminationModel.Count(t =>
                t.jobCategoryName == "Management"
            );

            professional = terminationModel.Count(t =>
                t.jobCategoryName == "Professional"
            );

        }
    }
}
