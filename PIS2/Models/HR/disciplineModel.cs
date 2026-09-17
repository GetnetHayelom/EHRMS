using System.ComponentModel.DataAnnotations;

namespace PIS2.Models.HR
{
    /// <summary>
    /// Discipline of education fields(Engineering, Health, Law, Accounting,Computer Science etc)
    /// </summary>
    public class disciplineModel
    {
        [Key]
        public int disciplineID { get; set; }
        [Required]
        [StringLength(100)]
        public string disciplineName { get; set; }
        [Required]
        [StringLength(100)]
        public string disciplineDescription { get; set; }
        public DateTime createdDate { get; set; }
        
        public string createdBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public string modifiedBy { get; set; }

        public disciplineModel() { }
    }
}
