using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class contractModel
    {
        [Key]
        public int contractID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string contractRemark { get; set; } = "";
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public string modifiedBy { get; set; }
        public virtual ICollection<contractHistoryModel>? ContractHistories { get; set; }
        public contractModel() { }
    }
    public class contractHistoryModel
    {
        [Key]
        public int contractHistotyID { get; set; }
        public int contractID { get; set; }
        public virtual contractModel? contractModel { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string contractHistoryRemark { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }
        public contractHistoryModel() { }
    }
}
