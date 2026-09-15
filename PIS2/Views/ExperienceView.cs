using Microsoft.EntityFrameworkCore;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Views
{
    public class ExperienceView
    {
        public int? experienceID { get; set; }
        public string? givenID { get; set; }
        public int? employmentID { get; set; }
        public string? employmentTypeName { get; set; }
        public string FullName { get; set; }
        public Ex_In experienceType { get; set; }
        public string jobTitle { get; set; }
        public string? jobDepartment { get; set; }
        public string? jobGrade { get; set; }
        [Precision(18, 2)]
        public decimal? jobSalary { get; set; }
        public string? jobStep { get; set; }
        public DateTime? experienceStartDate { get; set; } = default(DateTime?);
        public DateTime? experienceEndDate { get; set; } = default(DateTime?);
        public int personID { get; set; }
        public Gender personGender { get; set; }
    }
}
