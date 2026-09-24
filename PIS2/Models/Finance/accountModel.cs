using PIS2.Enums;
using PIS2.Models.Foundation;
using PIS2.Models.Organization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models.Finance
{    
    public class accountModel
        {
            [Key]
            public int accountID { get; set; }

            [Required]
            [MaxLength(30)]
            public string accountNumber { get; set; } = string.Empty;

            [Required]
            [MaxLength(200)]
            public string accountName { get; set; } = string.Empty;

            [MaxLength(500)]
            public string? accountDescription { get; set; }

            public mainStatus accountStatus { get; set; }

            public AccountType? accountType { get; set; }

            // Hierarchical account structure
            public int? ParentAccountID { get; set; }

            [ForeignKey(nameof(ParentAccountID))]
            public virtual accountModel? ParentAccount { get; set; }

            public virtual ICollection<accountModel> ChildAccounts
            {
                get;
                set;
            } = new List<accountModel>();

            // Subaccounts
            public virtual ICollection<subAccountModel> SubAccounts
            {
                get;
                set;
            } = new List<subAccountModel>();

            // Posting control
            public bool IsPostingAccount { get; set; } = true;

            // Existing payroll relationships
            public virtual ICollection<deductionType> DeductionTypes
            {
                get;
                set;
            } = new List<deductionType>();

            public virtual ICollection<earningType> EarningTypes
            {
                get;
                set;
            } = new List<earningType>();

            public virtual ICollection<payrollPay> PayrollPays{get;set;} = new List<payrollPay>();

            // Audit
            [MaxLength(100)]
            public string modifiedBy { get; set; } = string.Empty;

            public DateTime modifiedDate { get; set; } = DateTime.Now;
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
        [Key]
        public int JournalEntryID { get; set; }

        [Required]
        public int companyID { get; set; }

        [ForeignKey(nameof(companyID))]
        public virtual companyModel? Company { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Reference { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public JournalStatus Status { get; set; } = JournalStatus.Draft;

        // Accounting period reference
        public int? accountingPeriodID { get; set; }

        public virtual accountingPeriodModel? AccountingPeriod { get; set; }

        // Source integration
        [MaxLength(50)]
        public string? SourceModule { get; set; }

        [MaxLength(100)]
        public string? SourceReference { get; set; }

        // Audit
        [MaxLength(100)]
        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<JournalEntryLine> Lines
        {
            get;
            set;
        } = new List<JournalEntryLine>();
    }
    public class JournalEntryLine
    {
        [Key]
        public int JournalEntryLineID { get; set; }

        [Required]
        public int JournalEntryID { get; set; }

        public virtual JournalEntry? JournalEntry { get; set; }

        // Main account
        [Required]
        public int accountID { get; set; }

        public virtual accountModel? Account { get; set; }

        // Sub account / entity dimension
        [Required]
        public int subAccountID { get; set; }

        public virtual subAccountModel? SubAccount { get; set; }

        // Display/order of lines
        [Required]
        public int lineNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class companyAccountModel
        {
            [Key]
            public int companyAccountID { get; set; }

            [Required]
            public int companyID { get; set; }

            [ForeignKey(nameof(companyID))]
            public virtual companyModel? Company { get; set; }

            [Required]
            public int accountID { get; set; }

            [ForeignKey(nameof(accountID))]
            public virtual accountModel? Account { get; set; }

            // Company-specific settings
            public bool isActive { get; set; } = true;

            // Optional company-specific account number
            [MaxLength(30)]
            public string? companyAccountNumber { get; set; }

            [MaxLength(100)]
            public string modifiedBy { get; set; } = string.Empty;

            public DateTime modifiedDate { get; set; } = DateTime.Now;
        }

    public class fiscalYearModel
    {
        [Key]
        public int fiscalYearID { get; set; }

        [Required]
        public int year { get; set; }

        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }

        public mainStatus status { get; set; } = mainStatus.Active;

        public bool isClosed { get; set; } = false;

        public DateTime? closedDate { get; set; }

        public string? closedBy { get; set; }

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<accountingPeriodModel> Periods { get; set; }
            = new List<accountingPeriodModel>();
    }
    public class accountingPeriodModel
    {
        [Key]
        public int accountingPeriodID { get; set; }

        [Required]
        public int fiscalYearID { get; set; }

        public virtual fiscalYearModel? FiscalYear { get; set; }

        [Required]
        public int periodNumber { get; set; }

        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }

        public bool isOpen { get; set; } = true;

        public DateTime? closedDate { get; set; }

        public string? closedBy { get; set; }

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<JournalEntry> JournalEntries { get; set; }
            = new List<JournalEntry>();
    }

    public class bankAccountModel
    {
        [Key]
        public int bankAccountID { get; set; }

        [Required]
        public int companyID { get; set; }

        public virtual companyModel? Company { get; set; }

        [Required]
        [MaxLength(150)]
        public string bankName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string accountName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string bankAccountNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? branchCode { get; set; }

        [MaxLength(100)]
        public string? branchName { get; set; }

        [Required]
        [MaxLength(10)]
        public string currencyCode { get; set; } = "ETB";

        // GL account associated with this bank account
        [Required]
        public int accountID { get; set; }

        public virtual accountModel? Account { get; set; }

        // Existing subaccount/entity structure
        public int? subAccountID { get; set; }

        public virtual subAccountModel? SubAccount { get; set; }

        public mainStatus status { get; set; } = mainStatus.Active;

        public DateTime? openingDate { get; set; }

        public decimal openingBalance { get; set; } = 0;

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<bankTransactionModel> Transactions { get; set; }
            = new List<bankTransactionModel>();
    }

    public class cashAccountModel
    {
        [Key]
        public int cashAccountID { get; set; }

        [Required]
        public int companyID { get; set; }

        public virtual companyModel? Company { get; set; }

        [Required]
        [MaxLength(150)]
        public string cashAccountName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? description { get; set; }

        [Required]
        [MaxLength(10)]
        public string currencyCode { get; set; } = "ETB";

        // GL account
        [Required]
        public int accountID { get; set; }

        public virtual accountModel? Account { get; set; }

        // Optional existing entity/subaccount
        public int? subAccountID { get; set; }

        public virtual subAccountModel? SubAccount { get; set; }

        public mainStatus status { get; set; } = mainStatus.Active;

        public DateTime? openingDate { get; set; }

        public decimal openingBalance { get; set; } = 0;

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<cashTransactionModel> Transactions { get; set; }
            = new List<cashTransactionModel>();
    }

    public class bankTransactionModel
    {
        [Key]
        public int bankTransactionID { get; set; }

        [Required]
        public int bankAccountID { get; set; }

        public virtual bankAccountModel? BankAccount { get; set; }

        [Required]
        public DateTime transactionDate { get; set; }

        [Required]
        public CashBankTransactionType transactionType { get; set; }

        [Required]
        [MaxLength(100)]
        public string reference { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }

        [MaxLength(100)]
        public string? externalReference { get; set; }

        public CashBankTransactionStatus status { get; set; }
            = CashBankTransactionStatus.Draft;

        // Journal generated from this transaction
        public int? journalEntryID { get; set; }

        public virtual JournalEntry? JournalEntry { get; set; }

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;
    }

    public class cashTransactionModel
    {
        [Key]
        public int cashTransactionID { get; set; }

        [Required]
        public int cashAccountID { get; set; }

        public virtual cashAccountModel? CashAccount { get; set; }

        [Required]
        public DateTime transactionDate { get; set; }

        [Required]
        public CashBankTransactionType transactionType { get; set; }

        [Required]
        [MaxLength(100)]
        public string reference { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }

        [MaxLength(100)]
        public string? externalReference { get; set; }

        public CashBankTransactionStatus status { get; set; }
            = CashBankTransactionStatus.Draft;

        public int? journalEntryID { get; set; }

        public virtual JournalEntry? JournalEntry { get; set; }

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;
    }

    public class cashBankTransferModel
    {
        [Key]
        public int transferID { get; set; }

        [Required]
        public int companyID { get; set; }

        public virtual companyModel? Company { get; set; }

        [Required]
        public DateTime transferDate { get; set; }

        [Required]
        public CashBankTransferType transferType { get; set; }

        [Required]
        [MaxLength(100)]
        public string reference { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? description { get; set; }

        [Required]
        [MaxLength(10)]
        public string currencyCode { get; set; } = "ETB";

        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }

        // Source bank account
        public int? sourceBankAccountID { get; set; }

        public virtual bankAccountModel? SourceBankAccount { get; set; }

        // Destination bank account
        public int? destinationBankAccountID { get; set; }

        public virtual bankAccountModel? DestinationBankAccount { get; set; }

        // Source cash account
        public int? sourceCashAccountID { get; set; }

        public virtual cashAccountModel? SourceCashAccount { get; set; }

        // Destination cash account
        public int? destinationCashAccountID { get; set; }

        public virtual cashAccountModel? DestinationCashAccount { get; set; }

        // Global accounting period
        [Required]
        public int accountingPeriodID { get; set; }

        public virtual accountingPeriodModel? AccountingPeriod { get; set; }

        // Generated General Ledger journal
        public int? journalEntryID { get; set; }

        public virtual JournalEntry? JournalEntry { get; set; }

        public CashBankTransactionStatus status { get; set; }
            = CashBankTransactionStatus.Draft;

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;
    }
    public class bankReconciliationModel
    {
        [Key]
        public int bankReconciliationID { get; set; }

        [Required]
        public int bankAccountID { get; set; }

        public virtual bankAccountModel? BankAccount { get; set; }

        [Required]
        public DateTime statementStartDate { get; set; }

        [Required]
        public DateTime statementEndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal statementOpeningBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal statementClosingBalance { get; set; }

        [Required]
        public DateTime reconciliationDate { get; set; }

        public BankReconciliationStatus status { get; set; }
            = BankReconciliationStatus.Draft;

        [MaxLength(500)]
        public string? description { get; set; }

        public DateTime? completedDate { get; set; }

        public string? completedBy { get; set; }

        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public virtual ICollection<bankReconciliationLineModel> Lines { get; set; }
            = new List<bankReconciliationLineModel>();
    }

    public class bankReconciliationLineModel
    {
        [Key]
        public int bankReconciliationLineID { get; set; }

        [Required]
        public int bankReconciliationID { get; set; }

        public virtual bankReconciliationModel? BankReconciliation { get; set; }

        [Required]
        public DateTime transactionDate { get; set; }

        [MaxLength(100)]
        public string? statementReference { get; set; }

        [MaxLength(500)]
        public string? description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal debit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal credit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal statementBalance { get; set; }

        // Optional ERP transaction match
        public int? bankTransactionID { get; set; }

        public virtual bankTransactionModel? BankTransaction { get; set; }

        public bool isMatched { get; set; } = false;

        public DateTime? matchedDate { get; set; }

        public string? matchedBy { get; set; }

        [MaxLength(500)]
        public string? matchingNote { get; set; }
    }
}
