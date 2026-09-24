namespace PIS2.Enums
{
    
    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }

    public enum allowanceDuration
    {
        Unlimited,
        Limited
    }

    public enum BudgetStatus
    {
        HOLD,
        APPROVED,
        ACTIVE,
        CLOSED,
        DECLINED
    }
    public enum payrollStatus
    {
        PENDING = 0,
        APPROVED = 1,
        PROCESSED = 2,
        POSTED = 3,
        COMPLETED = 4,
        DISCARDED = 5
    }
    public enum deductionBase
    {
        SALARY,
        ALLOWANCE,
        NET,
        GROSS,
        NONE
    }
    public enum earningBase
    {
        SALARY,
        ALLOWANCE,
        NONE
    }
    public enum earningGroup
    {
        EARNING,
        COMPENSATION,
        BENEFIT
    }

    public enum CashBankTransactionType
    {
        Deposit = 1,
        Withdrawal = 2,
        Transfer = 3,
        BankCharge = 4,
        InterestIncome = 5,
        Adjustment = 6
    }
    public enum CashBankTransactionStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Completed = 3,
        Cancelled = 4,
        Reversed = 5
    }
    public enum CashBankTransferType
    {
        BankToBank = 1,
        CashToBank = 2,
        BankToCash = 3,
        CashToCash = 4
    }
    public enum BankReconciliationStatus
    {
        Draft = 0,
        InProgress = 1,
        Completed = 2,
        Reopened = 3
    }
}
