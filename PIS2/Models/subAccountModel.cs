using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class subAccountModel
    {
        [Key]
        public int subAccountID { get; set; }
        [Required]
        public int accountID { get; set; }
        public virtual accountModel? accountModel { get; set; }
        [Required]
        public string subAccountName { get; set; }
        [Required]
        public string? subAccountDescription { get; set; }
        [Required]
        public mainStatus subAccountStatus { get; set; }
        public ICollection<departmentModel>? Departments { get; set; } = null!;
        public subAccountModel() { }
        public subAccountModel(int accountID, string subAccountName, string subAccountDescription, mainStatus status) {
            this.accountID = accountID;
            this.subAccountName = subAccountName;
            this.subAccountDescription = subAccountDescription;
            this.subAccountStatus = status;
        }
        public string modifiedBy { get; set; }
    }
}
