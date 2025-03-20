using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class departmentModel
    {
        [Key]
        public int departmentID { get; set; }
        public string? departmentShort { get; set; }
        [Required(ErrorMessage = "Department name can not be empty.")]
        public string departmentName { get; set; }
        public mainStatus departmentStatus { get; set; }      
        public int? subAccountID { get; set; }
        public virtual subAccountModel? subAccountModel { get; set; } = null!;
        public int companyID { get; set; }
        public virtual companyModel? companyModel { get; set; } = null!;
        //Manager
        public int? employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public ICollection<jobPlacementModel>? JobPlacements { get; set; } = null!;
        public string modifiedBy { get; set; }
        public departmentModel() { }
    }
}
