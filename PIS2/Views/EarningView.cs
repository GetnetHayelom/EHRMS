using Microsoft.EntityFrameworkCore;

namespace PIS2.Views
{
    public class EarningView
    {
        public int empID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Company { get; set; }
        [Precision(18, 2)]
        public decimal Salary { get; set; }
        [Precision(18, 2)]
        public decimal Allowance { get; set; }

        public EarningView() { }
    }
}
