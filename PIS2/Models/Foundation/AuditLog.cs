using System.ComponentModel.DataAnnotations;

namespace PIS2.Models.Foundation
{
    public class AuditLog
    {
        [Key]
        public int AuditID { get; set; }
        public string TableName { get; set; }
        public int RecordID { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public AuditLog() { }
    }
}
