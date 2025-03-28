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
        public string CompanyName { get; set; }
        public string Manager { get; set; }
        public int Departments { get; set; }
        public int Employees { get; set; }
        public decimal Salary { get; set; }
    }
}
