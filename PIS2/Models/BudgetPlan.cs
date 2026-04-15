using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class BudgetPlan
    {
        [Key]
        public int BudgetPlanID { get; set; }

        [Required, StringLength(100)]
        public string BudgetName { get; set; } // e.g., "Fiscal Year 2026 - Q1"

        public string? BudgetPeriod { get; set; } = "";
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public BudgetStatus Status { get; set; } = BudgetStatus.HOLD;
        public string Description { get; set; }
        public string? FiscalYear { get; set; } = "";
        [Precision(18, 2)]
        public decimal? TotalAmount { get; set; } 
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }= DateTime.Now;
        // Navigation
        public virtual ICollection<BudgetLine>? BudgetLines { get; set; }=new List<BudgetLine>();
        [Precision(18, 2)]
        public decimal? UnAllocated { get { return TotalAmount - BudgetLines?.Sum(b => b.AllocatedAmount); } }
    }

    public class BudgetLine
    {
        [Key]
        public int BudgetLineID { get; set; }
        public int BudgetPlanID { get; set; }
        public virtual BudgetPlan? BudgetPlan { get; set; }

        // Linked to your existing subAccountModel (Department)
        public int subAccountID { get; set; }
        public virtual subAccountModel SubAccount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AllocatedAmount { get; set; }
        public string Notes { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
    }
    public class BudgetSummaryViewModel
    {
        public int BudgetLineID { get; set; }
        public int BudgetID {get; set;}
        public string DepartmentName { get; set; }
        public string SubAccountNumber { get; set; }
        [Precision(18, 2)]
        public decimal Budgeted { get; set; }
        [Precision(18, 2)]
        public decimal ActualSpent { get; set; }
        [Precision(18, 2)]
        public decimal Remaining => Budgeted - ActualSpent;
        [Precision(18, 2)]
        public decimal UtilizationRate => Budgeted > 0 ? (ActualSpent / Budgeted) * 100 : 0;
    }
    public enum BudgetStatus 
    {
        HOLD,
        APPROVED,
        ACTIVE,
        CLOSED,
        DECLINED
    }
}
