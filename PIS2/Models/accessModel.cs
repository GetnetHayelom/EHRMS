using Microsoft.AspNetCore.Identity;
using PIS2.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Microsoft.AspNetCore.Identity;

namespace PIS2.Models
{
    public class accessModel
    {
        [Key]
        public int accessID { get; set; }

        // Identity user
        public int userID { get; set; }

        public virtual userModel? userModel { get; set; }

        // Identity role
        public int roleID { get; set; }

        public virtual IdentityRole<int>? Role { get; set; }

        // Company scope
        public int? companyID { get; set; }

        public virtual companyModel? CompanyModel { get; set; }

        // Access status
        public mainStatus accessStatus { get; set; }
            = mainStatus.Active;

        public string modifiedBy { get; set; } = "System";

        public DateTime modifiedDate { get; set; }
            = DateTime.Now;

        public virtual List<accessHistoryModel> AccessHistories
        { get; set; }
            = new List<accessHistoryModel>();

        public accessModel()
        {
        }
    }

    public class accessHistoryModel{
        [Key]
        public int accessHistoryID { get; set; }
        public int accessID { get; set; }
        public virtual accessModel? accessModel { get; set; }
        public mainStatus accessStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public accessHistoryModel() { }
    }

   
    

}
