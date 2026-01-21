using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.EvaluationType
{
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<evaluationTypeModel> EvaluationTypes { get; set; }

        public void OnGet()
        {
            EvaluationTypes = _context.EvaluationTypes
                .Include(et => et.EvaluationTasks).ThenInclude(et => et.EvaluationSubTasks)
                .OrderBy(e => e.evaluationTypeName)
                .ToList();
        }
    }
}

