using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PIS2.Models
{
    public class experienceModel
    {
        [Key] public int experienceID { get; set; }
  
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; }
        [Required]
        public DateTime experienceStartDate { get; set; }
        [AllowNull]
        public DateTime experienceEndDate { get; set; }
        public string? jobTitle { get; set; }
        public string? jobGrade { get; set; }
        public string? jobStep { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? jobSalary { get; set; }
        public string? jobDepartment { get; set; }
        public Ex_In experienceType { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }=DateTime.Now;
        public List<int>? AttachementIDs { get; set; } = new List<int>();


        public experienceModel()
        {

        }

    }


    public enum Ex_In
    {
        External,
        Internal,

    }
}
