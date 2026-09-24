using PIS2.Enums;
using PIS2.Models.Organization;
using PIS2.Models.Finance;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models.Supply
{
    public class supplierModel
    {
        [Key]
        public int supplierID { get; set; }

        // Company ownership
        [Required]
        public int companyID { get; set; }

        public virtual companyModel? Company { get; set; }

        // Supplier identification
        [Required]
        [MaxLength(50)]
        public string supplierCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string supplierName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? legalName { get; set; }

        [MaxLength(50)]
        public string? taxIdentificationNumber { get; set; }

        [MaxLength(50)]
        public string? tradeLicenseNumber { get; set; }

        // Contact information
        [MaxLength(100)]
        public string? contactPerson { get; set; }

        [MaxLength(50)]
        public string? phoneNumber { get; set; }

        [MaxLength(150)]
        public string? email { get; set; }

        [MaxLength(500)]
        public string? address { get; set; }

        [MaxLength(100)]
        public string? city { get; set; }

        [MaxLength(100)]
        public string? country { get; set; }

        // Banking information
        [MaxLength(150)]
        public string? bankName { get; set; }

        [MaxLength(100)]
        public string? bankAccountNumber { get; set; }

        [MaxLength(100)]
        public string? bankAccountName { get; set; }

        // Accounting configuration
        [Required]
        public int payableAccountID { get; set; }

        public virtual accountModel? PayableAccount { get; set; }

        // Supplier-specific subaccount
        public int? payableSubAccountID { get; set; }

        public virtual subAccountModel? PayableSubAccount { get; set; }

        // Payment terms
        public int paymentTermDays { get; set; } = 30;

        [Required]
        [MaxLength(10)]
        public string currencyCode { get; set; } = "ETB";

        // Status
        public mainStatus status { get; set; } = mainStatus.Active;

        // Optional opening balance
        [Column(TypeName = "decimal(18,2)")]
        public decimal openingBalance { get; set; } = 0;

        // Audit information
        [MaxLength(100)]
        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        // Relationships
        public virtual ICollection<supplierInvoiceModel> Invoices { get; set; }= new List<supplierInvoiceModel>();

        public virtual ICollection<supplierPaymentModel> Payments { get; set; }= new List<supplierPaymentModel>();
    }
    public class supplierInvoiceModel
    {
        [Key]
        public int supplierInvoiceID { get; set; }

        // Company
        [Required]
        public int companyID { get; set; }

        public virtual companyModel? Company { get; set; }

        // Supplier
        [Required]
        public int supplierID { get; set; }

        public virtual supplierModel? Supplier { get; set; }

        // Supplier's invoice number
        [Required]
        [MaxLength(100)]
        public string invoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime invoiceDate { get; set; }

        [Required]
        public DateTime dueDate { get; set; }

        // Accounting period
        [Required]
        public int accountingPeriodID { get; set; }

        public virtual accountingPeriodModel? AccountingPeriod { get; set; }

        // Currency
        [Required]
        [MaxLength(10)]
        public string currencyCode { get; set; } = "ETB";

        // Exchange rate against company base currency
        [Column(TypeName = "decimal(18,6)")]
        public decimal exchangeRate { get; set; } = 1;

        // Amounts
        [Column(TypeName = "decimal(18,2)")]
        public decimal subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal taxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal withholdingAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal otherCharges { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal totalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal payableAmount { get; set; }

        // Description
        [MaxLength(500)]
        public string? description { get; set; }

        // Workflow
        public SupplierInvoiceStatus status { get; set; }
            = SupplierInvoiceStatus.Draft;

        // General Ledger journal
        public int? journalEntryID { get; set; }

        public virtual JournalEntry? JournalEntry { get; set; }

        // Audit
        [MaxLength(100)]
        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        // Lines
        public virtual ICollection<supplierInvoiceLineModel> Lines { get; set; }
            = new List<supplierInvoiceLineModel>();

        // Payments
        public virtual ICollection<supplierPaymentAllocationModel> PaymentAllocations { get; set; }
            = new List<supplierPaymentAllocationModel>();
    }
    public class supplierInvoiceLineModel
    {
        [Key]
        public int supplierInvoiceLineID { get; set; }

        [Required]
        public int supplierInvoiceID { get; set; }

        public virtual supplierInvoiceModel? SupplierInvoice { get; set; }

        public int lineNumber { get; set; }

        [Required]
        [MaxLength(500)]
        public string description { get; set; } = string.Empty;

        // Debit account
        [Required]
        public int accountID { get; set; }

        public virtual accountModel? Account { get; set; }

        // Cost / expense / asset dimension
        [Required]
        public int subAccountID { get; set; }

        public virtual subAccountModel? SubAccount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal unitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal taxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal totalAmount { get; set; }
    }

    public class supplierPaymentModel
    {
        [Key]
        public int supplierPaymentID { get; set; }

        // Company
        [Required]
        public int companyID { get; set; }

        public virtual companyModel? Company { get; set; }

        // Supplier
        [Required]
        public int supplierID { get; set; }

        public virtual supplierModel? Supplier { get; set; }

        // Payment information
        [Required]
        public DateTime paymentDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string paymentReference { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? description { get; set; }

        [Required]
        [MaxLength(10)]
        public string currencyCode { get; set; } = "ETB";

        [Column(TypeName = "decimal(18,6)")]
        public decimal exchangeRate { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal amount { get; set; }

        // Accounting period
        [Required]
        public int accountingPeriodID { get; set; }

        public virtual accountingPeriodModel? AccountingPeriod { get; set; }

        // Payment source
        public int? bankAccountID { get; set; }

        public virtual bankAccountModel? BankAccount { get; set; }

        public int? cashAccountID { get; set; }

        public virtual cashAccountModel? CashAccount { get; set; }

        // GL Journal
        public int? journalEntryID { get; set; }

        public virtual JournalEntry? JournalEntry { get; set; }

        // Status
        public SupplierPaymentStatus status { get; set; }
            = SupplierPaymentStatus.Draft;

        // Audit
        [MaxLength(100)]
        public string modifiedBy { get; set; } = string.Empty;

        public DateTime modifiedDate { get; set; } = DateTime.Now;

        // Invoice allocations
        public virtual ICollection<supplierPaymentAllocationModel> Allocations { get; set; }
            = new List<supplierPaymentAllocationModel>();
    }
    public class supplierPaymentAllocationModel
    {
        [Key]
        public int supplierPaymentAllocationID { get; set; }

        [Required]
        public int supplierPaymentID { get; set; }

        public virtual supplierPaymentModel? SupplierPayment { get; set; }

        [Required]
        public int supplierInvoiceID { get; set; }

        public virtual supplierInvoiceModel? SupplierInvoice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal allocatedAmount { get; set; }

        [MaxLength(500)]
        public string? note { get; set; }
    }
}