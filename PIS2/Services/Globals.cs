using PIS2.Data;
using PIS2.Models;
using Microsoft.EntityFrameworkCore;

namespace PIS2.Services
{
    public class Globals
    {
        private readonly PISContext _db;
        public Globals(PISContext db) { _db = db; }

        public static readonly decimal pensionEmployee = 0.07m;
        public static readonly decimal pensionEmployer = 0.11m;
        public static readonly  decimal avgWorkingHoursPerMonth = 208;
        public static readonly  decimal hoursPerDay = 8m;
        public static readonly  string defaultSubAccName = "3000000000000";
        public static readonly  string pensionTypeCode = "PENSIN";
        public static readonly  string OtTypeCode = "OT";
        public static readonly  string SalaryTypeCode = "SALARY";
        public static readonly  string AllowanceTypeCode = "TALLOW";
        public static readonly  string TaxTypeCode = "PAYE";
        public static readonly  string EMPLOYEE_CREDIT_ACCOUNT_NAME = "Account Payables-Employees";
        public static readonly  bool IsStrict = false;

        public int pensionDedType => _db.DeductionTypes.AsNoTracking().FirstOrDefault(d => d.deductionCode.ToLower() == pensionTypeCode.ToLower()).deductionTypeID;
        public int OtEarningType => _db.EarningTypes.AsNoTracking().FirstOrDefault(d => d.earningTypeCode.ToLower() == OtTypeCode.ToLower()).earningTypeID;
           
        public int AllowanceEarningType => _db.EarningTypes.AsNoTracking().FirstOrDefault(d => d.earningTypeCode.ToLower() == AllowanceTypeCode).earningTypeID;
        public int SalaryEarningType => _db.EarningTypes.AsNoTracking().FirstOrDefault(d => d.earningTypeCode.ToLower() == SalaryTypeCode.ToLower()).earningTypeID;
        public int TaxDeductionType => _db.DeductionTypes.FirstOrDefault(d => d.deductionCode.ToLower() == TaxTypeCode.ToLower()).deductionTypeID;

    }
}
