using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Models;
using PIS2.Models.HR;
using PIS2.Models.Organization;
namespace PIS2.Views
{
    public class CompanyView
    {
        private readonly PISContext _context;

        public CompanyView(PISContext context)
        {
            _context = context;
        }
        public companyModel Company = new companyModel();
        public List<employmentModel> Employments { get; set; }
        public List<employmentModel> ExEmployments { get; set; }
        public List<employmentModel> ExContract { get; set; }
        public List<employmentModel> ExPermanent { get; set; }
        public List<employmentModel> OnLeaveEmployments { get; set; }
        public List<employmentModel> ActiveEmployments { get; set; }
        public List<employmentModel> ActiveContract { get; set; }
        public List<employmentModel> ActivePermanent { get; set; }
        public List<leaveModel> Leaves { get; set; }
        public List<jobPlacementModel> JobPlacements { get; set; }
        public int totalNoEmployment { get; set; }
        public int exEmployments { get; set; }
        public int leaveEmployments { get; set; }
        public int permanentEmployments { get; set; }
        public int contractEmployments { get; set; }
        public int contractEnding { get; set; }
        public int pensionEmployments { get; set; }
        public List<leaveModel> leaves { get; set; }
        public List<overtimeRecordModel> overtimeRecords { get; set; }
        public CompanyView(int id)
        {
            Company = _context.Companies.FirstOrDefault(c =>c.companyID == id);
        }
    }
}
