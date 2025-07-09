using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class siteAssignmentModel
    {
        [Key]
         public int siteAssignmentID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public int workSiteID { get; set; }
        public virtual workSiteModel? workSiteModel { get; set; }
        public DateTime? modifiedDate { get; set; } = DateTime.Now;
        public string modifiedBy { get; set; }

        public siteAssignmentModel() { }
    }
}
