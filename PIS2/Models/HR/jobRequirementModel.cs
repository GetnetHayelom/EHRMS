using System.ComponentModel.DataAnnotations;
using PIS2.Enums;
using PIS2.Models.Foundation;
using PIS2.Models.Organization;
namespace PIS2.Models
{
    public class jobRequirementModel//request made to hr to employ new employees only managers can request
    {
        [Key]
        [Display(Name = "Employment Request Record ID")]
        public int jobRequirementID { get; set; }
        [Required]
        [Display(Name = "Requesting Department")]
        public int departmentID { get; set; }
        public virtual departmentModel? DepartmentModel { get; set; }
        [Required]
        [Display(Name = "Employment Requested Job")]
        public int jobID { get; set; }
        public virtual jobModel? JobModel { get; set; }
        [Required]
        [Display(Name = "Required No.")]
        public int requiredNumber { get; set; }
        [Display(Name = "Employment Type")]
        public int employmentTypeID { get; set; }
        public employmentTypeModel? employmentTypeModel { get; set; }
        [Display(Name = "Reason")]
        [Required]
        public string? requiredReason { get; set; }
        [Display(Name = "Approved No")]
        public int? approvedNumber { get; set; }
        [Display(Name = "Hired No")]
        public int? hiredNumber { get; set; }
        [Display(Name = "Request Status")]
        public jobReqStatus jobRequirementStatus { get; set; }
        [Display(Name = "Modified By")]
        public string modifiedBy { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public virtual ICollection<jobRequirementHistoryModel>? JobRequirementHistories { get; set; }
        public virtual ICollection<VacancyModel>? Vacancies { get; set; }
        public virtual ICollection<employmentModel>? Employments { get; set; }
        public jobRequirementModel() { }

    }
    public class jobRequirementHistoryModel
    {
        [Key]
        public int jobRequirementHistoryID { get; set; }
        [Required]
        public int jobRequirementID { get; set; }
        public virtual jobRequirementModel? JobRequirementModel { get; set; }
        [Required]
        [Display(Name = "Employment Requested Job")]
        public int jobID { get; set; }
        public virtual jobModel? JobModel { get; set; }
        [Required]
        public int requiredNumber { get; set; }
        public jobReqStatus jobRequirementStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public jobRequirementHistoryModel() { }

    }

    public class jobReqCost
    {
        [Key]
        public int jobReqCostID { get; set; }
        public int VacancyID { get; set; }
        public virtual VacancyModel? VacancyModel { get; set; }
        public jobReqStatus jobReqStatus { get; set; }
        public string jobReqCostReason { get; set; }
        public double jobReqCostEstimate { get; set; }
        public double jobReqCostActual { get; set; }
        public double jobReqCostReference { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public jobReqCost() { }

    }   

    public class VacancyModel
    {
        [Key]
        public int VacancyID { get; set; }
        [Required]
        [Display(Name ="Vacancy Title")]
        public string VacancyTitle { get; set; }

        [Required]
        [Display(Name = "Job Title/Position")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Job Title.")]
        public int jobID { get; set; }   
        public virtual jobModel? jobModel {get; set;}// Position name

        [StringLength(500)]
        public string? Remark { get; set; }// Job description

        public int? departmentID { get; set; }// Link to Department
        public virtual departmentModel? departmentModel { get; set; }

        public string? Location { get; set; }// Work location

        [Required]
        [Display(Name = "Required Number")]
        public int VacancyRequiredNumber { get; set; }// How many open slots

        public VacancyStatus Status { get; set; } = VacancyStatus.OnHold;// Open, Closed, OnHold
        public jobReqStatus VacancyStage { get; set; } = jobReqStatus.Hold;
        [Required]
        [Display(Name = "Date Posted")]
        public DateTime DatePosted { get; set; } = DateTime.Now;

        public DateTime? ClosingDate { get; set; }       // Optional closing date

        public int? jobRequirementID { get; set; }
        public virtual jobRequirementModel? jobRequirmentmodel { get; set; }
        [Required]
        [Display(Name = "Employment Method")]
        public int employmentMethodID { get; set; }
        public virtual employmentMethodModel? employmentMethodModel { get; set; }
        [Required]
        [Display(Name = "Employment Type")]
        public int employmentTypeID { get; set; }
        public virtual employmentTypeModel? employmentTypeModel { get; set; }
        [Required]
        [Display(Name = "Vacancy Type")]
        public VacancyTypes VacancyType { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<ApplicantModel>? Applicants { get; set; } = new List<ApplicantModel>();
        public virtual ICollection<jobReqCost>? JobReqCost { get; set; }
    }
    

    public class ApplicantModel
    {
        [Key]
        public int ApplicantID { get; set; }

        [Required]
        public int VacancyID { get; set; }
        public virtual VacancyModel? VacancyModel { get; set; }
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; }
        public string? ResumeFilePath { get; set; }     // Optional resume upload
        public DateTime AppliedDate { get; set; } = DateTime.Now;
        public ApplicantStatus Status { get; set; } = ApplicantStatus.Pending;
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
    }

    


}

