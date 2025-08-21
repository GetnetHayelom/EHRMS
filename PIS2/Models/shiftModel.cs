using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class shiftModel
    {
        [Key]
        public int shiftID { get; set; }
        [Required]
        public string shiftName { get; set; }
        public TimeSpan shiftStart { get; set; }
        public TimeSpan shiftEnd { get; set; }
        public mainStatus shiftStatus { get; set; }
        public virtual ICollection<breakModel>? Breaks { get; set; }
        public virtual ICollection<shiftAssignmentModel>? ShiftAssignments { get; set; }
        public string modifiedBy { get; set; }
        public shiftModel() { }
    }
}
