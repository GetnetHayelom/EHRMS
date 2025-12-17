using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class employmentTypeModel
    {
        [Key]
        public int employmentTypeID { get; set; }
        public string employmentTypeName { get; set; }
        public string? employmentTypeDescription { get; set; }
        public bool isLeaveCount { get; set; } = false;
        public bool isExprienceCount { get; set; } =false;
        public bool isSalaryAllowed { get; set; } = false;
        public bool isLoyalityAllowed { get; set; } = false;
        public bool isSeveranceAllowed { get; set; } = false;
        public bool isCarrierAllowed { get; set; } = false;
        public bool isStepAllowed { get; set; } = false;
        public bool isPensionAllowed { get; set; } = false;
        public mainStatus employmentTypeStatus { get; set; }
        public double? annualAccrualRate { get; set; } = 1;
        public double? employmentBaseLeave { get; set; } = 20;
        public int employmentMinAge { get; set; } = 18;
        public int employmentMaxAge { get; set; } = 60;
        public double? maxLeaveIncrement { get; set; } = 40;
        public int? probationDays = 45;
         
        public ICollection<employmentHistoryModel>? EmploymentHistories { get; set; } = null!;
        public ICollection<employmentModel>? Employments { get; set; } = null!;
        public virtual ICollection<employmentRequestModel>? EmploymentRequests { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public employmentTypeModel()
        {

        }
    }
}
