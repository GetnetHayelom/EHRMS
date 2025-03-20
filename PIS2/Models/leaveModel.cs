using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class leaveModel
    {
        [Key]
        public int leaveID { get; set; }  
        [Required]
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        [Required]
        public DateTime leaveReaquestDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime leaveStartDate { get; set; }=DateTime.Now;
        [Required]
        public DateTime leaveEndDate { get; set; } = DateTime.Now;
        [Required]
        
        public double leaveDays { get; set; }     
        public int leaveTypeID { get; set; }
        public virtual leaveTypeModel? leaveTypeModel { get; set; }
        public leaveStatus leaveStatus { get; set; } = leaveStatus.Hold;
        public virtual ICollection<leaveHistoryModel>? LeaveHistories { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (leaveEndDate < leaveStartDate)
            {
                yield return new ValidationResult(
                    "Leave end date must be on or after the leave start date.",
                    new[] { nameof(leaveEndDate) });
            }

            var maxDays = (leaveEndDate - leaveStartDate).TotalDays;

            if (leaveDays < 0.5 || leaveDays > maxDays)
            {
                yield return new ValidationResult(
                    $"Leave days must be between 0.5 and {maxDays}.",
                    new[] { nameof(leaveDays) });
            }
        }
        public string modifiedBy { get; set; }
        public leaveModel() { }
       
    }
    public enum leaveStatus
    {
        Hold,
        Approved,
        Posted,
        Declined,
        Comleted

    }
    public class leaveHistoryModel
    {
        [Key] public int leaveHistoryID { get; set; }
        public int leaveID { get; set; }
        public virtual leaveModel leaveModel { get; set; }
        public leaveStatus leaveHistoryAction { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy {get; set;}
        
        public leaveHistoryModel()
        {

        }
    }
}
