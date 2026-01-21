using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using static PIS2.Pages.Evaluation.EditModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Pages.Evaluation
{
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        public CreateModel(PISContext context) => _context = context;

        [BindProperty]
        public evaluationModel Evaluation { get; set; } = new();

        public List<evaluationTypeModel> EvaluationTypes { get; set; } = new();

        public SelectList Employee { get; set; }

        public async Task OnGetAsync(int dep, int? emp)
        {

            var tempDep = new departmentModel();

            Employee = new SelectList("", "");

            tempDep = await _context.Departments.FirstOrDefaultAsync(d => d.departmentID == dep);

            if (tempDep != null)
            {
                var employees = _context.JobPlacements.Where(d => d.departmentID == tempDep.departmentID && d.jobPlacementStatus == mainStatus.Active).Select(d => d.employmentID).ToList();

                var employee = await _context.Employments
                .Include(e => e.personModel)
                .Where(e => e.employmentStatus == mainStatus.Active && employees.Contains(e.employmentID))
                .Select(e => new
                {
                    EmpID = e.employmentID,

                    FullName = e.givenID + " - " + e.personModel.personFullName
                })
                .ToListAsync();

                Employee = new SelectList(employee, "EmpID", "FullName", emp);
            }
            else {
                var employee = await _context.Employments
                .Include(e => e.personModel)
                .Where(e => e.employmentStatus == mainStatus.Active)
                .Select(e => new
                {
                    EmpID = e.employmentID,

                    FullName = e.givenID + " - " + e.personModel.personFullName
                })
                .ToListAsync();

                Employee = new SelectList(employee, "EmpID", "FullName", emp);
            }

                // Load types with their predefined tasks/subtasks for the UI
                EvaluationTypes = await _context.EvaluationTypes
                    .Include(t => t.EvaluationTasks)
                        .ThenInclude(tk => tk.EvaluationSubTasks)
                    .Where(t => t.evaluationTypeStatus == mainStatus.Active)
                    .ToListAsync();

        }

        // 1. Create Main Evaluation
        public async Task<JsonResult> OnPostCreateEval([FromBody] CreateEvaluationDto dto)
        {
            var data = new evaluationModel();
            var jobPlacementID = await _context.JobPlacements.Where(j => j.employmentID == dto.employmentID && j.jobPlacementStatus == mainStatus.Active).Select(j => j.jobPlacementID).FirstOrDefaultAsync();
            data.employmentID = dto.employmentID;
            data.jobPlacementID = jobPlacementID;
            data.evaluationName = dto.evaluationName;
            data.evaluationStartDate = dto.evaluationStartDate;
            data.evaluationEndDate = dto.evaluationEndDate;
            data.modifiedDate = DateTime.Now;
            data.modifiedBy = User.Identity.Name;
            data.evaluationStatus = evaluationStatus.Pending;
            _context.Evaluations.Add(data);
            await _context.SaveChangesAsync();
            return new JsonResult(new { id = data.evaluationID });
        }

        // 2. Create Dynamic Task
        public async Task<JsonResult> OnPostCreateTask([FromBody] CreateTaskDto dto)
        {
            var data = new evaluationTaskModel();
            data.evaluationTaskName = dto.evaluationTaskName;
            data.evaluationTaskWeight = dto.evaluationTaskWeight;
            data.evaluationTypeID = dto.evaluationTypeID;
            data.modifiedDate = DateTime.Now;
            data.modifiedBy = User.Identity.Name;
            _context.EvaluationTasks.Add(data);
            await _context.SaveChangesAsync();
            return new JsonResult(new { id = data.evaluationTaskID });
        }

        // 3. Create Dynamic SubTask
        public async Task<JsonResult> OnPostCreateSubTask([FromBody] CreateSubTaskDto dto)
        {
            var data = new evaluationSubTaskModel();
            data.evaluationSubTaskName = dto.evaluationSubTaskName;
            data.evaluationSubTaskWeight = dto.evaluationSubTaskWeight;
            data.evaluationTaskID = dto.evaluationTaskID;
            data.modifiedDate = DateTime.Now;
            data.modifiedBy = User.Identity.Name;

            _context.EvaluationSubTasks.Add(data);
            await _context.SaveChangesAsync();
            return new JsonResult(new { id = data.evaluationSubTaskID });
        }

        // 4. Upsert Valuation (Update if exists, Create if not)
        public async Task<JsonResult> OnPostSaveValuation([FromBody] CreateValauationDto dto)
        {
            var data = new evaluationValuationModel();
            var existing = await _context.EvaluationValuations
                .FirstOrDefaultAsync(v => v.evaluationID == dto.evaluationID && v.evaluationSubTaskID == dto.evaluationSubTaskID);

            if (existing != null)
            {
                existing.timeValuation = dto.timeValuation;
                existing.resourceValuation = dto.resourceValuation;
                existing.performanceValuation = dto.performanceValuation;
                existing.modifiedDate = DateTime.Now;
                existing.modifiedBy = User.Identity.Name;
            }
            else
            {
                data.evaluationID = dto.evaluationID;
                data.evaluationSubTaskID = dto.evaluationSubTaskID;
                data.timeValuation = dto.timeValuation;
                data.resourceValuation = dto.resourceValuation;
                data.performanceValuation = dto.performanceValuation;
                data.modifiedDate = DateTime.Now;
                data.modifiedBy = User.Identity.Name;
                _context.EvaluationValuations.Add(data);
            }
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

        public async Task<JsonResult> OnPostPostEvaluation([FromBody] IdDto dto)
        {
            var eval = await _context.Evaluations.FindAsync(dto.id);

            if (eval.modifiedBy != User.Identity.Name) { return new JsonResult(new { success = false, message = "Evaluation can only be submitted by its creator." }); }

            eval.evaluationStatus = evaluationStatus.Submitted;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        
        
        


    }
    public class CreateEvaluationDto
        {
            public int employmentID { get; set; }
            public string evaluationName { get; set; }
            public DateTime evaluationStartDate { get; set; }
            public DateTime evaluationEndDate { get; set; }
        }
    public class CreateTaskDto { public string evaluationTaskName { get; set; } public int evaluationTypeID { get; set; } public decimal evaluationTaskWeight { get; set; } }
    public class CreateSubTaskDto { public string evaluationSubTaskName { get; set; } public int evaluationTaskID { get; set; } public decimal evaluationSubTaskWeight { get; set; } }
    public class CreateValauationDto { public int evaluationID { get; set; } public int evaluationSubTaskID { get; set; } public decimal timeValuation { get; set; } public decimal resourceValuation { get; set; } public decimal performanceValuation { get; set; } }
}
