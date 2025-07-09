using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class deligationModel
    {
        [Key]
        public int deligationID { get; set; }
        public int deligationFrom { get; set; }
        public virtual employmentModel? FromEmployment { get; set; }
        public int deligationTo { get; set; }
        public virtual employmentModel? ToEmployment { get; set; }
        public DateTime deligationStartDate { get; set; }
        public DateTime deligationEndDate { get; set; }
        public deligationScopes deligationScope { get; set; }
        public mainStatus deligationStatus { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }

        public deligationModel() { }
    }


    public enum deligationScopes
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
        [Display(Name = "WorksiteAssignment")]
        FullDeligation
    }
}
