using PIS2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class structureModel
    {
        [Key]
        public int structureID { get; set; }
        public int departmentID { get; set; }
        [ForeignKey("departmentID")]
        public virtual departmentModel? departmentModel { get; set; }
        
        public int jobID { get; set; }
        [ForeignKey("jobID")]
        public virtual jobModel? jobModel { get; set; }
        public int requiredNumber { get; set; }
        public mainStatus structureStatus { get; set; }
        public string modifiedBy { get; set; }
        public int? reportsTo { get; set; }
        [ForeignKey(nameof(reportsTo))]
        public virtual structureModel? ReportsTo { get; set; }
        public virtual ICollection<structureModel>? Subordinates { get; set; }
        public virtual List<structureHistoryModel>? StructureHistories { get; set; }
        public structureModel() {
            StructureHistories = new List<structureHistoryModel>();
        }
    }
    public class structureHistoryModel
    {
        [Key]
        public int structureHistoryID { get; set; }
        public int structureID { get; set; }
        public virtual structureModel? structureModel { get; set; }
        public int requiredNumber { get; set; }
        public mainStatus structureStatus { get; set; }
        public int? reportsTo { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public structureHistoryModel() { }
    }
}
