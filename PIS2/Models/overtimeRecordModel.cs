using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class overtimeRecordModel
    {
        [Key]
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
        public virtual ICollection<overtimeHistoryModel>? OvertimeHistories { get; set; }
        public int? departmentID { get; set; }
        public virtual departmentModel? departmentModel {get; set;}
        public string modifiedBy { get; set; }
        public int? oldBatchNbr { get; set; }
        public overtimeRecordModel() { }

    }
    

}
