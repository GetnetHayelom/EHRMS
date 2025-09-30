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
    public enum educationDomains
    {
        [Display(Name = "Natural Sciences, Mathematics and Statistics")]
        Natural,
        [Display(Name = "Engineering, Manufacturing and Construction")]
        Engineering,
        [Display(Name = "Information and Communication Technologies (ICT)")]
        ICT,
        [Display(Name = "Health and Welfare")]
        Health,
        [Display(Name = "Social Sciences, Journalism and Information")]
        Social,
        [Display(Name = "Arts and Humanities")]
        Art,
        [Display(Name = "Business, Administration and Law")]
        MBA,
        [Display(Name = "Agriculture, Forestry, Fisheries and Veterinary")]
        Agriculture,
        [Display(Name = "Services")]
        Services

    }
}
