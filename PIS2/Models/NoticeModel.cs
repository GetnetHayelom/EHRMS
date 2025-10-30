using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class NoticeModel
    {
        [Key]
        public int noticeID { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Notice Title")]
        public string noticeTitle { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Notice Sub-Title")]
        public string? noticeSubTitle { get; set; }

        [Required]
        [Display(Name = "Notice Content")]
        public string noticeContent { get; set; }

        [Display(Name = "Posted By")]
        [StringLength(100)]
        public string noticePostedBy { get; set; }

        [Display(Name = "Approved By")]
        [StringLength(100)]
        public string noticeApprovedBy { get; set; }

        [Display(Name = "Notice From")]
        [StringLength(100)]
        public string? noticeFrom { get; set; }
        [Display(Name = "Notice Priority")]
        public Priority noticePriority { get; set; }
        [Display(Name = "Date Posted")]
        [DataType(DataType.Date)]
        public DateTime DatePosted { get; set; } = DateTime.Now;

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Attachment (Optional)")]
        public string? AttachmentPath { get; set; }

        public NoticeModel() { }
    }

    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }
}
