using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class leaveTypeModel
    {
        [Key]
        public int leaveTypeID { get; set; }
        public string leaveTypeName { get; set; }      
        public leaveTypeImpact leaveTypeImpact { get; set; }
        public mainStatus leaveTypeStatus { get; set; }
        public virtual ICollection<leaveModel>? Leaves { get; set; }
        public virtual leaveGroup leaveGroup { get; set; }
        public string modifiedBy { get; set; }
        public string leaveAvailability { get; set; }
        public bool leaveJob { get; set; }
        public bool leaveLegality { get; set; }
        public leaveTypeModel() { }
    }
    public enum leaveGroup
    {
        AnnualLeave,
        Absentism,
        AllowedLeave
    }

    
}

