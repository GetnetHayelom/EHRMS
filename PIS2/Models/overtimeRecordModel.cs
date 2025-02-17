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
        public DateTime overtimeRecordDate { get; set; }
        public TimeSpan overtimeRecordStartTime { get; set; }
        public TimeSpan overtimeRecordEndTime { get; set; }
        public overtimeStatus overtimeRecordStatus { get; set; } = overtimeStatus.Requested;
        public virtual ICollection<overtimeHistoryModel>? OvertimeHistories { get; set; }
        public overtimeRecordModel() { }

    }
    

}
