using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class companyModel
    {
        

        [Key]
        public int companyID { get; set; }
        [Required(ErrorMessage = "Company name can not be empty.")]
        public string companyName { get; set; }
        public string? companyAlias { get; set; }
        public int? employmentID { get; set; } //Company Manager
        public virtual employmentModel? employmentModel { get; set; }
        public int? addressID { get; set; }
        public virtual addressModel? addressModel { get; set; } = null!;
        public mainStatus companyStatus { get; set; }
        public virtual ICollection<departmentModel>? Departments { get; set; } = null!;
        public virtual ICollection<businessUnitModel>? BusinessUnits { get; set; } = null!;
        public virtual ICollection<accessModel>? Accesses { get; set; } = null!;
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public companyModel()
        {

        }
    }
}
