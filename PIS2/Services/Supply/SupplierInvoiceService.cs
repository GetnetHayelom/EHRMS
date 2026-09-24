using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Finance;
using PIS2.Models.Supply;

namespace PIS2.Services.Supply
{
    public class SupplierInvoiceService
    {
        private readonly PISContext _context;

        public SupplierInvoiceService(PISContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, int? JournalEntryID)>
            PostInvoiceAsync(int supplierInvoiceID, string userName)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var invoice = await _context.SupplierInvoices
                    .Include(i => i.Supplier)
                    .Include(i => i.Lines)
                        .ThenInclude(l => l.Account)
                    .Include(i => i.Lines)
                        .ThenInclude(l => l.SubAccount)
                    .Include(i => i.AccountingPeriod)
                    .Include(i => i.JournalEntry)
                    .FirstOrDefaultAsync(i =>
                        i.supplierInvoiceID == supplierInvoiceID);

                if (invoice == null)
                    return (false, "Supplier invoice was not found.", null);

                // Already posted
                if (invoice.journalEntryID.HasValue)
                {
                    return (
                        false,
                        "This supplier invoice has already been posted.",
                        invoice.journalEntryID
                    );
                }

                if (invoice.Supplier == null)
                    return (false, "Supplier information is missing.", null);

                if (invoice.AccountingPeriod == null)
                    return (false, "Accounting period is missing.", null);

                // Only approved invoices can be posted
                if (invoice.status != SupplierInvoiceStatus.Approved)
                {
                    return (
                        false,
                        "Only approved supplier invoices can be posted.",
                        null
                    );
                }

                // Period must be open
                if (!invoice.AccountingPeriod.isOpen)
                {
                    return (
                        false,
                        "The accounting period is closed.",
                        null
                    );
                }

                // Invoice date must belong to the period
                if (invoice.invoiceDate.Date <
                    invoice.AccountingPeriod.startDate.Date ||
                    invoice.invoiceDate.Date >
                    invoice.AccountingPeriod.endDate.Date)
                {
                    return (
                        false,
                        "The invoice date does not belong to the selected accounting period.",
                        null
                    );
                }

                if (invoice.Lines == null || !invoice.Lines.Any())
                {
                    return (
                        false,
                        "The supplier invoice has no invoice lines.",
                        null
                    );
                }

                if (invoice.Supplier.payableAccountID <= 0)
                {
                    return (
                        false,
                        "The supplier does not have a payable account configured.",
                        null
                    );
                }

                // ---------------------------------------------------------
                // Validate invoice lines
                // ---------------------------------------------------------

                decimal lineTotal = 0;

                foreach (var line in invoice.Lines)
                {
                    if (line.Account == null)
                    {
                        return (
                            false,
                            $"Account is missing on invoice line {line.lineNumber}.",
                            null
                        );
                    }

                    if (line.SubAccount == null)
                    {
                        return (
                            false,
                            $"Subaccount is missing on invoice line {line.lineNumber}.",
                            null
                        );
                    }

                    if (line.Account.accountStatus != mainStatus.Active)
                    {
                        return (
                            false,
                            $"Account '{line.Account.accountName}' is inactive.",
                            null
                        );
                    }

                    if (line.SubAccount.subAccountStatus != mainStatus.Active)
                    {
                        return (
                            false,
                            $"Subaccount '{line.SubAccount.subAccountName}' is inactive.",
                            null
                        );
                    }

                    if (line.amount < 0)
                    {
                        return (
                            false,
                            $"Invoice line {line.lineNumber} has an invalid amount.",
                            null
                        );
                    }

                    if (line.taxAmount < 0)
                    {
                        return (
                            false,
                            $"Invoice line {line.lineNumber} has an invalid tax amount.",
                            null
                        );
                    }

                    lineTotal += line.amount + line.taxAmount;
                }

                // ---------------------------------------------------------
                // Validate invoice total
                // ---------------------------------------------------------

                if (lineTotal <= 0)
                {
                    return (
                        false,
                        "The supplier invoice total must be greater than zero.",
                        null
                    );
                }

                if (Math.Abs(lineTotal - invoice.totalAmount) > 0.01m)
                {
                    return (
                        false,
                        "Invoice line totals do not match the invoice total.",
                        null
                    );
                }

                // ---------------------------------------------------------
                // Validate supplier payable account
                // ---------------------------------------------------------

                var payableAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.accountID == invoice.Supplier.payableAccountID);

                if (payableAccount == null)
                {
                    return (
                        false,
                        "The supplier payable account was not found.",
                        null
                    );
                }

                if (payableAccount.accountStatus != mainStatus.Active)
                {
                    return (
                        false,
                        "The supplier payable account is inactive.",
                        null
                    );
                }

                // ---------------------------------------------------------
                // Validate supplier payable subaccount
                // ---------------------------------------------------------

                subAccountModel? payableSubAccount = null;

                
                    payableSubAccount =
                        await _context.SubAccounts
                            .FirstOrDefaultAsync(s =>
                                s.subAccountID ==
                                invoice.Supplier.payableSubAccountID.Value);

                    if (payableSubAccount == null)
                    {
                        return (
                            false,
                            "The supplier payable subaccount was not found.",
                            null
                        );
                    }

                    if (payableSubAccount.subAccountStatus != mainStatus.Active)
                    {
                        return (
                            false,
                            "The supplier payable subaccount is inactive.",
                            null
                        );
                    }

                    if (payableSubAccount.accountID !=
                        payableAccount.accountID)
                    {
                        return (
                            false,
                            "The supplier payable subaccount does not belong to the payable account.",
                            null
                        );
                    }
                

                // ---------------------------------------------------------
                // Create GL Journal
                // ---------------------------------------------------------

                var journal = new JournalEntry
                {
                    companyID = invoice.companyID,

                    EntryDate = invoice.invoiceDate,

                    accountingPeriodID = invoice.accountingPeriodID,

                    Reference = invoice.invoiceNumber,

                    Description = $"Supplier invoice - {invoice.Supplier.supplierName}",

                    Status = JournalStatus.Posted,

                    SourceModule = "Supply",

                    SourceReference = $"SUPINV-{invoice.supplierInvoiceID}",

                    modifiedBy = userName,

                    modifiedDate = DateTime.Now
                };

                int lineNumber = 1;

                // ---------------------------------------------------------
                // Debit invoice lines
                // ---------------------------------------------------------

                foreach (var line in invoice.Lines)
                {
                    var amount = line.amount + line.taxAmount;

                    if (amount <= 0)
                        continue;

                    journal.Lines.Add(new JournalEntryLine
                    {
                        lineNumber = lineNumber++,

                        accountID = line.accountID,

                        subAccountID = line.subAccountID,

                        Debit = amount,

                        Credit = 0,

                        Description = line.description
                    });
                }

                // ---------------------------------------------------------
                // Credit Accounts Payable
                // ---------------------------------------------------------

                journal.Lines.Add(new JournalEntryLine
                {
                    lineNumber = lineNumber,

                    accountID = payableAccount.accountID,

                    subAccountID = invoice.Supplier.payableSubAccountID,

                    Debit = 0,

                    Credit = invoice.totalAmount,

                    Description = $"Accounts payable - {invoice.Supplier.supplierName}"
                });

                // ---------------------------------------------------------
                // Final journal validation
                // ---------------------------------------------------------

                var totalDebit = journal.Lines.Sum(l => l.Debit);

                var totalCredit = journal.Lines.Sum(l => l.Credit);

                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    return (
                        false,
                        "The generated journal entry is not balanced.",
                        null
                    );
                }

                if (totalDebit <= 0)
                {
                    return (
                        false,
                        "The generated journal entry has no accounting value.",
                        null
                    );
                }

                // ---------------------------------------------------------
                // Save
                // ---------------------------------------------------------

                _context.JournalEntries.Add(journal);

                await _context.SaveChangesAsync();

                invoice.journalEntryID = journal.JournalEntryID;

                // Once posted, invoice is initially unpaid
                invoice.status = SupplierInvoiceStatus.Approved;

                invoice.modifiedBy = userName;
                invoice.modifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return (
                    true,
                    "Supplier invoice posted successfully.",
                    journal.JournalEntryID
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return (
                    false,
                    $"Unable to post supplier invoice: {ex.Message}",
                    null
                );
            }
        }
    }
}