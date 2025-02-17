using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class bankInfoModel
    {
        [Key]
        public int bankInfoID { get; set; }
        [Required]
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; }
        [Required]
        public string bankName { get; set; }
        [Required]
        public string bankAccountNumber { get; set; }
        public string? bankBranch { get; set; }
        public mainStatus banikInfoStatus { get; set; }
        
        public bankInfoModel() { }
    }
}
