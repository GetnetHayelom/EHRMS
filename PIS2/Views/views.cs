using PIS2.Models;

namespace PIS2.Views
{
    public class views
    {
    }
    public class EducationLevelData
    {
        public string EducationLevelCategory { get; set; }
        public string EducationLevelName { get; set; }
        public int EducationLevelCount { get; set; }
    }
    public class NameAndCount
    {
        public string zName { get; set; }
        public int zCount { get; set; }
    }
    public class YearAndCount
    {
        public int zYear { get; set; }
        public int zCount { get; set; }
    }
    public class CompanySummary
    {
      public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Manager { get; set; }
        public int Departments { get; set; }
        public int Employees { get; set; }
        public int xEmployees { get; set; }
        public decimal Salary { get; set; }
        public leaveDetail Leaves { get; set; }
    }
    public class DepartmentSummary
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Manager { get; set; }
        public int Employees { get; set; }
        public int xEmployees { get; set; }
        public decimal Salary { get; set; }
        public leaveDetail Leaves { get; set; }
        public double Overtime { get; set; }
        public double Allowance { get; set; }
        public double Total { get; set; }
    }
    public class EmployeeView
    {
        public employmentModel Employment{ get; set; }
        public personModel Person { get; set; }
        public jobPlacementModel Job { get; set; }
        public departmentModel DepartmentModel { get; set; }
        public companyModel Company { get; set; }
        public workSiteModel WorkSite { get; set; }
        public addressModel Address { get; set; }
        public shiftModel Shift { get; set; }
        public leaveDetail Leave { get; set; }
    }
}
