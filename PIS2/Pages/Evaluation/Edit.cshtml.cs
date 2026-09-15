using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using System.Threading.Tasks;

namespace PIS2.Pages.Evaluation
{
    [Authorize(Roles ="MANAGEMENT")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        public EditModel(PISContext context) => _context = context;

        public evaluationModel Evaluation { get; set; }
        public List<evaluationTypeModel> EvaluationTypes { get; set; }
        public ICollection<evaluationTaskModel> ExistingTasks { get; set; }
        public List<evaluationValuationModel> ExistingValuations { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // 1. Load the core Evaluation record
            Evaluation = await _context.Evaluations
                .Include(e => e.EmploymentModel).ThenInclude(em => em.personModel)
                .FirstOrDefaultAsync(m => m.evaluationID == id);

            if (Evaluation == null) return NotFound();

            if(Evaluation.modifiedBy != User.Identity.Name && Evaluation.evaluationStatus != Enums.evaluationStatus.Pending) { return RedirectToPage("/Shared/AccessDenied"); }
            // 2. Load the Valuation entries for this specific evaluation
            // These records bridge the Evaluation to the Subtasks
            ExistingValuations = await _context.EvaluationValuations
                .Include(e => e.EvaluationSubTaskModel).ThenInclude(e => e.EvaluationTaskModel).ThenInclude(e => e.EvaluationTypeModel)
                .Where(v => v.evaluationID == id)
                .ToListAsync();

            // 3. Get IDs of Subtasks that have valuations for this evaluation
            var valuatedSubTaskIds = ExistingValuations.Select(v => v.evaluationSubTaskID).ToList();
            
            // 4. Load ONLY related Tasks and Subtasks
            // We load tasks if:
            // a) The Task's Type is "Fixed" (meaning it's a standard requirement)
            // b) OR the Task contains Subtasks that have already been valuated for THIS evaluation
            ExistingTasks = await _context.EvaluationTasks
                .Include(t => t.EvaluationSubTasks)
                .Where(t => t.EvaluationSubTasks.Any(st => valuatedSubTaskIds.Contains(st.evaluationSubTaskID)))
                .ToListAsync();

            var SelectedTasks = ExistingTasks.Select(e => e.evaluationTaskID).ToList();
            // 5. Load Active Evaluation Types
            //EvaluationTypes = await _context.EvaluationTypes.Include(e => e.EvaluationTasks).ThenInclude(t => t.EvaluationSubTasks)
            //    .Where(t => ExistingTasks.Select(es => es.evaluationTypeID).ToList().Contains(t.evaluationTypeID))
            //    .ToListAsync();

            EvaluationTypes = await _context.EvaluationTypes.Include(e => e.EvaluationTasks).ThenInclude(t => t.EvaluationSubTasks)
                .Where(t => Evaluation.evaluationTypes.Contains(t.evaluationTypeID.ToString())).ToListAsync();

            ExistingTasks = EvaluationTypes.SelectMany(t =>
                t.isFixed == true
                    ? t.EvaluationTasks                     // ALL tasks
                    : t.EvaluationTasks.Where(x => SelectedTasks.Contains(x.evaluationTaskID))  // ONLY specific ones
            ).ToList();
            return Page();
        }

        public async Task<JsonResult> OnPostUpdateHeader([FromBody] HeaderDto dto)
        {
            var eval = await _context.Evaluations.FindAsync(dto.id);
            if(eval.modifiedBy != User.Identity.Name) { return new JsonResult(new { success = false, message = "Only the creater of this owner can update evaluations!" }); }
            if (eval.evaluationStatus != Enums.evaluationStatus.Pending) return new JsonResult(new { success = false, message="Evaluation is submitted." });

            eval.evaluationName = dto.evaluationName;
            eval.evaluationStartDate = dto.evaluationStartDate;
            eval.evaluationEndDate = dto.evaluationEndDate;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostCreateTask([FromBody] CreateTaskDto dto)
        {
            var task = new evaluationTaskModel
            {
                evaluationTaskName = dto.evaluationTaskName,
                evaluationTaskWeight = dto.evaluationTaskWeight,
                evaluationTypeID = dto.evaluationTypeID,
                modifiedBy = User.Identity.Name,
                modifiedDate = DateTime.Now
            };
            _context.EvaluationTasks.Add(task);
            await _context.SaveChangesAsync();
            return new JsonResult(new { id = task.evaluationTaskID });
        }

        public async Task<JsonResult> OnPostUpdateTask([FromBody] UpdateDto dto)
        {
            var task = await _context.EvaluationTasks.FindAsync(dto.id);
            task.evaluationTaskName = dto.name;
            task.evaluationTaskWeight = dto.weight;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostDeleteTask([FromBody] IdDto2 dto)
        {
            var task = await _context.EvaluationTasks
                .Include(t => t.EvaluationSubTasks)
                .FirstOrDefaultAsync(t => t.evaluationTaskID == dto.id);

            if (task == null)
                return new JsonResult(new { success = false, message = "Task not found" });

            var subTaskIds = task.EvaluationSubTasks
                .Select(st => st.evaluationSubTaskID)
                .ToList();

            // Remove valuations for THIS evaluation only
            var evaluationsToRemove = await _context.EvaluationValuations
                .Where(v =>
                    subTaskIds.Contains(v.evaluationSubTaskID) &&
                    v.evaluationID == dto.EVAL_ID)
                .ToListAsync();

            _context.EvaluationValuations.RemoveRange(evaluationsToRemove);

            // Check if these subtasks are used by OTHER evaluations
            var hasOtherEvaluations = await _context.EvaluationValuations
                .AnyAsync(v =>
                    subTaskIds.Contains(v.evaluationSubTaskID) &&
                    v.evaluationID != dto.EVAL_ID);

            if (!hasOtherEvaluations)
            {
                _context.EvaluationSubTasks.RemoveRange(task.EvaluationSubTasks);
                _context.EvaluationTasks.Remove(task);
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostCreateSubTask([FromBody] CreateSubTaskDto dto)
        {
            var sub = new evaluationSubTaskModel
            {
                evaluationTaskID = dto.evaluationTaskID,
                evaluationSubTaskName = dto.evaluationSubTaskName,
                evaluationSubTaskWeight = dto.evaluationSubTaskWeight,
                modifiedBy = User.Identity.Name
            };
            _context.EvaluationSubTasks.Add(sub);
            await _context.SaveChangesAsync();
            return new JsonResult(new { id = sub.evaluationSubTaskID });
        }

        public async Task<JsonResult> OnPostUpdateSubTask([FromBody] UpdateDto dto)
        {
            var sub = await _context.EvaluationSubTasks.FindAsync(dto.id);
            sub.evaluationSubTaskName = dto.name;
            sub.evaluationSubTaskWeight = dto.weight;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostDeleteSubTask([FromBody] IdDto dto)
        {
            var valuation = await _context.EvaluationValuations.FindAsync(dto.id);
            if (valuation == null)
                return new JsonResult(new { success = false, message = "Valuation not found" });


            var subTaskId = valuation.evaluationSubTaskID;

            // Remove the valuation first
            _context.EvaluationValuations.Remove(valuation);

            // Check if other valuations still reference this subtask
            var hasOtherValuations = await _context.EvaluationValuations
                .AnyAsync(v => v.evaluationSubTaskID == subTaskId && v.evaluationValuationID != dto.id);

            if (!hasOtherValuations)
            {
                var subTask = await _context.EvaluationSubTasks.FindAsync(subTaskId);
                if (subTask != null)
                    _context.EvaluationSubTasks.Remove(subTask);
            }

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostSaveValuation([FromBody] CreateValauationDto dto)
        {
            var existing = await _context.EvaluationValuations
                .FirstOrDefaultAsync(v => v.evaluationID == dto.evaluationID && v.evaluationSubTaskID == dto.evaluationSubTaskID);

            var newVal = new evaluationValuationModel();
            if (existing != null)
            {
                existing.timeValuation = dto.timeValuation;
                existing.resourceValuation = dto.resourceValuation;
                existing.performanceValuation = dto.performanceValuation;
                existing.modifiedDate = DateTime.Now;
            }
            else
            {
                newVal = new evaluationValuationModel
                {
                    evaluationID = dto.evaluationID,
                    evaluationSubTaskID = dto.evaluationSubTaskID,
                    timeValuation = dto.timeValuation,
                    resourceValuation = dto.resourceValuation,
                    performanceValuation = dto.performanceValuation,
                    modifiedBy = User.Identity.Name
                };
                _context.EvaluationValuations.Add(newVal);
            }
            
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, id = newVal.evaluationValuationID });
        }

        public async Task<JsonResult> OnPostPostEvaluation([FromBody] IdDto dto)
        {
            var eval = await _context.Evaluations.FindAsync(dto.id);
            eval.evaluationStatus = Enums.evaluationStatus.Submitted;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        
    }
    // DTOs
    public class HeaderDto { public int id { get; set; } public string evaluationName { get; set; } public DateTime evaluationStartDate { get; set; } public DateTime evaluationEndDate { get; set; } }

    public class UpdateDto { public int id { get; set; } public string name { get; set; } public decimal weight { get; set; } }
    public class IdDto { public int id { get; set; } }
    public class IdDto2 { public int id { get; set; } public int? EVAL_ID { get; set; } }
}
