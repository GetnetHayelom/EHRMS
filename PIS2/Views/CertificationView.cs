using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Views
{
    public class CertificationView
    {
        public string CertificationType { get; set; }
        public int? CategoryID { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int TotalCount { get; set; }
        public CertificationView() { }
    }

    public class CertificationSummaryView
    {
        public educationCategory CertificationCategory { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Total { get; set; }

    }

    public class CertificationBreakDownView
    {
        public educationCategory EducationLevelCategory { get; set; }
        public string EducationLevelName { get; set; }
        public int EducationLevelCount { get; set; }
    }

    ///<summary>
    ///Certification Details for Active Employee
    /// </summary>
    /// 
    public class CertificationDetailsView
    {
        public int? CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public int? EducationLevelID { get; set; }
        public educationCategory? EducationLevelCategory { get; set; }
        public string? EducationField { get; set; }
        public string? EducationDiscipline { get; set; }
        public string? EducationDomain { get; set; }
        public DateTime? EducationLevelDate { get; set; }
        public string? EducationLevelMark { get; set; }
        public string? EducationLevelInstitutionName { get; set; }
        public int PersonID { get; set; }
        public string FullName { get; set; }
        public Gender PersonGender { get; set; }
        public string? GivenID { get; set; }
        public mainStatus EmploymentStatus { get; set; }
        public EmploymentPositions? EmploymentPosition { get; set; }


    }

}
