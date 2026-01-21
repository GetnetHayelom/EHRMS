using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{


    public class loyaltyHistoryModel
    {
        [Key] public int loyaltyHistoryID { get; private set; }
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public int loyaltyID { get; set; }
        public virtual loyaltyModel? loyaltyModel { get; set; }  
        public double loyaltyAmount { get; set; }
        public loyaltyStatus loyaltyHistoryStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }= DateTime.Now;
        public loyaltyHistoryModel() { }
    }
    public enum loyaltyStatus
    {
        Approved,
        Hold,
        Posted,
        Completed,
        Declined
    }
}