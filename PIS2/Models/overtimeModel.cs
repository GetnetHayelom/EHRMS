using PIS2.Enums;
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
        public decimal overtimeRate { get; set; }
        public virtual ICollection<overtimeRecordModel>? OvertimeRecords { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public overtimeModel() { }
    }
}
