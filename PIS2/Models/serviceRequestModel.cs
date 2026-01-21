using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class serviceRequestModel
    {
        [Key]
        public int serviceRequestID { get; set; }
        [Required]
        public int employmentID {  get; set; }
        public virtual employmentModel? Employment { get; set; }

        public DateTime serviceRequestDate { get; set; } = DateTime.Now;
        public ServiceRequestTypes requestedService { get; set; }

        public ServiceRequestStatus serviceRequestStatus { get;set; }
        public virtual ICollection<serviceRequestHistoryModel>? ServiceRequestHistoies { get; set; }
        public string modifiedBy { get; set; }

        public serviceRequestModel() { }
    }
    public class serviceRequestHistoryModel
    {
        [Key]
        public int serviceRequestHistoryID { get; set; }
        public mainStatus serviceRequestStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; }
        public int serviceRequestID { get; set; }
        public virtual serviceRequestModel? ServiceRequest { get; set; }
        public serviceRequestHistoryModel() { }
    }
    public enum ServiceRequestTypes
    {
        Experience,
        Guaranty,
        Termination
    }
    public enum ServiceRequestStatus
    {
        Hold,
        Reviewed,
        Completed
    }
    
}
