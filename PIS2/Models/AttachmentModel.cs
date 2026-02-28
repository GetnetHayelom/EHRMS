using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class AttachmentModel
    {
        [Key]
        public int AttachmentID { get; set; }

        [Required]
        public string TableName { get; set; }   // e.g. "Employees", "Incidents", "LeaveRequests"

        [Required]
        public int RecordId { get; set; }       // ID from that table

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }    // network path

        public string FileType { get; set; }
        public long FileSize { get; set; }

        public string UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.Now;
    }

}
