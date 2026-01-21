using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class personEducationLevelModel
    {
        [Key]
        public int personEducationLevelID { get; set; }
        
        public int personID { get; set; }
        public personModel? personModel { get; set; }
        
        public int educationLevelID { get; set; }
        public educationLevelModel? educationLevelModel { get; set; }
        public DateOnly educationLevelDate { get; set; }
        public string educationLevelMark { get; set; }//CPA or GPA
        public string educationLevelInstitutionName { get; set; }
        public string? educationLevelNumber { get; set; }//Certificate serial Number if any
        public string? educationField { get; set; }
        public educationDomains? educationDomain { get; set; }//Natural, Social
        public string? educationDiscipline { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public List<int>? AttachementIDs { get; set; } =new List<int>();
        public personEducationLevelModel()
        {

        }
    }
}
