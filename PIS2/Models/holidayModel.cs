using PIS2.Enums;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class holidayModel
    {
        [Key]
        public int holidayID { get; set; }
        [Required]
        public string holidayName { get; set; }
        [Required]
        public string holidayType { get; set; } //national, religious
        [Required]
        public DateTime holidayStart { get; set; }
        public DateTime holidayEnd { get; set; }
        public string holidayCycle { get; set; }
        public mainStatus holidayStatus { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public holidayModel() { }
    }
}
