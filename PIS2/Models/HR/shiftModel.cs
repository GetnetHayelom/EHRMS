using PIS2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models.HR
{
    public class shiftModel
    {
        [Key]
        public int shiftID { get; set; }
        [Required]
        public string shiftName { get; set; }
        public TimeSpan shiftStart { get; set; }
        public TimeSpan shiftEnd { get; set; }
        public int? maxLateIn { get; set; }
        public int? maxEarlyOut { get; set; }
        public int? maxEarlyIn { get; set; }
        public int? maxLateOut { get; set; }
        public mainStatus shiftStatus { get; set; }
        public virtual ICollection<breakModel>? Breaks { get; set; }
        public virtual ICollection<shiftAssignmentModel>? ShiftAssignments { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public shiftModel() { }
    }
}
