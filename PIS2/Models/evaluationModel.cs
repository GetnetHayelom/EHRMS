using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace PIS2.Models
{
    public class evaluationModel
    {
        [Key]
        public int evaluationID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public string? evaluationName { get; set; }
        public DateTime evaluationStartDate { get; set; }
        public DateTime evaluationEndDate { get; set; }
        public evaluationStatus evaluationStatus { get; set; }
        public List<string>? evaluationTypes { get; set; }
        public int? jobPlacementID { get; set; }
        public virtual jobPlacementModel? JobPlacementModel { get; set; }
        public string modifiedBy { get; set; } = "";
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public ICollection<evaluationValuationModel>? EvaluationValuations { get; set; }
        
    }
    public class evaluationTypeModel
    {
        [Key]
        public int evaluationTypeID { get; set; }
        public string evaluationTypeName { get; set; } //HR, Management
        public decimal evaluationTypeWeight { get; set; } //30%, 70%
        public mainStatus evaluationTypeStatus { get; set; }
        public bool isFixed { get; set; }
        public string? Remark { get; set; }
        public int? jobClassID { get; set; }
        public virtual jobClassModel? jobClassModel { get; set; }
        public EmploymentPositions? Position { get; set; }
        public string modifiedBy { get; set; } = "";
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public ICollection<evaluationTaskModel>? EvaluationTasks { get; set; }
    }
    public class evaluationTaskModel
    {
        [Key]
        public int evaluationTaskID { get; set; }
        public string evaluationTaskDescription { get; set; } = "";
        public int evaluationTypeID { get; set; }
        public virtual evaluationTypeModel? EvaluationTypeModel { get; set; }
        public string evaluationTaskName { get; set; }
        public decimal evaluationTaskWeight { get; set; }
        public bool IsLocked { get; set; } = false;
        public string modifiedBy { get; set; } = "";
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public ICollection<evaluationSubTaskModel>? EvaluationSubTasks { get; set; }
    }
    public class evaluationSubTaskModel
    {
        [Key]
        public int evaluationSubTaskID { get; set; }
        public string evaluationSubTaskName {get; set;}
        public string? evaluationSubTaskDescription { get; set; } = "";
        public int evaluationTaskID { get; set; }
        public virtual evaluationTaskModel? EvaluationTaskModel { get; set; }
        public decimal evaluationSubTaskWeight { get; set; }
        public string modifiedBy { get; set; } = "";
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public ICollection<evaluationValuationModel>? EvaluationValuations { get; set; }
    }
    public class  evaluationValuationModel
    {
        [Key]
        public int evaluationValuationID { get; set; }
        public int evaluationID { get; set; }
        public virtual evaluationModel? EvaluationModel { get; set; }
        public int evaluationSubTaskID { get; set; }
        public virtual evaluationSubTaskModel? EvaluationSubTaskModel { get; set; }
        public decimal timeValuation { get; set; }
        public decimal resourceValuation { get; set; }
        public decimal performanceValuation { get; set; }  
        public decimal totalValuation { get { return timeValuation + resourceValuation + performanceValuation; } }
        public string modifiedBy { get; set; } = "";
        public DateTime modifiedDate { get; set; } = DateTime.Now;

    }


    public enum evaluationStatus 
    {
        Pending,
        Submitted,
        Void
    }
    public enum ProficiencyLevel
    {
        [Display(Name = "Exceptional (4.0)")]
        Exceptional = 40,

        [Display(Name = "Very Good (3.5)")]
        VeryGood = 35,

        [Display(Name = "Good (3.0)")]
        Good = 30,

        [Display(Name = "Average (2.5)")]
        Average = 25,

        [Display(Name = "Unsatisfactory (2.0)")]
        Unsatisfactory = 20,

        [Display(Name = "Poor (1.5)")]
        Poor = 15,

        [Display(Name = "Very Poor (1.0)")]
        VeryPoor = 10,

        [Display(Name = "Unacceptable (0.5)")]
        Unacceptable = 5,

        [Display(Name = "No Output / Zero (0.0)")]
        Zero = 0
    }

}
