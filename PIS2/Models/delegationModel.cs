using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class delegationModel
    {
        [Key]
        public int delegationID { get; set; }
        public int delegationFrom { get; set; }
        public virtual employmentModel? FromEmployment { get; set; }
        public int delegationTo { get; set; }
        public virtual employmentModel? ToEmployment { get; set; }
        public DateTime delegationStartDate { get; set; }
        public DateTime delegationEndDate { get; set; }
        public delegationScopes delegationScope { get; set; }
        public mainStatus delegationStatus { get; set; }
        public DateTime? modifiedDate { get; set; } = DateTime.Now;
        public string modifiedBy { get; set; }
        public virtual List<delegationHistoryModel>? DelegationHistories { get; set; }

        public delegationModel() { }
    }

    public class delegationHistoryModel
    {
        [Key]
        public int delegationHistoryID { get; set; }
        public int delegationID { get; set; }
        public virtual delegationModel? delegationModel { get; set; }
        public DateTime delegationStartDate { get; set; }
        public DateTime delegationEndDate { get; set; }
        public mainStatus delegationStatus { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }

        public delegationHistoryModel() { }
    }
    public enum delegationScopes
    {
        [Display(Name = "Leave Approve")]
        LeaveApprove,
        [Display(Name = "Overtime Approve")]
        OTApprove,
        [Display(Name = "Shift Assignment")]
        ShiftAssignment,
        [Display(Name = "Worksite Assignment")]
        WorkSiteAssignment,
        [Display(Name = "View Only")]
        ViewOnly,
        [Display(Name = "Full Delegation")]
        Fulldelegation
    }
}
