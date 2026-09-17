using System.ComponentModel.DataAnnotations;

namespace PIS2.Models.HR
{
    public class shiftAssignmentModel
    {
        [Key]
        public int shiftAssignmentID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public int shiftID {  get; set; }
        public virtual shiftModel? shiftModel { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }

        public shiftAssignmentModel() { }
    }
}
