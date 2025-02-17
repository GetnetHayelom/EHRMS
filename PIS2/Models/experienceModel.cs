using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PIS2.Models
{
    public class experienceModel
    {
        [Key] public int experienceId { get; set; }
  
        public int personID { get; set; }
        public virtual personModel? personModel { get; set; }
        [Required]
        public DateTime experienceStartDate { get; set; }
        [AllowNull]
        public DateTime experienceEndDate { get; set; }
        public int jobPlacementID { get; set; }
        public virtual jobPlacementModel? jobPlacementModel { get; set; }
        public experienceModel()
        {

        }

    }


}
