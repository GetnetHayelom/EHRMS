using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PIS2.Enums;
namespace PIS2.Models
{
    public class overtimeRecordModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int overtimeRecordID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public int overtimeID { get; set; }
        public virtual overtimeModel? overtimeModel { get; set; } = null!;
        public string overtimeRecordReason { get; set; }
        public DateTime overtimeRecordDate { get; set; } = DateTime.Now;
        public TimeSpan overtimeRecordStartTime { get; set; }
        public TimeSpan overtimeRecordEndTime { get; set; }
        public overtimeStatus overtimeRecordStatus { get; set; } = overtimeStatus.Hold;
        public decimal overtimeRecordEmploymentRate { get; set; } = 0;
        public decimal overtimeRate { get; set; } = 0;
        public virtual ICollection<overtimeHistoryModel>? OvertimeHistories { get; set; }
        public int? departmentID { get; set; }
        public virtual departmentModel? departmentModel {get; set;}
        public string modifiedBy { get; set; }
        public int? oldBatchNbr { get; set; }
        [Precision(18, 2)]
        public decimal? overtimeAmount { get; }
        public TimeSpan TimeElapsed { get { return overtimeRecordEndTime - overtimeRecordStartTime; } }
        public decimal GetOtCost { get { return (decimal)TimeElapsed.TotalHours*overtimeRate*overtimeRecordEmploymentRate; } }  
        public overtimeRecordModel() { }
        

    }
    

}
