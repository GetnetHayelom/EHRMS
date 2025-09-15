using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using PIS2.Pages.OvertimeHistory;
using System.Text.RegularExpressions;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Views
{
    
    public class views
    {}
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
        public decimal Allowance { get; set; } = 0;
        public decimal Overtime { get; set; }           
        
        public decimal getTotalExpence()
        {
            return Salary + Allowance + Overtime;
        }
        public decimal payableLeaves { get; set; }
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
        public decimal Overtime { get; set; }
        public decimal Allowance { get; set; }
        public decimal Total { get; set; }
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
