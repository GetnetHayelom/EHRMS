using PIS2.Pages.EmployeeService;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Security.Permissions;

namespace PIS2.Models
{
    public class payrollModel
    {
        [Key]
        public int payrollID { get; set; }
        public string payrollName { get; set; }
        public string payrollMonth { get; set; }
        public int? companyID { get; set; }
        public virtual companyModel? companyModel { get; set; }
        public decimal? totalGross { get; set; }
        public decimal? totalNet { get; set; }
        public decimal? totalTax { get; set; }
        public decimal? totalPensionEmployee { get; set; }
        public decimal? totalPensionEmployer { get; set; }
        public int? totalEmployees { get; set; }
        public DateTime StartDate {get; set;}
        public DateTime EndDate { get; set; }
        public payrollStatus payrollStatus { get; set; }
        public ICollection<payrollHistory>? payrollHistories { get; set; } = new List<payrollHistory>();
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
        public DateTime modifiedDate { get; set; }
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
        public ICollection<earningModel> earnings { get; set; } = new List<earningModel>();
        public ICollection<deductionModel> deductions { get; set; } = new List<deductionModel>();
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public string modifiedBy { get; set; }
        
        public payrollPay() { }
    }

    public class earningModel
    {
        [Key]
        public int earningID { get; set; }
        public int earningTypeId { get; set; }
        public earningType? earningType { get; set; }
        public int earningReference { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public decimal earningAmount { get; set; }
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
        public int earningIteration { get; set; } = 1; //no of monthe the deduction will recure
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
        public decimal? deductionAmount { get; set; }
         public mainStatus deductionStatus { get; set; }
        public string modifiedBy { get; set; }
        public deductionModel() { }
    }

    public class deductionType
    {
        [Key]
        public int deductionTypeID { get; set; }
        public string deductionName { get; set; } // e.g. "Tax", "Pension", "Penalty"
        public bool fromGross { get; set; }
        public int dedcutionPriority { get; set; }//priority which to deduct first
        public bool isRecurring { get; set; } // tax/pension = recurring, penalty = one-time
        public int deductionIteration { get; set; } = 1; //no of monthe the deduction will recure
        public mainStatus Status { get; set; }
        public bool isMandatory { get; set; }//would apply to everyone
        public virtual ICollection<deductionModel>? Deductions { get; set; }
        public deductionType() { }
    }

    public class taxRateModel
    {
        [Key]
        public int taxRateID { get; set; }
        /// <summary>
        /// Reference to the Proclamation e.g.
        /// </summary>
        public string reference { get; set; }
        public decimal from { get; set; }
        public decimal ceiling { get; set; }
        public decimal taxRate { get; set; }
        public decimal deduction { get; set; }
        public mainStatus tazStatus { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }
        public taxRateModel() { }
    }
    public enum payrollStatus
    {
        PENDING = 0,
        APPROVED = 1,
        POSTED = 2,
        COMPLETED = 3
    }
}
