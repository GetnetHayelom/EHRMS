using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using PIS2.Models;

namespace PIS2.Views
{
    public class DepartmentView
    {
        private readonly PIS2.Models.PISContext _context;

        public DepartmentView(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        public departmentModel Department = new departmentModel();
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
        public DepartmentView(int id)
        {

            Department = _context.Departments.FirstOrDefault(d => d.departmentID == id);
            if (Department != null)
            {
                JobPlacements = _context.JobPlacements.Where(jp => jp.departmentID == id)
                    .Include(jp => jp.employmentModel).ThenInclude(e => e.employmentTypeModel)
                    .Include(jp => jp.employmentModel).ThenInclude(e => e.Leaves)
                    .ToList();
                if (JobPlacements.Any())
                {
                    Employments = JobPlacements
                        .Select(jp => jp.employmentModel)
                        .Where(e => e != null)
                        .ToList() ?? new List<employmentModel?>();
                    ExEmployments = Employments.Where(e => e.employmentStatus == mainStatus.Inactive).ToList();
                    ExContract = ExEmployments.Where(e => e.employmentTypeModel.employmentTypeName == "Contract").ToList();

                }
            }
            else
            {

            }

            Employments = _context.Employments.Where(e => e.JobPlacements.Any(jp => jp.departmentID == id))?.ToList();

            totalNoEmployment = Employments.Count;
            exEmployments = Employments.Where(e => e.employmentStatus == mainStatus.Inactive).Count();

            var empActive = Employments.Where(e => e.employmentStatus == mainStatus.Active).ToList();
            var empIds = empActive.Select(e => e.employmentID).ToList();
            var leaves = _context.Leaves.Where(l => empIds.Contains(l.employmentID)).ToList();
            var activeLeaves = leaves.Where(l => l.leaveStartDate <= DateTime.Now && l.leaveEndDate >= DateTime.Now).ToList();
            var empOnLeave = activeLeaves.Select(l => l.employmentID).ToList();
        }
        public DepartmentView()
        {
        }
    }
}
