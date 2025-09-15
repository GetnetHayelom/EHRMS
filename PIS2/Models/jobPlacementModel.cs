using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PIS2.Models
{
    public class jobPlacementModel
    {
        [Key]
        public int jobPlacementID { get; set; }      
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }      
        public int departmentID { get; set; }
        public virtual departmentModel? departmentModel { get; set; }        
        public int jobID { get; set; }
        public jobModel? jobModel { get; set; }
        
        [Required]
        public decimal jobPlacementSalary { get; set; }
        public int jobStepID { get; set; }
        public virtual jobStepModel? jobStepModel { get; set; }
        public DateTime jobPlacementDate { get; set; } = DateTime.Now;
        public mainStatus jobPlacementStatus { get; set; }
        public string jobPlacementReference { get; set; }
        public string jobPlacementReason { get; set; }
        public virtual ICollection<jobPlacementHistoryModel>? JobPlacementHistories { get; set; }

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

        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public jobPlacementHistoryModel() { }
    }

}
