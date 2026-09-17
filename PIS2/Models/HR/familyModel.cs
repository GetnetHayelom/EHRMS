using System.ComponentModel.DataAnnotations;
using PIS2.Enums;
using PIS2.Models.Foundation;

namespace PIS2.Models
{
    public class familyModel
    {
        [Key]
        public int familyID { get; set; }
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; }
        public int personID2 { get; set; }
        public virtual personModel? personModel2 { get; set; }
        public familyRelation relation { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public familyModel() { }
    }

    
}
