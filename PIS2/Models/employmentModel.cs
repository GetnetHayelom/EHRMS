using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class employmentModel : IValidatableObject
    {
        [Key]
        public int employmentID { get; set; }
        [Required]
        public string givenID { get; set; }
        [Required]
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; } = default!;
        [Required]
        public DateTime employmentDate { get; set; } = DateTime.Now;
        public DateTime? employmentTerminationDate { get; set; }
        [Required]
        public mainStatus employmentStatus { get; set; } = mainStatus.Active;
        public string? employmentReference { get; set; }
        public int? workingHoursPerWeek  { get; set; }
        public double employmentCarriedOverLeave { get; set; } = 0;
        [Required]
        public int employmentTypeID { get; set; }
        public virtual employmentTypeModel? employmentTypeModel { get; set; }
        public virtual ICollection<overtimeRecordModel>? OvertimeRecords { get; set; }
        public virtual ICollection<jobPlacementModel>? JobPlacements { get; set; }
        public virtual ICollection<leaveModel>? Leaves { get; set; }
        public virtual ICollection<allowanceAssignmentModel>? AllowanceAssignments { get; set; }
        public virtual ICollection<employmentHistoryModel>? EmploymentHistories { get; set; }
        public virtual ICollection<loyaltyHistoryModel>? LoyaltyHistories { get; set; } 
        //public virtual departmentModel? departmentModel { get; set; }
        //public virtual companyModel? companyModel { get; set; }
        
        public employmentModel() { }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (PISContext)validationContext.GetService(typeof(PISContext));
            personModel person = _context.Persons.FirstOrDefault(r => r.personID == personID);
            employmentTypeModel employmentType = _context.EmploymentTypes.FirstOrDefault(e => e.employmentTypeID == employmentTypeID);
            // Explicitly load the related model
            if(person == null)
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
        public int employmentID { get; set; }
        public string givenID { get; set; }
        public virtual employmentModel? employmentModel { get; set; } = default!;
        public DateTime employmentHistoryDate { get; set; }
        public int employmentTypeID { get; set; }
        public virtual employmentTypeModel? employmentTypeModel { get; set; }
        public mainStatus employmentStatus { get; set; } = mainStatus.Active;
        public string? employmentHistoryRemark { get; set; }
        public string employmentHistoryUser {  get; set; }
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
}


