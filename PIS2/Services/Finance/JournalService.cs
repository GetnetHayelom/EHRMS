using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Finance;

namespace PIS2.Services.Finance
{
    public class JournalService
    {
        private readonly PISContext _context;
        private readonly ILogger<PayrollService> _logger;

        public JournalService(PISContext context, ILogger<PayrollService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> PostAsync(int journalID, string userName)
        {
            var journal = await _context.JournalEntries
                .Include(j => j.Lines)
                .Include(j => j.AccountingPeriod)
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.JournalEntryID == journalID);

            if (journal == null)
                return (false, "Journal entry was not found.");

            if (journal.Status == JournalStatus.Posted)
                return (false, "Journal entry is already posted.");

            if (journal.Status == JournalStatus.Reversed)
                return (false, "A reversed journal cannot be posted again.");

            if (journal.Lines == null || !journal.Lines.Any())
                return (false, "Journal entry must contain at least one line.");

            // Period validation
            var period = journal.AccountingPeriod;

            if (period == null)
                return (false, "Accounting period was not found.");

            if (!period.isOpen)
                return (false, "The accounting period is closed.");

            if (journal.EntryDate.Date < period.startDate.Date ||
                journal.EntryDate.Date > period.endDate.Date)
            {
                return (false,
                    "Journal entry date does not belong to the selected accounting period.");
            }

            // Amount validation
            foreach (var line in journal.Lines)
            {
                if (line.Debit < 0 || line.Credit < 0)
                    return (false, "Debit and credit amounts cannot be negative.");

                if (line.Debit > 0 && line.Credit > 0)
                    return (false,
                        $"Journal line {line.lineNumber} cannot contain both debit and credit.");

                if (line.Debit == 0 && line.Credit == 0)
                    return (false,
                        $"Journal line {line.lineNumber} must contain a debit or credit amount.");
            }

            decimal totalDebit = journal.Lines.Sum(x => x.Debit);
            decimal totalCredit = journal.Lines.Sum(x => x.Credit);

            if (totalDebit != totalCredit)
            {
                return (false,
                    $"Journal is not balanced. Debit: {totalDebit:N2}, Credit: {totalCredit:N2}.");
            }

            if (totalDebit <= 0)
                return (false, "Journal total must be greater than zero.");

            // Validate accounts
            foreach (var line in journal.Lines)
            {
                var companyAccountExists =
                    await _context.CompanyAccounts.AnyAsync(x =>
                        x.companyID == journal.companyID &&
                        x.accountID == line.accountID &&
                        x.isActive);

                if (!companyAccountExists)
                {
                    return (false,
                        $"Account {line.accountID} is not active for this company.");
                }

                var account = await _context.Set<accountModel>()
                    .FirstOrDefaultAsync(x => x.accountID == line.accountID);

                if (account == null)
                    return (false, $"Account {line.accountID} was not found.");

                if (account.accountStatus != mainStatus.Active)
                    return (false,
                        $"Account {account.accountNumber} is inactive.");
            }


            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // validate
                // Post
                journal.Status = JournalStatus.Posted;
                journal.modifiedBy = userName;
                journal.modifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payroll pay generation failed");
                throw;
            }

            return (true, "Journal posted successfully.");
        }
    }
}