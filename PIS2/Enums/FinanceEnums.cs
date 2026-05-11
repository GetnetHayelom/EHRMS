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
}
