using Microsoft.EntityFrameworkCore;
using PIS2.Enums;
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
        public string? payrollMonth { get; set; }//Or Period
        public int? companyID { get; set; }
        public virtual companyModel? companyModel { get; set; }
        [Precision(18, 2)]
        public decimal? totalGross { get; set; }
        [Precision(18, 2)]
        public decimal? totalNet { get; set; }
        [Precision(18, 2)]
        public decimal? totalTax { get; set; }
        [Precision(18, 2)]
        public decimal? totalPensionEmployee { get; set; }
        [Precision(18, 2)]
        public decimal? totalPensionEmployer { get; set; }
        public int? totalEmployees { get; set; }
        public DateTime StartDate {get; set;}
        public DateTime EndDate { get; set; }
        public payrollStatus payrollStatus { get; set; }
        public ICollection<payrollHistory>? payrollHistories { get; set; } = new List<payrollHistory>();
        public virtual ICollection<payrollPay>? PayrollPays { get; set; }
        public bool IsPayroll { get; set; } = true;
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public string? remark { get; set; }
        public string? referenceType { get; set; }
        public string? reference { get; set; }
        public decimal TotalPension => (totalPensionEmployee + totalPensionEmployer) ?? 0;
    
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
        public ICollection<earningRecordModel>? EarningRecords { get; set; } = new List<earningRecordModel>();
        public ICollection<deductionRecordModel>? DeductionRecords { get; set; } = new List<deductionRecordModel>();
        [Precision(18, 2)]
        public decimal GrossPay { get; set; }
        [Display(Name ="Credit Account")]
        public int CreditAccountID { get; set; }
        public virtual subAccountModel? CreditAccount{ get; set; }
        [Display(Name = "Debit Account")]
        public int DebitAccountID { get; set; }
        public virtual subAccountModel? DebitAccount{ get; set; }
        public int? jobPlacementID { get; set; }
        public virtual jobPlacementModel? JobPlacement { get; set; }
        public int? departmentID { get; set; }
        public virtual departmentModel? Department {get; set;}
        [Precision(18, 2)]
        public decimal NetPay { get; set; }
        [Precision(18, 2)]
        public decimal workedHours { get; set; }
        [Precision(18, 2)]
        public decimal hourlyRate { get; set; }
        [Precision(18, 2)]
        public decimal TotalEarning{get { return EarningRecords.Sum(e => e.earningAmount); }}
        [Precision(18, 2)]
        public decimal TotalDeduction { get { return DeductionRecords.Sum(d => d.deductionAmount) ??0; } }
        public string modifiedBy { get; set; }
        
        public payrollPay() { }
    }

    
    public class earningModel {
        [Key]
        public int earningID { get; set; }
        public int earningTypeID { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public earningType? earningType { get; set; }
        public string? earningReference { get; set; }
        public mainStatus earningStatus { get; set; }
        public int earningIteration { get; set; } = 1; //no of months the earning will recure
        public int remainingIteration { get; set; } = 0;
        // Calculation method
        public bool IsPercentage { get; set; } // true = % of base value, false = fixed
        public earningBase earningBase { get; set; } = earningBase.NONE;
        [Precision(18, 2)]
        public decimal earningAmount { get; set; }// e.g. 7 for 7%, or 500 for fixed
        public string modifiedBy { get; set; }
        public virtual List<earningHistoryModel>? EarningHistories { get; set; }
        public earningModel() { }
    }
    public class earningHistoryModel
    {
        [Key]
        public int earningHistoryID { get; set; }
        public int earningID { get; set; }
        public virtual earningModel? earningModel { get; set; }
        public mainStatus earningStatus { get; set; }
        [Precision(18, 2)]
        public decimal earningAmount { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public earningHistoryModel() { }
    }

    public class earningRecordModel
    {
        [Key]
        public int earningRecordID { get; set; }
        public int earningTypeID { get; set; }
        public earningType? earningType { get; set; }
        public string earningReference { get; set; }
        [Precision(18, 2)]
        public decimal earningAmount { get; set; }
        public int payrollPayID { get; set; }
        public virtual payrollPay? PayrollPay { get; set; }       
        public string modifiedBy { get; set; }
        public earningRecordModel() { }
    }
    public class earningType
    {
        [Key]
        public int earningTypeID { get; set; }
        public string? earningTypeCode { get; set; }
        public string earningTypeName { get; set; } // e.g. "Basic Salary", "Overtime", "Transport Allowance"     
        public bool isRecurring { get; set; } // true = applies every payroll (e.g. Salary), false = ad-hoc (e.g. Bonus)       
        public bool isTaxable { get; set; } // salary yes, per diem maybe no
        public bool isPayroll { get; set; } = true; //true if it is to be proccessed on payroll(i.e OT), false for(i.e medication benefit)
        public mainStatus earningTypeStatus { get; set; }
        public string? earningTypeDescription { get; set; }
        public int? accountID { get; set; }
        public virtual accountModel? Account{get; set;}
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public virtual List<allowanceModel>? Allowances { get; set; }

        public virtual ICollection<earningRecordModel>? Earnings { get; set; }
    }

    public class deductionModel
    {
        [Key]
        public int deductionID { get; set; }
        public int deductionTypeID { get; set; }
        public virtual deductionType? DeductionType { get; set; }
        public string? deductionReference { get; set; }
        public int employmentID { get; set; }
        public virtual employmentModel? EmploymentModel { get; set; }
        public int deductionIteration { get; set; } = 1; //no of month the deduction will recure
        public int remainingIteration { get; set; } = 1;
        // Calculation method
        public bool IsPercentage { get; set; } // true = % of base value, false = fixed
        [Precision(18, 2)]
        public decimal deductionAmount { get; set; }// e.g. 7 for 7%, or 500 for fixed
        public deductionBase deductionBase { get; set; }
        public mainStatus deductionStatus { get; set; }
        public string modifiedBy { get; set; }
        public virtual List<deductionHistoryModel>? DeductionHitroies { get; set; }
        public deductionModel() { }
    }
    public class deductionHistoryModel
    {
        [Key]
        public int deductionHistoryID { get; set; }
        public int deductionID { get; set; }
        public virtual deductionModel? deductionModel { get; set; }
        public mainStatus deductionStatus { get; set; }
        [Precision(18, 2)]
        public decimal deductionAmount { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }

        public deductionHistoryModel() { }
    }
    public class deductionRecordModel
    {
        [Key]
        public int deductionRecordID { get; set; }
        public int deductionTypeID { get; set; }
        public virtual deductionType? DeductionType { get; set; }
        public string? deductionReference { get; set; }
        [Precision(18, 2)]
        public decimal? deductionAmount { get; set; }
        public int payrollPayID { get; set; }
        public virtual payrollPay? PayrollPay { get; set; }
        public string modifiedBy { get; set; }
        public deductionRecordModel() { }
    }

    public class deductionType
    {
        [Key]
        public int deductionTypeID { get; set; }
        public string? deductionCode { get; set; }
        public string deductionName { get; set; } // e.g. "Tax", "Pension", "Penalty"
        public string? deductionDescription { get; set; }
        public deductionBase deductBase { get; set; }// e.g. "BasicSalary", "Gross", "Net", etc.
        public int deductionPriority { get; set; }//priority which to deduct first
        public bool isRecurring { get; set; } // tax/pension = recurring, penalty = one-time 
        public mainStatus deductionStatus { get; set; }
        public bool isMandatory { get; set; }//would apply to everyone
        public virtual ICollection<deductionRecordModel>? Deductions { get; set; }
        public int? accountID { get; set; }
        public virtual accountModel? Account { get; set; }
        public string modifiedBy {get; set;}
        public DateTime modifiedDate { get; set; } = DateTime.Now;
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
        [Precision(18, 2)]
        public decimal amount { get; set; }// taxable amount
        [Precision(18, 2)]
        public decimal taxRate { get; set; }
        [Precision(18, 2)]
        public decimal deduction { get; set; }
        public mainStatus taxStatus { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public string modifiedBy { get; set; }
        public taxRateModel() { }
    }
    
}
