using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class penaltyModel
    {
        [Key]
        public int penaltyID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public DateTime penaltyIssueDate { get; set; } = DateTime.Now;
        public String penaltyReason { get; set; }
        public int penaltyTypeID { get; set; }
        public penaltyStatus penaltyStatus { get; set; }
        public virtual penaltyTypeModel? penaltyTypeModel { get; set; }
        public string modifiedBy { get; set; }
        public virtual ICollection<penaltyHistoryModel> PenaltyHistories { get; set; } 

        public penaltyModel() { }
    }

    public class penaltyHistoryModel
    {
        [Key]
        public int penaltyHistoryID { get; set; }
        public int penaltyID { get; set; }
        public virtual penaltyModel? penaltyModel { get; set; }
        public penaltyStatus penaltyStatus { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }
    }
    public class penaltyTypeModel
    {
        [Key]
        public int penaltyTypeID { get; set; }
        public string penaltyName { get; set; }
        public mainStatus penaltyTypeStatus { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }
        public virtual List<penaltyModel>? Penalties { get; set; }

        public penaltyTypeModel() { }
    }

    public enum penaltyStatus
    {
        Hold,
        Post,
        Void
    }
}
