using PIS2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PIS2.Models
{
    public class jobPlacementModel
    {
        [Key]
        [Display(Name ="Placement ID")]
        public int jobPlacementID { get; set; }
        [Display(Name = "Employee")]
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        [Display(Name = "Department")]
        public int departmentID { get; set; }
        public virtual departmentModel? departmentModel { get; set; }
        [Display(Name = "Position")]
        public int jobID { get; set; }
        public jobModel? jobModel { get; set; }
        
        [Required]
        [Display(Name = "Salary")]
        public decimal jobPlacementSalary { get; set; }
        [Display(Name = "Step")]
        public int jobStepID { get; set; }
        public virtual jobStepModel? jobStepModel { get; set; }
        [Display(Name = "Date")]
        public DateTime jobPlacementDate { get; set; } = DateTime.Now;
        [Display(Name = "Status")]
        public mainStatus jobPlacementStatus { get; set; }
        [Display(Name = "Reference")]
        public string jobPlacementReference { get; set; }
        [Display(Name = "Reason")]
        public string jobPlacementReason { get; set; }
        [Display(Name = "Change Effective Date")]
        public DateTime? ChangeEffDate { get; set; }
        [Display(Name = "Career Path")]
        public JobCareer jobPlacementCareer { get; set; }
        public virtual ICollection<jobPlacementHistoryModel>? JobPlacementHistories { get; set; }
        public virtual ICollection<evaluationModel>? Evaluations { get; set; }
        public virtual ICollection<payrollPay>? PayrollPays { get; set; }
        public string modifiedBy { get; set; }
        public jobPlacementModel() { }
        public decimal getJobRate()
        {
            decimal hourlyRate = 0;
            hourlyRate =(decimal) Math.Round(jobPlacementSalary / 208, 2);

            return hourlyRate;
        }
    }
    public class jobPlacementHistoryModel
    {
        [Key]
        public int jobPlacementHistoryID { get; set; }
        public int jobPlacementID { get; set; }
        public virtual jobPlacementModel? jobPlacementModel { get; set; }
        public int? jobStepID { get; set; }
        public virtual jobStepModel? jobStepModel { get; set; }
        public int departmentID { get; set; }
        public virtual departmentModel? departmentModel { get; set; }
        public string? jobPlacementReference { get; set; }
        public decimal jobPlacementSalary { get; set; }
        public mainStatus jobPlacementStatus { get; set; }
        public string jobPlacementReason { get; set; }
        public DateTime jobPlacementDate { get; set; }
        [Display(Name = "Career Path")]
        public JobCareer jobPlacementCareer { get; set; }
        [Display(Name = "Change Effective Date")]
        public DateTime? ChangeEffDate { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public jobPlacementHistoryModel() { }
    }

    
}
