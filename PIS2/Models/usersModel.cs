using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class userModel
    {
        [Key]
        public int userID { get; set; }
        [Required]
        public string userName { get; set; }
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; }
        public mainStatus userStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public virtual ICollection<userHistoryModel>? UserHistories { get; set; }
        public virtual ICollection<accessModel>? Accesses { get; set; }
         public userModel() { }
    }
    public class userHistoryModel
    {
        [Key]
        public int userHistoryID { get; set; }
        [Required]
        public string userName { get; set; }
        public int userID { get; set; }
        public virtual userModel userModel { get; set; }
        public mainStatus userStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public userHistoryModel() { }
    }
}
