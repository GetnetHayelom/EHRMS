using PIS2.Enums;
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
        public int serviceRequestTypeID { get; set; }
        public virtual serviceRequestTypeModel? ServiceRequestType { get; set; }

        public ServiceRequestStatus serviceRequestStatus { get;set; }
        public virtual ICollection<serviceRequestHistoryModel>? ServiceRequestHistoies { get; set; }
        public string modifiedBy { get; set; }

        public serviceRequestModel() { }
    }
    public class serviceRequestHistoryModel
    {
        [Key]
        public int serviceRequestHistoryID { get; set; }
        public ServiceRequestStatus serviceRequestStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; }
        public int serviceRequestID { get; set; }
        public virtual serviceRequestModel? ServiceRequest { get; set; }
        public serviceRequestHistoryModel() { }
    }

    public class serviceRequestTypeModel
    {
        [Key]
        public int serviceRequestTypeID { get; set; }
        [Required]
        [Display(Name ="Service Request Name")]
        public string serviceRequestTypeName { get; set; }
        public mainStatus serviceRequestTypeStatus { get; set; }
        public string? serviceRequestTypeDescription { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public virtual ICollection<serviceRequestModel>? ServiceRequests { get; set; }

        public serviceRequestTypeModel() { }
    }
    
    
    
}
