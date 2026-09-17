using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PIS2.Enums;

namespace PIS2.Models.Foundation
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
        [Column(TypeName = "nvarchar(max)")]
        public string noticeContent { get; set; }

        [Display(Name = "Notice Status")]
        public NoticeStatus noticeStatus { get; set; }

        [Display(Name = "Notice From")]
        [StringLength(100)]
        public string? noticeFrom { get; set; }
        [Display(Name = "Notice To")]
        [StringLength(100)]
        public string? noticeTo { get; set; }
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

        [Display(Name = "Signed By")]
        public string? noticeSignedBy { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public NoticeModel() { }
    }

    
}
