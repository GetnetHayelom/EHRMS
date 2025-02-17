using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PIS2.Models
{
    public class jobPlacementModel
    {
        [Key]
        public int jobPlacementID { get; set; }      
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }      
        public int departmentID { get; set; }
        public virtual departmentModel? departmentModel { get; set; }        
        public int jobID { get; set; }
        public jobModel? jobModel { get; set; }
        public int? shiftID { get; set; }
        public virtual shiftModel? shiftModel { get; set; }
        [Required]
        public decimal jobPlacementSalary { get; set; }
        public DateTime jobPlacementDate { get; set; } = DateTime.Now;
        public mainStatus jobPlacementStatus { get; set; }
        public string jobPlacementReference { get; set; }
        public string jobPlacementReason { get; set; }

        public jobPlacementModel() { }
    }
}
