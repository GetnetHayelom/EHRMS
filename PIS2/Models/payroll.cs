using PIS2.Pages.EmployeeService;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace PIS2.Models
{
    public class payrollModel
    {
        [Key]
        public int payrollID { get; set; }
        public string payrollName { get; set; }
        public DateTime StartDate {get; set;}
        public DateTime EndDate { get; set; }
        public payrollStatus payrollStatus { get; set; }
        public ICollection<payrollHistory>? payrollHistories { get; set; }
        public string modifiedBy { get; set; }
        public payrollModel() { }
    }

    public class payrollHistory
    {
        [Key]
        public int payrollHistoryID { get; set; }
        public int payrollID { get; set; }
        public virtual payrollModel? payrollModel { get; set; }
        public payrollStatus? payrollStatus { get; set; }
        public DateTime modfiedDate { get; set; }
        public string modifiedBy { get; set; }
        public payrollHistory() { }
    }

    public class payrollPay
    {
        [Key]
        public int payrollPayID { get; set; }
        public int payrollID { get; set; }
        public virtual payrollModel? payrollModel { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public ICollection<earningModel> earnings { get; set; }
        public ICollection<deductionModel> deductions { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public string modifiedBy { get; set; }
        public payrollPay() { }
    }

    public class earningModel
    {
        [Key]
        public int earningID { get; set; }
        public int EarningTypeId { get; set; }
        public earningType? EarningType { get; set; }
        public int earningReference { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public decimal earningAmount { get; set; }
        public int? payrollID { get; set; }
        public virtual payrollModel? PayrollModel { get; set; }
        public mainStatus earningStatus { get; set; }
        public string modifiedBy { get; set; }
        public earningModel() { }
    }
    public class earningType
    {
        [Key]
        public int earningTypeID { get; set; }
        public string earningName { get; set; } // e.g. "Basic Salary", "Overtime", "Transport Allowance"
        public bool isRecurring { get; set; } // true = applies every payroll (e.g. Salary), false = ad-hoc (e.g. Bonus)
        public bool isTaxable { get; set; } // salary yes, per diem maybe no
        public mainStatus Status { get; set; }

        public virtual ICollection<earningModel>? Earnings { get; set; }
    }

    public class deductionModel
    {
        [Key]
        public int deductionID { get; set; }
        public int deductionTypeID { get; set; }
        public deductionType? DeductionType { get; set; }
        public int deductionReference { get; set; }
        
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public decimal deductionAmount { get; set; }
        public int payrollID { get; set; }
        public virtual payrollModel? payrollModel { get; set; }
        public mainStatus deductionStatus { get; set; }
        public string modifiedBy { get; set; }
        public deductionModel() { }
    }

    public class deductionType
    {
        [Key]
        public int deductionTypeID { get; set; }
        public string deductionName { get; set; } // e.g. "Tax", "Pension", "Penalty"
        public bool isRecurring { get; set; } // tax/pension = recurring, penalty = one-time
        public mainStatus Status { get; set; }

        public virtual ICollection<deductionModel>? Deductions { get; set; }
    }
    public enum payrollStatus
    {
        PENDING = 0,
        APPROVED = 1,
        POSTED = 2,
        COMPLETED = 3
    }
}
