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
        public string educationLevelCategory { get; set; }
        public string? educationLevelDescription { get; set; }
        public mainStatus educationLevelStatus { get; set; }
        public virtual ICollection<jobModel>? Jobs { get; set; } = new List<jobModel>();
        public virtual ICollection<personEducationLevelModel>? PersonEducationLevels { get; set; }
        public string modifiedBy { get; set; }
        public educationLevelModel()
        {

        }
    }
}
