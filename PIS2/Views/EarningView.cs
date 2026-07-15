using Microsoft.EntityFrameworkCore;
using PIS2.Enums;

namespace PIS2.Views
{
    public class EarningView
    {
        public int earningTypeID { get; set; }
        public int employmentID { get; set; }
        [Precision(18, 2)]
        public decimal earningAmount { get; set; }
        public mainStatus earningStatus { get; set; }
        public string earningTypeName { get; set; }
        public string givenID { get; set; }
        public string FullName { get; set; }
        public int departmentID { get; set; }
        public string departmentName { get; set; }
        public int companyID { get; set; }
        public string companyName { get; set; }
        public int jobID { get; set; }
        public string jobTitle { get; set; }

        public EarningView() { }
    }

    public class earningDetailsView 
    {
        public int earningTypeID { get; set; }
        public string earningTypeName { get; set; }
        public int earningID { get; set; }
        public bool isPayroll { get; set; }
        
        [Precision(18, 2)]
        public decimal earningAmount { get; set; }
        public mainStatus earningStatus { get; set; }
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public string FullName { get; set; }
        public int departmentID { get; set; }
        public string departmentName { get; set; }
        public int companyID { get; set; }
        public string companyName { get; set; }
        public int jobID { get; set; }
        public string jobTitle { get; set; }
    }


}
