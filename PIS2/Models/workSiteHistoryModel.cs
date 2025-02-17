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

        public workSiteHistoryModel()
        {

        }
    }
}
