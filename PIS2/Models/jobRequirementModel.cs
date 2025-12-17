using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class jobRequirementModel
    {
        [Key]
        public int jobRequirementID { get; set; }
        [Required]
        public int departmentID { get; set; }
        public virtual departmentModel? DepartmentModel { get; set; }
        [Required]
        public int jobID { get; set; }
        public virtual jobModel? JobModel { get; set; }
        [Required]
        public int requiredNumber { get; set; }
        public int? approvedNumber { get; set; }
        public int? hiredNumber { get; set; }
        public jobReqStatus jobRequirementStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public virtual ICollection<jobRequirementHistoryModel>? JobRequirementHistories { get; set; }
        public virtual ICollection<jobReqCost>? JobReqCosts { get; set; }
        public jobRequirementModel() { }

    }
    public class jobRequirementHistoryModel
    {
        [Key]
        public int jobRequirementHistoryID { get; set; }
        [Required]
        public int jobRequirementID { get; set; }
        public virtual jobRequirementModel? JobRequirementModel { get; set; }
        [Required]
        public int requiredNumber { get; set; }
        public jobReqStatus jobRequirementStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public jobRequirementHistoryModel() { }

    }

    public class jobReqCost
    {
        [Key]
        public int jobReqCostID { get; set; }
        public int jobRequirementID { get; set; }
        public virtual jobRequirementModel? JobRequirementModel { get; set; }
        public jobReqStatus jobReqStatus { get; set; }
        public string jobReqCostReason { get; set; }
        public double jobReqCostEstimate { get; set; }
        public double jobReqCostActual { get; set; }
        public double jobReqCostReference { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public jobReqCost() { }

    }
    public enum jobReqStatus
    {
        Hold,
        Approved,
        Staged,
        Published,
        Recruited,
        Screened,
        Exam,
        Interview,
        Completed,
        Declined,
        Failed
    }
}

