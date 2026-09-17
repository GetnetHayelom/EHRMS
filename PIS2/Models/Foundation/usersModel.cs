using PIS2.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PIS2.Models.Foundation
{
    public class userModel: IdentityUser<int>
    {
        // Override base Id property so base Identity methods and your codebase both use 'userID'
        [Key]
        public override int Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        public int? personID { get; set; }
        public virtual personModel? personModel { get; set; }
        public mainStatus userStatus { get; set; } = mainStatus.Active;
        public string modifiedBy { get; set; } = "System";
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public bool MustChangePassword { get; set; } = true;
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
        public int Id { get; set; }
        public virtual userModel userModel { get; set; }
        public mainStatus userStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public userHistoryModel() { }
    }
}
