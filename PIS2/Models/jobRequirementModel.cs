using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class jobRequirementModel
    {
        [Key]
        public int jobRequirementID { get; set; }
        [Required]
        public int departmentID { get; set; }
        public virtual departmentModel DepartmentModel { get; set; }
        [Required]
        public int jobID { get; set; }
        public virtual jobModel JobModel { get; set; }
        [Required]
        public int requiredNumber { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }= DateTime.Now;
        public jobRequirementModel() { }

    }
}
