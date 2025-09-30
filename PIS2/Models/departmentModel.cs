using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class departmentModel
    {
        [Key]
        public int departmentID { get; set; }
        public string? departmentShort { get; set; }
        [Required(ErrorMessage = "Department name can not be empty.")]
        public string departmentName { get; set; }
        public mainStatus departmentStatus { get; set; }      
        public int? subAccountID { get; set; }
        public virtual subAccountModel? subAccountModel { get; set; } = null!;
        public int companyID { get; set; }
        public virtual companyModel? companyModel { get; set; } = null!;
        //Manager
        public int? employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public int? businessUnitID { get; set; }
        public virtual businessUnitModel? businessUnitModel { get; set; }
        public ICollection<jobPlacementModel>? JobPlacements { get; set; } = null!;
        public ICollection<jobPlacementHistoryModel>? JobPlacementHistories { get; set; } = null!;
        public virtual ICollection<jobRequirementModel>? JobRequirements { get; set; } = null!;
        public virtual ICollection<overtimeRecordModel>? OvertimeRecords { get; set; } = null!;
        public virtual ICollection<departmentHistoryModel>? DepartmentHistories { get; set; } = null!;
        public virtual ICollection<structureModel>? Structures { get; set; } = null!;
        public string modifiedBy { get; set; }
        public departmentModel() { }
    }

    public class departmentHistoryModel
    {
        [Key]
        public int departmentHistoryID { get; set; }
        public int departmentID { get; set; }
        public virtual departmentModel? departmentModel { get; } = null!;
        public string departmentName { get; set; }
        public mainStatus departmentStatus { get; set; }
        public int? employmentID { get; set; }
        public virtual employmentModel? employmentModel { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public departmentHistoryModel() { }
    }
}
