using PIS2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models.HR
{
    public class allowanceModel
    {
        [Key]
        public int allowanceID { get; set; }
        [Required]
        public string allowanceName { get; set; }
        public string? allowanceDescription { get; set; }
        public decimal allowanceAmount { get; set; } = 0;
        public bool allowanceTaxable { get; set; } = true;
        public mainStatus allowanceStatus { get; set; }
        public allowanceDuration allowanceDuration { get; set; }
        public int? earningTypeID { get; set; }
        public virtual earningType? EarningType { get; set; }
        //Navigation Property
        public virtual ICollection<allowanceAssignmentModel>? AllowanceAssignments { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public allowanceModel() { }
    }
    
}
