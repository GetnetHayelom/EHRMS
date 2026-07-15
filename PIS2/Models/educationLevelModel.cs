using PIS2.Enums;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class educationLevelModel
    {
        [Key]
        public int educationLevelID { get; set; }
        [Required]
        public string educationLevelName { get; set; }
        public string educationLevelGrade { get; set; }
        public educationCategory educationLevelCategory { get; set; }
        public string? educationLevelDescription { get; set; }
        public mainStatus educationLevelStatus { get; set; }
       
        public virtual ICollection<jobModel>? Jobs { get; set; } = new List<jobModel>();
        public virtual ICollection<personEducationLevelModel>? PersonEducationLevels { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public educationLevelModel()
        {

        }
    }
   
}
