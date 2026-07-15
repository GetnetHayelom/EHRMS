using PIS2.Enums;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class accountModel
    {
        [Key]
        public int accountID { get; set; }
        [Required]
        public string accountNumber {  get; set; }       
        public string accountName { get; set; }
        public string? accountDescription { get; set; }
        public mainStatus accountStatus { get; set; }
        public AccountType? accountType { get; set; }
        public int? ParentAccountID { get; set; }
        public accountModel ParentAccount { get; set; }
        //Navigation Property
        public virtual ICollection<subAccountModel>? SubAccounts { get; set; }
        public virtual ICollection<deductionType>? DeductionTypes { get; set; }
        public virtual ICollection<earningType>? EarningTypes { get; set; }
        public virtual ICollection<payrollPay>? PayrollPays { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public accountModel() { }
        public accountModel(string accountNumber, string accountName, string accountDescription, mainStatus accountStatus)
        {
            this.accountNumber = accountNumber;
            this.accountName = accountName;
            this.accountDescription = accountDescription;
            this.accountStatus = accountStatus;
        }

    }
    public class subAccountModel
    {
        [Key]
        public int subAccountID { get; set; }
        [Required]
        public string? subAccountNumber { get; set; }
        [Required]
        public int accountID { get; set; }
        public virtual accountModel? accountModel { get; set; }
        [Required]
        public string subAccountName { get; set; }
        [Required]
        public string? subAccountDescription { get; set; }
        [Required]
        public mainStatus subAccountStatus { get; set; }
        public virtual ICollection<departmentModel>? Departments { get; set; } = null!;
        public virtual personModel? Person { get; set; } = null!;
        public virtual ICollection<workSiteModel>? WorkSites { get; set; } = null!;
        public virtual ICollection<payrollPay>? DPayrollPays { get; set; } = null!;
        public virtual ICollection<payrollPay>? CPayrollPays { get; set; } = null!;



        public subAccountModel() { }
        public subAccountModel(int accountID, string subAccountName, string subAccountDescription, mainStatus status)
        {
            this.accountID = accountID;
            this.subAccountName = subAccountName;
            this.subAccountDescription = subAccountDescription;
            this.subAccountStatus = status;
        }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
    }

    public class JournalEntry
    {
        public int JournalEntryID { get; set; }

        public DateTime EntryDate { get; set; }
        public string Reference { get; set; }  // e.g. Payroll-2026-03
        public string Description { get; set; }

        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public List<JournalEntryLine> Lines { get; set; }
    }
    public class JournalEntryLine
    {
        public int JournalEntryLineID { get; set; }

        public int JournalEntryID { get; set; }
        public JournalEntry JournalEntry { get; set; }
        public int accountID { get; set; }
        public accountModel Account { get; set; }
        public int subAccountID { get; set; }
        public subAccountModel SubAccount { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    
}
