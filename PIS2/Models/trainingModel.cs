using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using PIS2.Enums;

namespace PIS2.Models
{
    public class trainingModel
    {
        [Key]
        public int trainingID { get; set; }

        [Required]
        public string trainingTitle { get; set; }

        public string? description { get; set; }

        public trainingCategory category { get; set; }   // Technical, HR, Safety…

        public bool isMandatory { get; set; }

        public int? validityMonths { get; set; }  // e.g. Safety training valid for 12 months

        public bool isActive { get; set; } = true;
        public trainingStatus trainingStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public virtual ICollection<trainingSessionModel>? TrainingSessions { get; set; }
    }

    /// <summary>
    /// Training Sessions
    /// </summary>
    public class trainingSessionModel
    {
        [Key]
        public int trainingSessionID { get; set; }

        public int trainingID { get; set; }
        public virtual trainingModel? Training { get; set; }
        public string? trainingSessionTitle { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string? location { get; set; }
        public trainingDeliveryMode deliveryMode { get; set; } // Online, Onsite, Hybrid
        public int? personID { get; set; }
        public virtual personModel? PersonModel { get; set; }
        public string? provider { get; set; }
        [Precision(18, 2)]
        public decimal? cost { get; set; }

        public trainingStatus sessionStatus { get; set; } // Planned, Ongoing, Completed, Cancelled
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public virtual ICollection<trainingAttendanceModel>? Attendances { get; set; }
    }

    /// <summary>
    /// Training Session Attendance
    /// </summary>
    public class trainingAttendanceModel
    {
        [Key]
        public int trainingAttendanceID { get; set; }

        public int trainingSessionID { get; set; }
        public virtual trainingSessionModel? TrainingSession { get; set; }

        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }

        public trainingResult result { get; set; } // Passed, Failed, InProgress, Absent

        public DateTime? completionDate { get; set; }
        [Precision(18, 2)]
        public decimal? score { get; set; }

        public string? certificateNumber { get; set; }
        public DateTime? certificateExpiryDate { get; set; }

        public string? remarks { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
    }

    /// <summary>
    /// Training Cost Allocation
    /// </summary>
    public class trainingCostAllocationModel
    {
        [Key]
        public int trainingCostAllocationID { get; set; }
        public int trainingAttendanceID { get; set; }
        public virtual trainingAttendanceModel? Attendance { get; set; }
        public string? remarks { get; set; }
        [Precision(18, 2)]
        public decimal allocatedCost { get; set; } = 0;
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
    }

    /// <summary>
    /// Enums for Options
    /// </summary>

    


}
