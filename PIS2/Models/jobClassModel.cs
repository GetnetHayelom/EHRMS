using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class jobClassModel //Electrician, Accountant
    {
        [Key] public int JobClassId { get; set; }
        [Required]
        public string JobClassName { get; set; }
        [Required]
        public string? JobClassDescription { get; set; }
        public mainStatus JobClasStatus { get; set; }

        public virtual ICollection<jobModel>? Jobs { get; set; }
        public string modifiedBy { get; set; }

        public jobClassModel() { }
    }
}
