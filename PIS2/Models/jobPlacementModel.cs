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
        public int? shiftID { get; set; }
        public virtual shiftModel? shiftModel { get; set; }
        public int? workSiteID { get; set; }
        public virtual workSiteModel? workSiteModel { get; set; }
        [Required]
        public decimal jobPlacementSalary { get; set; }
        public int jobStepID { get; set; }
        public virtual jobStepModel? jobStepModel { get; set; }
        public DateTime jobPlacementDate { get; set; } = DateTime.Now;
        public mainStatus jobPlacementStatus { get; set; }
        public string jobPlacementReference { get; set; }
        public string jobPlacementReason { get; set; }
        public virtual ICollection<jobPlacementHistoryModel> JobPlacementHistories { get; set; }

        public string modifiedBy { get; set; }
        public jobPlacementModel() { }
        public double getJobRate()
        {
            double hourlyRate = 0;
            hourlyRate =Math.Round( (double) jobPlacementSalary / 208, 2);

            return hourlyRate;
        }
    }
    public class jobPlacementHistoryModel
    {
        [Key]
        public int jobPlacementHistoryID { get; }
        public int jobPlacementID { get; }
        public virtual jobPlacementModel? jobPlacementModel { get; }
        public int? jobStepID { get; set; }
        public virtual jobStepModel? jobStepModel { get; }
        public int departmentID { get; }
        public virtual departmentModel? departmentModel { get; }
        public string? jobPlacementReference { get; }
        public double jobPlacementSalary { get; }
        public mainStatus jobPlacementStatus { get; }
        public string jobPlacementReason { get; }

        public string modifiedBy { get; }
        public DateTime modifiedDate { get; }

        public jobPlacementHistoryModel() { }
    }

}
