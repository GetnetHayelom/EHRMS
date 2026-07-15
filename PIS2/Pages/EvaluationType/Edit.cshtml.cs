using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.EvaluationType
{
    [Authorize(Roles = "MIE\\PMS_HRADMIN")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;

        public EditModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public evaluationTypeModel EvaluationType { get; set; }
        public SelectList JobClasses { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var jobClasses = await _context.JobClasses.Where(j => j.jobClassStatus == mainStatus.Active).ToListAsync();
            JobClasses = new SelectList(jobClasses, "jobClassId", "jobClassName");

            EvaluationType = await _context.EvaluationTypes
                .Include(et => et.EvaluationTasks)
                .ThenInclude(t => t.EvaluationSubTasks).FirstOrDefaultAsync(et => et.evaluationTypeID == id);

            if (EvaluationType == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            decimal otherWeights = _context.EvaluationTypes
                .Where(x => x.evaluationTypeID != EvaluationType.evaluationTypeID
                         && x.evaluationTypeStatus == mainStatus.Active)
                .Sum(x => x.evaluationTypeWeight);

            if (otherWeights + EvaluationType.evaluationTypeWeight > 100)
            {
                ModelState.AddModelError("Error", "Total evaluation weight cannot exceed 100%.");
                return Page();
            }

            EvaluationType.modifiedBy = User.Identity.Name;
            EvaluationType.modifiedDate = DateTime.Now;

            _context.Attach(EvaluationType).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
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
            task.modifiedBy = User.Identity.Name;
            task.modifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostDeleteTask([FromBody] IdDto dto)
        {
            var task = await _context.EvaluationTasks.Include(t => t.EvaluationSubTasks).FirstOrDefaultAsync(t => t.evaluationTaskID == dto.id);
            _context.EvaluationTasks.Remove(task);
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
            var sub = await _context.EvaluationSubTasks.FindAsync(dto.id);
            _context.EvaluationSubTasks.Remove(sub);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostSaveValuation([FromBody] CreateValauationDto dto)
        {
            var existing = await _context.EvaluationValuations
                .FirstOrDefaultAsync(v => v.evaluationID == dto.evaluationID && v.evaluationSubTaskID == dto.evaluationSubTaskID);

            if (existing != null)
            {
                existing.timeValuation = dto.timeValuation;
                existing.resourceValuation = dto.resourceValuation;
                existing.performanceValuation = dto.performanceValuation;
                existing.modifiedDate = DateTime.Now;
            }
            else
            {
                _context.EvaluationValuations.Add(new evaluationValuationModel
                {
                    evaluationID = dto.evaluationID,
                    evaluationSubTaskID = dto.evaluationSubTaskID,
                    timeValuation = dto.timeValuation,
                    resourceValuation = dto.resourceValuation,
                    performanceValuation = dto.performanceValuation,
                    modifiedBy = User.Identity.Name
                });
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<JsonResult> OnPostPostEvaluation([FromBody] IdDto dto)
        {
            var eval = await _context.Evaluations.FindAsync(dto.id);
            eval.evaluationStatus = evaluationStatus.Submitted;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        // DTOs
        public class HeaderDto { public int id { get; set; } public string evaluationName { get; set; } public DateTime evaluationStartDate { get; set; } public DateTime evaluationEndDate { get; set; } }
        public class CreateTaskDto { public string evaluationTaskName { get; set; } public int evaluationTypeID { get; set; } public decimal evaluationTaskWeight { get; set; } }
        public class CreateSubTaskDto { public string evaluationSubTaskName { get; set; } public int evaluationTaskID { get; set; } public decimal evaluationSubTaskWeight { get; set; } }
        public class CreateValauationDto { public int evaluationID { get; set; } public int evaluationSubTaskID { get; set; } public decimal timeValuation { get; set; } public decimal resourceValuation { get; set; } public decimal performanceValuation { get; set; } }
        public class UpdateDto { public int id { get; set; } public string name { get; set; } public decimal weight { get; set; } }
        public class IdDto { public int id { get; set; } }
    }
}


