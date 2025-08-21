using System.ComponentModel.DataAnnotations;

namespace PIS2.Models
{
    public class familyModel
    {
        [Key]
        public int familyID { get; set; }
        public int personID { get; set; }
        public virtual personModel? personModel { get; private set; }
        public int personID2 { get; set; }
        public virtual personModel? personModel2 { get; private set; }
        public familyRelation relation { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public familyModel() { }
    }

    public enum familyRelation
    {
        Father,
        Mother,
        Son,
        Daughter,
        Sister,
        Brother,
        GrandFather,
        GrandMother,
        CareTaker,
        Uncle,
        Aunty,
        GrandSon,
        GrandDaughter
    }
}
