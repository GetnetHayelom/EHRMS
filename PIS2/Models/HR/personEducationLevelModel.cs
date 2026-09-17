using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PIS2.Enums;
using PIS2.Models.Foundation;
namespace PIS2.Models.HR
{
    public class personEducationLevelModel
    {
        [Key]
        [Display(Name ="Person Education Record ID")]
        public int personEducationLevelID { get; set; }
        [Display(Name = "Person")]
        public int personID { get; set; }
        public personModel? personModel { get; set; }
        [Display(Name = "Education Level")]
        public int educationLevelID { get; set; }
        public educationLevelModel? educationLevelModel { get; set; }
        [Display(Name = "Date Issued")]
        public DateOnly educationLevelDate { get; set; }
        [Display(Name = "Obtained Grade/Mark/Result")]
        public string? educationLevelMark { get; set; } = "N/A";//CPA or GPA
        [Display(Name = "Institution Name")]
        public string educationLevelInstitutionName { get; set; }
        [Display(Name = "Certification No.")]
        public string? educationLevelNumber { get; set; }//Certificate serial Number if any
        [Display(Name = "Field of Study")]
        public string? educationField { get; set; }
        public educationDomains? educationDomain { get; set; }//Natural, Social
        [Display(Name = "Diciplnine/Domain")]
        public string? educationDiscipline { get; set; }
        [Display(Name = "Modified By")]
        public string modifiedBy { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        [Display(Name = "Attachements")]
        public List<int>? AttachementIDs { get; set; } =new List<int>();
        public personEducationLevelModel()
        {

        }
    }
}
