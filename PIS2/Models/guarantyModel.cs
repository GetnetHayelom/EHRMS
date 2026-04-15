using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class guarantyModel
    {
        [Key]
        public int guarantyID { get; set; }
        [Display(Name ="Employment ID")]
        public int employmentID { get; set; }
        public virtual employmentModel? Employment { get; set; }
        [Display(Name = "Start Date")]
        public DateTime? guarantyStartDate { get; set; }= DateTime.Now;
        [Display(Name = "End Date")]
        public DateTime? guarantyEndDate { get; set; }
        [Display(Name = "Value of Risk")]
        public double? guarantyAmount { get; set; }
        [Display(Name = "Guaranty Type")]
        public string guarantyType { get; set; }
        [Display(Name = "Guarantee Name")]
        public string guarantyFor { get; set; }
        [Display(Name = "Issued To (Beneficiary)")]
        public string guarantyBeneficiary { get; set; }
        [Display(Name = "Status")]
        public mainStatus guarantyStatus { get; set; }
        public string modifiedBy { get; set; }
        [Display(Name = "Associated Request")]
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
        [Display(Name = "Internal Loan")]
        Internal_Loan,
        [Display(Name = "External Loan")]
        External_Loan,
        [Display(Name = "Employment")]
        Employment,
        [Display(Name = "Collateral")]
        Collateral,
        [Display(Name = "Other")]
        Other
    }
}
