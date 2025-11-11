using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class prohibitionModel
    {
        [Key]
        public int prohibitionID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public DateTime prohibitionDate { get; set; } = DateTime.Now;
        public DateTime prohibitionStart { get; set; } =DateTime.Now;
        public DateTime prohibitionEnd { get; set; } = DateTime.MaxValue;
        public mainStatus prohibitionStatus { get; set; }
        public string prohibitionReason { get; set; }
        public ProhibitionType prohibitionType { get; set; }
        public string? prohibitionRemark { get; set; }
        public string modifiedBy { get; set; }
        public virtual ICollection<prohibitionHistoryModel>? ProhibitionHistories { get; set; }
         public prohibitionModel()
        {

        }
    }

    public class prohibitionHistoryModel
    {
        [Key]
        public int prohibitionHistoryID { get;}
        public int prohibitionID { get;}
        public virtual prohibitionModel prohibitionModel { get; set; }
        public DateTime prohibitionStart { get; }
        public DateTime prohibitionEnd { get;}
        public mainStatus prohibitionStatus { get; set; }
        public string prohibitionReason { get; }
        public ProhibitionType prohibitionType { get;}
        public string prohibitionRemark { get;}
        public string modifiedBy { get; }
        public DateTime modifiedDate { get; }

        public prohibitionHistoryModel()
        {

        }
    }

    public enum ProhibitionType
    {
        Leave,
        Step,
        Scale,
        Transfer,
        Guaranty,
        Exprience,
        Promotion,
        Overtime
    }
}
