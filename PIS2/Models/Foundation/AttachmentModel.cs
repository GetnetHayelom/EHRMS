using System.ComponentModel.DataAnnotations;

namespace PIS2.Models.Foundation
{
    public class AttachmentModel
    {
        [Key]
        [Display(Name ="Attachment ID")]
        public int AttachmentID { get; set; }

        [Required]
        [Display(Name = "Related Record")]
        public string TableName { get; set; }   // e.g. "Employees", "Incidents", "LeaveRequests"

        [Required]
        [Display(Name = "related Record ID")]
        public int RecordId { get; set; }       // ID from that table

        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "File Path")]
        public string FilePath { get; set; }    // network path
        [Display(Name = "File Type")]
        public string FileType { get; set; }
        [Display(Name = "File Size")]
        public long FileSize { get; set; }

        [Display(Name = "Uploaded By")]
        public string UploadedBy { get; set; }

        [Display(Name = "Date of Upload")]
        public DateTime UploadedDate { get; set; } = DateTime.Now;
    }

}
