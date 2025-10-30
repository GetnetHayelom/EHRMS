using System.ComponentModel;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class accessModel
    {
        [Key]
        public int accessID { get; set; }
        public int userID { get; set; }
        public virtual userModel? userModel { get; set; }
        public UserGroups userGroups { get; set; }
        public int? companyID { get; set; }
        public virtual companyModel? CompanyModel { get; set; }
        public mainStatus accessStatus { get; set; }
        public string modifiedBy { get; set; }

        public virtual List<accessHistoryModel>? AccessHistories { get; set; }

        public accessModel() { }
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

   
    public enum UserGroups
    {
        [Display(Name = "HR Personnel")]
        PMS_HRCLERK,

        [Display(Name = "HR Admins")]
        PMS_HRMANAGER,

        //[Display(Name = "System Admin")]
        //PMS_ADMIN,

        //[Display(Name = "Management")]
        //PMS_MANAGEMENT,

        //[Display(Name = "Clinic")]
        //PMS_CLINIC
    }

}
