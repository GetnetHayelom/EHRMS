using System.ComponentModel.DataAnnotations;

namespace PIS2.Enums
{
    public enum mainStatus
    { 
        Active = 1,
        Inactive = 2,
        Suspended = 3
    }

    public enum UserGroups
    {
        [Display(Name = "HR Personnel")]
        HRCLERK,

        [Display(Name = "HR Admins")]
        HRMANAGER,

        //[Display(Name = "System Admin")]
        //ADMIN,

        //[Display(Name = "Management")]
        //MANAGEMENT,

        //[Display(Name = "Clinic")]
        //CLINIC
    }

    public enum delegationScopes
    {
        [Display(Name = "Leave Approve")]
        LeaveApprove,
        [Display(Name = "Overtime Approve")]
        OTApprove,
        [Display(Name = "Shift Assignment")]
        ShiftAssignment,
        [Display(Name = "Worksite Assignment")]
        WorkSiteAssignment,
        [Display(Name = "View Only")]
        ViewOnly,
        [Display(Name = "Full Delegation")]
        Fulldelegation
    }
    public enum familyRelation
    {
        Father,
        Mother,
        Son,
        Daughter,
        Sister,
        Brother,
        GrandFather,
        GrandMother,
        CareTaker,
        Uncle,
        Aunty,
        GrandSon,
        GrandDaughter,
        EmergencyContact,
        Other
    }
    public enum ServiceRequestStatus
    {
        Hold,
        Reviewed,
        Completed
    }
    public enum terminationStatus
    {
        Hold,
        Approved,
        Posted,
        Complete,
        Void
    }
    public enum EmploymentPositions
    {
        Non_Management,
        Management
    }
    public enum GuarantyTypes
    {
        [Display(Name = "Internal Loan")]
        Internal_Loan,
        [Display(Name = "External Loan")]
        External_Loan,
        [Display(Name = "Employment")]
        Employment,
        [Display(Name = "Collateral")]
        Collateral,
        [Display(Name = "Other")]
        Other
    }
    public enum loyaltyStatus
    {
        Approved,
        Hold,
        Posted,
        Completed,
        Declined
    }
    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }
    public enum NoticeStatus
    {
        Pending,
        Approved,
        Posted,
        Expired,
        Void
    }
    public enum overtimeStatus
    {
        Hold,
        Approved,
        Posted,
        Released,
        Completed,
        Cancelled,
        Void
    }
    //Education Enums
    #region 
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
    public enum educationCategory
    {
        Certificate,
        Diploma,
        Degree,
        Masters,
        PHD,
        Other
    }
    public enum Ex_In
    {
        External,
        Internal,

    }
    #endregion
    
    
    //Evaluation Enums
    #region
    public enum evaluationStatus
    {
        Pending,
        Submitted,
        Void
    }
    public enum ProficiencyLevel
    {
        [Display(Name = "Exceptional (4.0)")]
        Exceptional = 40,

        [Display(Name = "Very Good (3.5)")]
        VeryGood = 35,

        [Display(Name = "Good (3.0)")]
        Good = 30,

        [Display(Name = "Average (2.5)")]
        Average = 25,

        [Display(Name = "Unsatisfactory (2.0)")]
        Unsatisfactory = 20,

        [Display(Name = "Poor (1.5)")]
        Poor = 15,

        [Display(Name = "Very Poor (1.0)")]
        VeryPoor = 10,

        [Display(Name = "Unacceptable (0.5)")]
        Unacceptable = 5,

        [Display(Name = "No Output / Zero (0.0)")]
        Zero = 0
    }
    #endregion

    //Job and vacancy enums
    #region
    public enum JobCareer
    {
        Step,
        Transfer,
        Promotion,
        Demotion
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
    public enum ApplicantStatus
    {
        Pending = 1,
        ScreenPass = 2,
        ScreenFail = 3,
        InterviewPass = 4,
        InterviewFail = 5,
        ExamPass = 6,
        ExamFails = 7,
        Accepted = 8,
        Reserve = 9,
        Rejected = 10
    }
    #endregion
    //Letter Enums
    #region
    public enum LetterGroup
    {
        Incoming,
        Outgoing,
        Internal
    }
    public enum LetterStatus
    {
        Draft,
        Approved,
        Archived,
        Void
    }
    #endregion
    
    //Penalty and Prohibition
    #region
    public enum penaltyStatus
    {
        Hold,
        Post,
        Pending,
        Complete,
        Void
    }
    public enum penaltyCategory
    {
        Deciplinary,
        PerformanceRelated,
        PolicyViolation
    }
    public enum penaltyBase
    {
        SALARY,
        ALLOWANCE,
        NET,
        GROSS,
        NONE
    }
    public enum ProhibitionType
    {
        Leave,
        Step,
        Scale,
        Transfer,
        Guaranty,
        Exprience,
        Promotion,
        Overtime
    }
    #endregion
    
}
