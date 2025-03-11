using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class workSiteHistoryModel
    {
        [Key] public int workSiteHistoryID {get; set; }
        public int workSiteID {get; set; }
        public virtual workSiteModel? workSiteModel {get; set; }
        [Required]
        public mainStatus workSiteHistoryAction {get; set;}
        public DateTime workSiteHistoryDate {get; set;}
        public string worksiteName { get; set; }
        public string workSiteUser { get; set; }
        public int addressID { get; set; }
        public virtual addressModel? addressModel {get; set; }
        public workSiteHistoryModel()
        {

        }
    }
}
