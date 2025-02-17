using PIS2.Pages;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class addressModel
    {
        [Key]
        public int addressID { get; set; }
        [Required]
        public Country addressCountry { get; set; } = Country.Ethiopia;
        [Required]
        public string addressRegion { get; set; }
        [Required]
        public string addressZone { get; set; }
        [Required]
        public string addressWoreda { get; set; }
        [Required]
        public string addressTabya { get; set; }
        [Required]
        public mainStatus addressStatus { get; set; }
        public string addressFormatted => $"{addressTabya}, {addressWoreda}, {addressZone}, {addressRegion}, {addressCountry}";
        public string getAddressTitle()
        { 
            string addressTitle;
            addressTitle = addressCountry + ", " + addressRegion + ", " + addressZone + ", " +addressWoreda + ", " + addressTabya;
            return addressTitle;
        }
        //Navigation Property
        public virtual ICollection<personModel>? Persons { get; set; }
        public virtual ICollection<companyModel>? Companies { get; set; }
        public virtual ICollection<workSiteModel>? WorkSites { get; set; }
        //public virtual companyModel? companyModel { get; set; }
       
        public addressModel() { }
      
    }
}
