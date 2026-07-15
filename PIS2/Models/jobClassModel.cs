using PIS2.Enums;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class jobClassModel //Electrician, Accountant
    {
        [Key] public int jobClassId { get; set; }
        [Required]
        public string jobClassName { get; set; }
        [Required]
        public string? jobClassDescription { get; set; }
        public mainStatus jobClassStatus { get; set; }
        public virtual ICollection<jobModel>? Jobs { get; set; }
        public virtual ICollection<evaluationTypeModel>? EvaluationTypes { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;

        public jobClassModel() { }
    }
}
