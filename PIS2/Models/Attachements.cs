using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class Attachements
    {
        [Key]
        public int attachmentID { get; set; }

        [Required]
        public string fileName { get; set; }

        [Required]
        public string filePath { get; set; }

        public string fileType { get; set; }   // pdf, jpg, png
        public long fileSize { get; set; }

        public string uploadedBy { get; set; }
        public DateTime uploadedDate { get; set; } = DateTime.Now;

    }
}
