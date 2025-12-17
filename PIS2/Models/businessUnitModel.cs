using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class businessUnitModel
    {

        [Key]
        public int businessUnitID { get; set; }
        [Required(ErrorMessage = "Name can not be empty.")]
        public string businessUnitName { get; set; }
        public int companyID { get; set; }
        public virtual companyModel? companyModel { get; set; }
        public string? businessUnitAlias { get; set; }
        public int? employmentID { get; set; } //Company Manager
        public virtual employmentModel? employmentModel { get; set; }
        public int? addressID { get; set; }
        public virtual addressModel? addressModel { get; set; } = null!;
        public mainStatus businessUnitStatus { get; set; } = mainStatus.Suspended;
        public virtual ICollection<departmentModel>? Departments { get; set; } = null!;
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public businessUnitModel() { }

    }
    
}
