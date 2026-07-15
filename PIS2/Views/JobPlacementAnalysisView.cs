using PIS2.Enums;

namespace PIS2.Views
{
    public class JobPlacementAnalysisView
    {
        public List<PlacementGap> UnderStaffed { get; set; } = new();
        public List<PlacementGap> OverStaffed { get; set; } = new();
        public List<UnstructuredPlacement> Unstructured { get; set; } = new();
    }
    public class PlacementGap
    {
        public string CompanyName { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public int Required { get; set; }
        public int Actual { get; set; }
        public int Difference => Math.Abs(Required - Actual);
    }

    public class UnstructuredPlacement
    {
        public int JobPlacementID { get; set; }
        public int EmploymentID { get; set; }
        public string GivenID { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string CompanyName { get; set; } = "";
    }
    public class JobGapAnalysis
    {
        public string JobTitle { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public int DeptID { get; set; }
        public int JobID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; } = "";
        public int TargetCount { get; set; }
        public int ActualCount { get; set; }
        public int Variance { get; set; }
        public string PolicyStatus { get; set; } = "";
    }

    public class JobPlacementDetailView
    {
        public int jobPlacementID { get; set; }
        public int employmentID { get; set; }
        public string givenID { get; set;}
        public string FullName { get; set; }
        public DateTime jobPlacementDate { get; set; }
        public int PlacementYear { get; set; }
        public decimal jobPlacementSalary { get; set; }
        public decimal HourlyRate { get; set; }
        public int jobID { get; set; }
        public string jobTitle { get; set; }
        public int jobCategoryID { get; set; }
        public string jobCategoryName { get; set; }
        public int jobClassID { get; set; }
        public string jobClassName { get; set; }
        public int jobGradeID { get; set; }
        public string jobGradeName { get; set; }
        public int jobStepID { get; set; }
        public string jobStepName { get; set; }
        public mainStatus jobPlacementStatus { get; set; }
        public string jobPlacementReason { get; set; }
        public int departmentID { get; set; }
        public string departmentName { get; set; }
        public int companyID { get; set; }
        public string companyName { get; set; }
        public DateTime modifiedDate { get; set; }
    }

    public class JobPlacementView
    {
        public int jobPlacementID { get; set; }
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public string FullName { get; set; }
        public DateTime jobPlacementDate { get; set; }
        public decimal jobPlacementSalary { get; set; }
        public decimal HourlyRate { get; set; }
        public int jobID { get; set; }
        public string jobTitle { get; set; }
        public int jobCategoryID { get; set; }
        public string jobCategoryName { get; set; }
        public int jobClassID { get; set; }
        public string jobClassName { get; set; }
        public int jobGradeID { get; set; }
        public string jobGradeName { get; set; }
        public int jobStepID { get; set; }
        public string jobStepName { get; set; }
        public mainStatus jobPlacementStatus { get; set; }
        public string jobPlacementReason { get; set; }
        public int departmentID { get; set; }
        public string departmentName { get; set; }
        public int companyID { get; set; }
        public string companyName { get; set; }
    }
    public class YearlyJobPlacementSalaryView
    {
        public int PlacementYear { get; }
        public string jobCategoryName { get; }
        public int TotalPlacements { get; }
        public decimal AverageSalary { get; }
        public decimal MinSalary { get; }
        public decimal MaxSalary { get; }
        public int TotalPromotions { get; }
        public int UniqueEmployeesImpacted { get; }

    }

    public class EmployeeSalaryGrowthView
    {
        public int employmentID { get; }
        public int jobPlacementID { get; }
        public string jobTitle { get; }
        public int PlacementYear { get; }
        public decimal CurrentSalary { get; }
        public decimal PreviousSalary { get; }
        public decimal SalaryIncrease { get; }
        public string CurrentGrade { get; }
        public string jobPlacementReason {get;}
        public DateTime modifiedDate { get; }

    }
}
