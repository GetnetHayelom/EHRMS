using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class allowanceAssignmentModel
    {
        [Key]
        public int allowanceAssignmentID { get; set; }
        [Required]
        public int allowanceID { get; set; }   
        public virtual allowanceModel? allowanceModel { get; set; }
        public DateTime? allowanceAssignmentDate { get; set; } = DateTime.Now;
        [Required]
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public double allowanceAssignmentAmount { get; set; } = 0;
        public mainStatus allowanceStatus { get; set; } = mainStatus.Active;
        public virtual ICollection<allowanceAssignmentHistoryModel>? AllowanceAssignmentHistories { get; set; }
        public string modifiedBy { get; set; }
        public allowanceAssignmentModel() { }
    }
    public class allowanceAssignmentHistoryModel
    {
        [Key]
        public int allowanceAssignmentHistoryID { get; set; }
        public int allowanceAssignmentID { get; set; }
        public virtual allowanceAssignmentModel? allowanceAssignmentModel { get; set; }
        public DateTime modifiedDate { get; set;} = DateTime.Now;
        public mainStatus allowanceAssignmentHistoryStatus { get; set; }
        public string modifiedBy { get; set; }
       
        public allowanceAssignmentHistoryModel()
        {
        }

    }
}
