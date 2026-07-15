using PIS2.Enums;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class loyaltyModel
    {
        [Key, Required]
        public int loyaltyID { get; set; }
        [Required]
        public string loyaltyName { get; set; }
        public double loyaltyAmount { get; set; }
        public double loyaltyCounter { get; set; }
        public mainStatus loyaltyStatus { get; set; }
        public virtual ICollection<loyaltyHistoryModel>? LoyaltyHistories { get; set; }
        public string modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; } = DateTime.Now;
        public loyaltyModel() { }
    }
}
