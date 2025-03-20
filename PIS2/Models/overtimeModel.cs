using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class overtimeModel
    {
        [Key]
        public int overtimeID { get; set; }
        public string overtimeName { get; set; }
        public string?  overtimeDescription { get; set; }
        public mainStatus overtimeStatus { get; set; }
        public double overtimeRate { get; set; }
        public virtual ICollection<overtimeRecordModel>? OvertimeRecords { get; set; }
        public string modifiedBy { get; set; }
        public overtimeModel() { }
    }
}
