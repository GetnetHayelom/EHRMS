using Microsoft.EntityFrameworkCore;
using PIS2.Enums;
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
        [Precision(18, 2)]
        public decimal allowanceAssignmentAmount { get; set; } = 0;
        public DateTime? allowanceAssignmentEndDate { get; set; }
        public mainStatus allowanceStatus { get; set; } = mainStatus.Active;
        public virtual ICollection<allowanceAssignmentHistoryModel>? AllowanceAssignmentHistories { get; set; }
        public string modifiedBy { get; set; }
        public allowanceAssignmentModel() { }
    }
    public class allowanceAssignmentHistoryModel
    {
        [Key]
        public int allowanceAssignmentHistoryID { get; set; }
        public int allowanceAssignmentID { get;}
        public virtual allowanceAssignmentModel? allowanceAssignmentModel { get; }
        public DateTime modifiedDate { get;} 
        public mainStatus allowanceAssignmentHistoryStatus { get; }
        [Precision(18, 2)]
        public decimal allowanceAssignmentAmount { get;}
        public DateTime? allowanceAssignmentEndDate { get;  }
        public string modifiedBy { get; }
       
        public allowanceAssignmentHistoryModel()
        {
        }

    }
}
