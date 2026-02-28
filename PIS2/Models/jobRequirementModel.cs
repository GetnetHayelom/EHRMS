using System.ComponentModel.DataAnnotations;

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
    public enum jobReqStatus
    {
        Hold,
        Approved,
        Staged,
        Published,
        Recruited,
        Screened,
        Exam,
        Interview,
        Completed,
        Declined,
        Failed
    }

    public class VacancyModel
    {
        [Key]
        public int VacancyID { get; set; }
        public string VacancyTitle { get; set; }

        [Required]
        public int jobID { get; set; }   
        public virtual jobModel? jobModel {get; set;}// Position name

        [StringLength(500)]
        public string? Remark { get; set; }           // Job description

        [Required]
        public int? departmentID { get; set; }             // Link to Department
        public virtual departmentModel? departmentModel { get; set; }

        public string? Location { get; set; }              // Work location

        [Required]
        public int VacancyRequiredNumber { get; set; }        // How many open slots

        [Required]
        public VacancyStatus Status { get; set; }         // Open, Closed, OnHold
        public jobReqStatus VacancyStage { get; set; } = jobReqStatus.Hold;
        public DateTime DatePosted { get; set; } = DateTime.Now;

        public DateTime? ClosingDate { get; set; }       // Optional closing date

        public int? jobRequirementID { get; set; }
        public virtual jobRequirementModel? jobRequirmentmodel { get; set; }
        public int employmentMethodID { get; set; }
        public virtual employmentMethodModel? employmentMethodModel { get; set; }

        public int employmentTypeID { get; set; }
        public virtual employmentTypeModel? employmentTypeModel { get; set; }
        public VacancyTypes VacancyType { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<ApplicantModel>? Applicants { get; set; } = new List<ApplicantModel>();
        public virtual ICollection<jobReqCost>? JobReqCost { get; set; }
    }
    public enum VacancyStatus
    {
        Open = 1,
        Closed = 2,
        OnHold = 3
    }
    public enum VacancyTypes
    {
        [Display(Name = "Internal")]
        Internal,
        [Display(Name = "External")]
        External,
        [Display(Name = "Internal\\External")]
        In_Ex
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

    public enum ApplicantStatus
    {
        Pending = 1,
        ScreenPass =2,
        ScreenFail =3,
        InterviewPass = 4,
        InterviewFail =5,
        ExamPass =6,
        ExamFails =7,
        Accepted = 8,
        Reserve =9,
        Rejected = 10
    }


}

