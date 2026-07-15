using PIS2.Data;
using PIS2.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using PIS2.Enums;

namespace PIS2.Services
{
    public class Global_S
    {
        private readonly PISContext _db;
        public Global_S(PISContext db) { _db = db; }

        private int? _pensionDedType;
        private int? _OtEarningType;
        private int? _AllowanceEarningType;
        private int? _SalaryEarningType;
        private int? _TaxDeductionType;
        private int? _SeveranceType;
        private int? _LeavePayType;
        private int? _ExitPayType;
        private int? _DefaultSubAccount;
        private int? _PaidLeaveID;
        private List<taxRateModel> _TaxRates;
        public int pensionDedType => _pensionDedType ??= _db.DeductionTypes.AsNoTracking().Where(d => EF.Functions.Like(d.deductionCode, Global_C.PENSION_TYPE_CODE)).Select(d => (int?)d.deductionTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Pension deduction type not configured!");
        public int OtEarningType => _OtEarningType ??= _db.EarningTypes.AsNoTracking().Where(d => EF.Functions.Like(d.earningTypeCode, Global_C.OVERTIME_PAY_CODE)).Select(d => (int?)d.earningTypeID).FirstOrDefault() ?? throw new InvalidOperationException("OT earning type not configured!");
        public int AllowanceEarningType => _AllowanceEarningType ??= _db.EarningTypes.AsNoTracking().Where(d => EF.Functions.Like(d.earningTypeCode, Global_C.ALLOWANCE_TYPE_CODE)).Select(d => (int?)d.earningTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Allowance earning type not configured!");
        public int SalaryEarningType => _SalaryEarningType ??= _db.EarningTypes.AsNoTracking().Where(d => EF.Functions.Like(d.earningTypeCode, Global_C.SALARY_PAY_CODE)).Select(e => (int?)e.earningTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Salary earning type not configured!");
        public int TaxDeductionType => _TaxDeductionType ??= _db.DeductionTypes.AsNoTracking().Where(d => EF.Functions.Like(d.deductionCode, Global_C.TAX_PAY_CODE)).Select(e => (int?)e.deductionTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Income-tax deduction type not configured!");
        public int SeveranceType => _SeveranceType ??= _db.EarningTypes.AsNoTracking().Where(e => EF.Functions.Like(e.earningTypeCode, Global_C.SEVERANCE_PAY_CODE)).Select(e => (int?)e.earningTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Severance earning type not configured!");
        public int LeavePayType => _LeavePayType ??= _db.EarningTypes.AsNoTracking().Where(e => EF.Functions.Like(e.earningTypeCode, Global_C.LEAVE_PAY_CODE)).Select(e => (int?)e.earningTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Leave pay earning type not configured!");
        public int ExitPayType => _ExitPayType ??= _db.EarningTypes.AsNoTracking().Where(e => EF.Functions.Like(e.earningTypeCode, Global_C.EXIT_PAY_CODE)).Select(e => (int?)e.earningTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Exit pay earning type not configured!");
        public int DefaultSubAccount => _DefaultSubAccount ??= _db.SubAccounts.AsNoTracking().Where(e => EF.Functions.Like(e.subAccountName, Global_C.DEFAULT_SUBACCOUNT_NAME)).Select(e => (int?)e.subAccountID).FirstOrDefault() ?? throw new InvalidOperationException("Default sub account not configured!");
        public int PaidLeaveID => _PaidLeaveID ??=  _db.LeaveTypes.AsNoTracking().Where(e => EF.Functions.Like(e.leaveTypeName, Global_C.PAID_LEAVE_NAME)).Select(l => (int?)l.leaveTypeID).FirstOrDefault() ?? throw new InvalidOperationException("Paid leave type not configured!");
        public List<taxRateModel> TaxRates => _TaxRates ??= _db.TaxRates.AsNoTracking().Where(t => t.taxStatus == mainStatus.Active).ToList();

    }
    
}
