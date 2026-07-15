using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Views
{
    public class DepartmentView
    {
        private readonly PISContext _context;

        public DepartmentView(PISContext context)
        {
            _context = context;
        }
        /// <summary>
        /// The department
        /// </summary>
        public departmentModel Department = new departmentModel();
        /// <summary>
        /// Active Employments in the department
        /// </summary>
        public List<employmentModel> Employments { get; set; }
        /// <summary>
        /// ExEmployee from the department, based on their last job placement
        /// </summary>
        public List<employmentModel> ExEmployments { get; set; }
        public List<employmentModel> ExContract { get; set; }
        public List<employmentModel> ExPermanent { get; set; }
        /// <summary>
        /// Employees on leave
        /// </summary>
        public List<employmentModel> OnLeaveEmployments { get; set; }
        public List<employmentModel> ActiveEmployments { get; set; }
        public List<employmentModel> ActiveContract { get; set; }
        public List<employmentModel> ActivePermanent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<leaveModel> Leaves { get; set; }
        /// <summary>
        /// Job placements on the department
        /// </summary>
        public List<jobPlacementModel> JobPlacements { get; set; }
        public int totalNoEmployment { get; set; }
        public int exEmployments { get; set; }
        public int leaveEmployments { get; set; }
        public int permanentEmployments { get; set; }
        public int contractEmployments { get; set; }
        public int contractEnding { get; set; }
        public int pensionEmployments { get; set; }
        /// <summary>
        /// All the leaves from the department on hold
        /// </summary>
        public List<leaveModel> leaves { get; set; }
        /// <summary>
        /// Overtime records from the department
        /// </summary>
        public List<overtimeRecordModel> overtimeRecords { get; set; }
        /// <summary>
        /// Allowances in the department
        /// </summary>
        public List<allowanceAssignmentModel> allowanceAssignments { get; set; }
        public leaveDetail leaveSummary { get; set; }
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
                    ExContract = ExEmployments.Where(e => e.employmentTypeModel.employmentTypeName == "Contract" && e.employmentStatus == mainStatus.Inactive).ToList();

                }
            }
            else
            {

            }

            //Employments = _context.Employments.Where(e => e.JobPlacements.Any(jp => jp.departmentID == id))?.ToList();

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
