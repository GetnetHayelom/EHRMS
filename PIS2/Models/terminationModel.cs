using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class terminationModel
    {
        [Key] public int terminationID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public DateTime terminationDate { get; set; }
        public string terminationReason { get; set; }
        public string? terminationRemark { get; set; }
        public terminationStatus terminationStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public terminationModel()
        {

        }
    }
    public enum terminationStatus
    {
        Hold,
        Approved,
        Posted,
        Complete
    }
}
