using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class accountModel
    {
        [Key]
        public int accountID { get; set; }
        [Required]
        public string accountNumber {  get; set; }
        
        public string accountName { get; set; }
        public string? accountDescription { get; set; }
        public mainStatus accountStatus { get; set; }
        //Navigation Property
        public virtual ICollection<subAccountModel>? SubAccounts { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public accountModel() { }
        public accountModel(string accountNumber, string accountName, string accountDescription, mainStatus accountStatus)
        {
            this.accountNumber = accountNumber;
            this.accountName = accountName;
            this.accountDescription = accountDescription;
            this.accountStatus = accountStatus;
        }

    }
}
