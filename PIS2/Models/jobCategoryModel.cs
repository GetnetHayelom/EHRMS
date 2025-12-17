using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class jobCategoryModel //Mangerial, Professional
    {
        [Key]
        public int jobCategoryID { get; set; }
        public String jobCategoryName { get; set; }
        public string? jobCategoryDescription { get; set; }
        public mainStatus jobCategoryStatus { get; set; }
        public virtual ICollection<jobModel>? Jobs { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public jobCategoryModel() { }
    }
}
