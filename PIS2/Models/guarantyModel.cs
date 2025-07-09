using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class guarantyModel
    {
        [Key]
        public int guarantyID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? Employment { get; set; }
        public DateTime? guarantyStartDate { get; set; }= DateTime.Now;
        public DateTime? guarantyEndDate { get; set; }
        public double? guarantyAmount { get; set; }
        public string guarantyType { get; set; }
        public string guarantyFor { get; set; }
        public string guarantyBeneficiary { get; set; }
        public mainStatus guarantyStatus { get; set; }
        public string modifiedBy { get; set; }
        public int? serviceRequestID { get; set; }
        public virtual serviceRequestModel? serviceRequestModel { get; set; }
        
        public virtual ICollection<guarantyHistoryModel>? GuarantyHistories {  get; set; }
        public guarantyModel() { }
    }
    public class guarantyHistoryModel
    {
        [Key]
        public int guarantyHistoryID { get; set; }
        public int guarantyID { get; set; }
        public virtual guarantyModel guarantyModel{get;set;}
        public mainStatus guarantyStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; }
        public guarantyHistoryModel() { }
    }
     public enum GuarantyTypes{
        Internal_Loan,
        External_Loan,
        Employment,
        Collateral,
        Other
    }
}
