using PIS2.Enums;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class jobStepModel
    {
        [Key]
        public int jobStepID { get; set; }
        public string jobStepName { get; set; }
        public int jobStepNumber { get; set; }
        public int jobGradeID { get; set; }
        public virtual jobGradeModel? jobGradeModel { get; set; }
        public double jobStepSalary { get; set; }
        public mainStatus jobStepStatus { get; set; }
        public string modifiedBy {get; set;}
        public virtual ICollection<jobStepHistoryModel>? JobStepHistories { get; set; }
        public virtual ICollection<jobPlacementModel>? JobPlacements { get; set; }
        public virtual ICollection<jobPlacementHistoryModel>? JobPlacementHistories { get; set; }
        public jobStepModel() { }
    }

    public class jobStepHistoryModel
    {
        [Key]
        public int jobStepHistoryID { get; set; }
        public int jobStepID { get; set; }
        public string jobStepName { get; set; }

        public virtual jobStepModel? jobStepModel { get; set; }
        public double jobStepSalary { get; set; }
        public mainStatus jobStepStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public jobStepHistoryModel() { }
    }

}
