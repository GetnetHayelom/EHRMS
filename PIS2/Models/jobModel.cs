using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace PIS2.Models
{
    public class jobModel
    {
        [Key]
        public int jobID { get; set; }
        public string? jobCode { get; set; }
        public string jobTitle { get; set; }
        public int jobGradeID { get; set; } 
        public virtual jobGradeModel? jobGradeModel { get; set; }
        public int jobCategoryID { get; set; }
        public virtual jobCategoryModel? jobCategoryModel { get; set; }
        public int jobClassID { get; set; }
        public virtual jobClassModel? jobClassModel { get; set; }
        public string? jobDescription { get; set; }
        public mainStatus jobStatus { get; set; }

        public virtual ICollection<educationLevelModel>? EducationLevels {get; set; }
        public virtual ICollection<jobPlacementModel>? JobPlacements { get; set; }
        public virtual ICollection<jobRequirementModel>? JobRequirements { get; set; }
        public virtual ICollection<employmentRequestModel>? EmploymentRequests { get; set; }
        public virtual List<structureModel>? Structures { get; set; }
        public string modifiedBy { get; set; }

        public jobModel() { }
    }
    public class jobGradeModel{
        [Key]
        public int jobGradeID { get; set; }
        public string jobGradeName { get; set; }
        public string? jobGradeDescription { get; set; }
        public double jobGradeBasicSalary { get; set; }
        public double jobGradeMidSalary { get; set; }
        public double jobGradeMaxSalary { get; set; }
        public mainStatus jobGradeStatus { get; set; }
        public virtual ICollection<jobModel>? Jobs { get; set; }
        public virtual ICollection<jobStepModel> JobSteps { get; set; }
        public string modifiedBy { get; set; }

        public jobGradeModel()
        {
        }
    }

}
