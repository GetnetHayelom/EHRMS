using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class employmentModel : IValidatableObject
    {
        [Key]
        [Display(Name = "Employment Record ID")]
        public int employmentID { get; set; }
        [Required]
        [Display(Name ="Employee ID")]
        public string givenID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; } = default!;
        [Required]
        [Display(Name = "Date of Employment")]
        public DateTime employmentDate { get; set; } = DateTime.Now;
        [Display(Name = "Employment Termination Date")]
        public DateTime? employmentTerminationDate { get; set; }
        [Required]
        [Display(Name = "Employment Status")]
        public mainStatus employmentStatus { get; set; } = mainStatus.Active;
        [Display(Name = "Employment Refrence No.")]
        public string? employmentReference { get; set; }
        [Display(Name = "Working Hours per Week")]
        public int? workingHoursPerWeek { get; set; }
        [Display(Name = "Carried Over Leave(from previous employment)")]
        public double employmentCarriedOverLeave { get; set; } = 0;
        [Required]
        [Display(Name = "Employment Type")]
        public int employmentTypeID { get; set; }
        public virtual employmentTypeModel? employmentTypeModel { get; set; }
        [Display(Name = "Employment Method")]
        public int? employmentMethodID { get; set; }
        public virtual employmentMethodModel? employmentMethodModel { get; set; }
        [Display(Name = "Associated Employment Request")]
        public int? jobRequirementID { get; set; }
        public virtual jobRequirementModel? JobRequirementModel { get; set; }
        [Display(Name = "Employment Position")]
        public EmploymentPositions employmentPosition { get; set; }
        public virtual contractModel? contractModel { get; set; }
        public virtual terminationModel? TerminationModel { get; set; }
        public virtual ICollection<overtimeRecordModel>? OvertimeRecords { get; set; }
        public virtual ICollection<jobPlacementModel>? JobPlacements { get; set; }
        public virtual ICollection<leaveModel>? Leaves { get; set; }
        public virtual ICollection<allowanceAssignmentModel>? AllowanceAssignments { get; set; }
        public virtual ICollection<employmentHistoryModel>? EmploymentHistories { get; set; }
        public virtual ICollection<loyaltyHistoryModel>? LoyaltyHistories { get; set; }
        public virtual ICollection<workSiteModel>? WorkSites { get; set; }
        public virtual ICollection<guarantyModel>? Guaranties { get; set; }
        public virtual ICollection<serviceRequestModel>? ServiceRequests { get; set; }
        public virtual ICollection<prohibitionModel>? Prohibitions { get; set; }
        public virtual ICollection<delegationModel>? delegationsFrom { get; set; }
        public virtual ICollection<delegationModel>? delegationsTo { get; set; }
        public virtual ICollection<penaltyModel>? Penalties { get; set; }
        public virtual ICollection<shiftAssignmentModel>? ShiftAssignments { get; set; }
        public virtual ICollection<siteAssignmentModel>? SiteAssignments { get; set; }
        public virtual ICollection<earningModel>? Earnings { get; set; }
        public virtual ICollection<deductionModel>? Deductions { get; set; }
        public virtual ICollection<trainingAttendanceModel>? TrainingAttendaces { get; set; }
        public virtual ICollection<departmentModel>? departmentModel { get; set; }
        public virtual ICollection<companyModel>? companyModel { get; set; }
        public virtual ICollection<evaluationModel>? Evaluations { get; set; }
        
        public string modifiedBy { get; set; }
        public employmentModel() { }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (PISContext)validationContext.GetService(typeof(PISContext));
            personModel person = _context.Persons.FirstOrDefault(r => r.personID == personID);
            employmentTypeModel employmentType = _context.EmploymentTypes.FirstOrDefault(e => e.employmentTypeID == employmentTypeID);
            // Explicitly load the related model
            if (person == null)
            {
                yield return new ValidationResult("Person Not Found.");
            }
            else if (_context.Employments.Any(e => e.personID == personID && e.employmentStatus == mainStatus.Active))
            {
                yield return new ValidationResult("Person can not have more than one active employments.");
            }
            else if (person.personDoB > employmentDate)
            {
                yield return new ValidationResult("Date of birth can not be latest than employment date");
            }
            else if (person.personsAge < employmentType.employmentMinAge)
            {
                yield return new ValidationResult("Employee is under age for selected employment type");
            }
            else if (person.personsAge > employmentType.employmentMaxAge)
            {
                yield return new ValidationResult("Employee is under age for selected employment type");
            }

        }

    }
    public class employmentHistoryModel
    {
        [Key]
        public int employmentHistoryID { get; set; }
        [Display(Name = "Employment Record ID")]
        public int employmentID { get; set; }
        [Display(Name = "Employee ID")]
        public string givenID { get; set; }
        public virtual employmentModel? employmentModel { get; set; } = default!;
        [Display(Name = "Date Modified")]
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        [Display(Name = "Employment Type")]
        public int employmentTypeID { get; set; }
        public virtual employmentTypeModel? employmentTypeModel { get; set; }
        [Display(Name = "Employment Position")]
        public string employmentPosition { get; set; }
        [Display(Name = "Employment Status")]
        public mainStatus employmentStatus { get; set; } = mainStatus.Active;
        [Display(Name = "Remark")]
        public string? employmentHistoryRemark { get; set; }
        [Display(Name = "Modified By")]
        public string modifiedBy { get; set; }
        public void validateAge()
        {
            int age = 0;
            age = DateTime.Now.Year - employmentModel.personModel.personDoB.Year;
            if (age < employmentTypeModel.employmentMinAge || age > employmentTypeModel.employmentMaxAge)
            {
                throw new InvalidOperationException($"Age must be between {employmentTypeModel.employmentMinAge} and {employmentTypeModel.employmentMaxAge}!");
            }
        }

        public employmentHistoryModel()
        {

        }
    }
    public class employmentMethodModel //indicates how the employment is made, exam, transfer, by letter
    {
        [Key]
        [Display(Name = "Method ID")]
        public int employmentMethodID { get; set; }
        [Display(Name = "Employment Method Name")]
        public string employmentMethodName { get; set; }
        [Display(Name = "Employment Method Description")]
        public string employmentMethodDescription { get; set; }
        [Display(Name = "Employment Method Status")]
        public mainStatus employmentMethodStatus { get; set; } = mainStatus.Active;
        public virtual ICollection<employmentModel>? Employments { get; set; }
        public virtual ICollection<employmentMethodHistoryModel>? EmploymentMethodHistories { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? modifiedDate { get; set; }
        public virtual ICollection<VacancyModel>? Vacancies { get; set; }
        [Display(Name = "Modified By")]
        public string modifiedBy { get; set; }
        public employmentMethodModel(){}
    }
    public class employmentMethodHistoryModel 
    {
        [Key]
        [Display(Name = "Employment Method History Record ID")]
        public int employmentMethodHistoryID { get; set; }
        [Display(Name = "Employment Method")]
        public int employmentMethodID { get; set; }
        public virtual employmentMethodModel employmentMethodModel { get; set; }
        [Display(Name = "Employment Method Name")]
        public string employmentMethodName { get; set; }
        [Display(Name = "Employment Method Description")]
        public string employmentMethodDescription { get; set; }
        [Display(Name = "Employment Method Status")]
        public mainStatus employmentMethodStatus { get; set; } = mainStatus.Active;
        [Display(Name = "Date Modified")]
        public DateTime modifiedDate { get; set; }
        [Display(Name = "Modified By")]
        public string modifiedBy { get; set; }
        public employmentMethodHistoryModel(){}
    }

    public enum EmploymentPositions
    {        
        Non_Management,
        Management
    }
}

