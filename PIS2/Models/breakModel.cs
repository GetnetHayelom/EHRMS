using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class breakModel //for a break with in a shift like lunch break
    {
        [Key]
        public int breakID { get; set; }     
        public int shiftID { get; set; }
        public virtual shiftModel? shiftModel { get; set; }
        public TimeSpan breakStart { get; set; }
        public TimeSpan breakEnd { get; set; }
        public string breakName { get; set; }
        public mainStatus breakStatus { get; set; }
        public breakModel() { }

    }
}
