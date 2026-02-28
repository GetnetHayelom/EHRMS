using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Evaluation
{
    public class DetailsModel : PageModel
    {
        private readonly PISContext _context;
        public DetailsModel(PISContext context) => _context = context;

        public evaluationModel Evaluation { get; set; }
        public List<evaluationTypeModel> EvaluationTypes { get; set; }
        public List<evaluationTaskModel> ExistingTasks { get; set; }
        public List<evaluationValuationModel> ExistingValuations { get; set; }
        public decimal GrandTotalScore { get; set; }
        public decimal GrandWeightScore { get; set; }
        public decimal GrandWeight { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Evaluation = await _context.Evaluations
                .Include(e => e.EmploymentModel).ThenInclude(em => em.personModel)
                .FirstOrDefaultAsync(m => m.evaluationID == id);

            if (Evaluation == null) return NotFound();

            ExistingValuations = await _context.EvaluationValuations
                .Where(v => v.evaluationID == id)
                .ToListAsync();

            var valuatedSubIds = ExistingValuations.Select(v => v.evaluationSubTaskID).ToList();
            
            ExistingTasks = await _context.EvaluationTasks
                .Include(e => e.EvaluationTypeModel)
                .Include(t => t.EvaluationSubTasks)
                .Where(t => t.EvaluationSubTasks.Any(st => valuatedSubIds.Contains(st.evaluationSubTaskID)))
                .ToListAsync();

            var SelectedTasks = ExistingTasks.Select(e => e.evaluationTaskID).ToList();

            EvaluationTypes = await _context.EvaluationTypes.Include(e => e.EvaluationTasks).ThenInclude(t => t.EvaluationSubTasks)
                .Where(t => ExistingTasks.Select(es => es.evaluationTypeID).ToList().Contains(t.evaluationTypeID))
                .ToListAsync();

            ExistingTasks = EvaluationTypes.SelectMany(t =>
                t.isFixed == true
                    ? t.EvaluationTasks                     // ALL tasks
                    : t.EvaluationTasks.Where(x => SelectedTasks.Contains(x.evaluationTaskID))  // ONLY specific ones
            ).ToList();

            CalculateGrandTotal();

            return Page();
        }

        private void CalculateGrandTotal()
        {
            decimal grandScore = 0;
            decimal grandWeight = 0;
            decimal grandPercent = 0;

            foreach (var taskGroup in ExistingTasks.GroupBy(e => e.evaluationTypeID))
            {
                decimal taskTypeWeight = taskGroup.FirstOrDefault().EvaluationTypeModel?.evaluationTypeWeight ?? 0;
                decimal grandSubPercent = 0;
                decimal grandSubScore = 0;
                decimal grandSubWeight = 0;

                foreach (var task in taskGroup)
                {
                    decimal tWeight = task.evaluationTaskWeight;
                    decimal subTotalWeight = 0;
                    decimal subWeightedScore = 0;

                    foreach (var sub in task.EvaluationSubTasks)
                    {
                        var val = ExistingValuations.FirstOrDefault(v => v.evaluationSubTaskID == sub.evaluationSubTaskID);
                        if (val != null)
                        {
                            decimal avg = (val.timeValuation + val.resourceValuation + val.performanceValuation) / 3;
                            subTotalWeight += sub.evaluationSubTaskWeight;
                            subWeightedScore += (avg * sub.evaluationSubTaskWeight);
                            
                        }
                    }

                    if (subTotalWeight > 0)
                    {
                        // Task score = (Achieved Weighted Score / (Max possible 4.0 * Total Weights)) * Task Weight
                        decimal taskResult = (subWeightedScore / (subTotalWeight * 4)) * tWeight;
                        grandScore += taskResult;
                        grandSubScore += taskResult;
                    }
                    grandSubWeight += tWeight;
                    grandWeight += tWeight;
                }

                grandSubPercent += grandSubScore * (taskTypeWeight / grandSubWeight);

                grandPercent += grandSubPercent;
            }
            GrandTotalScore = grandPercent;
            GrandWeight = grandWeight;
            GrandWeightScore = grandScore;
            //GrandTotalScore = totalWeightAllocated > 0 ? (totalWeightedPoints / totalWeightAllocated) * 100 : 0;
        }
    }
}