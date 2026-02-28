using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class letterModel
    {
        [Key]
        [Display(Name = "Letter ID")]
        public int letterID { get; set; }
        [Display(Name = "Letter Number")]
        public string? letterNumber { get; set; }
        [Display(Name = "Letter Title")]
        public string? letterTitle { get; set; }
        [Display(Name = "Letter Group")]
        public LetterGroup letterGroup { get; set; }// Incoming, Outgoing, Memo
        [Display(Name = "Letter Type")]
        public int letterTypeID { get; set; }
        public virtual letterTypeModel? LetterType { get; set; } = null!;
        [Display(Name = "Letter Type")]
        public string letterSubject { get; set; } = null!;
        [Display(Name = "Letter Sender")]
        public string letterSender { get; set; } = null!;
        [Display(Name = "Letter Reciever")]
        public string letterReceiver { get; set; } = null!;
        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Letter Body")]
        public string? letterBody { get; set; }
        [Display(Name = "Letter Parent")]
        public int? letterParent { get; set; }
        public virtual letterModel? LetterParent { get; set; }
        public virtual ICollection<letterModel>? ChildLetters { get; set; }
        public virtual archiveModel? Archive { get; set; }
        [Display(Name = "Letter Signed By")]
        public string? letterSignedBy { get; set; }
        [Display(Name = "Letter Date")]
        public DateTime letterDate { get; set; } = DateTime.Now;
        public LetterStatus letterStatus { get; set; } = LetterStatus.Draft;
        // Draft, Approved, Archived, Void
        public string modifiedBy { get; set; }
        public DateTime modifiedDate {  get; set; } = DateTime.Now;

        public letterModel() { }

    }
    public class letterTypeModel
    {
        [Key]
        [Display(Name ="Letter Type ID")]
        public int letterTypeID { get; set; }
        [Display(Name = "Letter Type Name")]
        public string letterTypeName { get; set; }
        [Display(Name = "Letter Type Description")]
        public string letterTypeDescription { get; set; } = "";
        [Display(Name = "Letter Type Code")]
        public string letterTypeCode { get; set; }
        [Display(Name = "Letter Type ID")]
        public string letterTypePrefix { get; set; } = "MME";
        [Display(Name = "Modified By")]
        public string modifiedBy { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime modifiedDate { get; set; } = DateTime.Now;
        public virtual ICollection<letterModel>? Letters { get; set; }

        public letterTypeModel() { }
    }
    public class archiveModel
    {
        [Key]
        public int archiveID { get; set; }
        public int letterID { get; set; }
        public virtual letterModel? Letter { get; set; }
        public string? recievedBy { get; set; }
        public DateTime archivedDate { get; set; }
        public string? remark { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; } = DateTime.Now;
    }
    public class LetterSequence
    {
        [Key]
        public int letterSequenceId { get; set; }
        public string letterTypePrefix { get; set; }
        public string letterTypeCode { get; set; }
        public LetterGroup letterGroup { get; set; }
        public int lastNumber { get; set; }
        public int Year { get; set; }

        public string GetFormated()
        {
            return $"{letterTypePrefix}/{letterGroup}/{letterTypeCode}/{lastNumber}/{Year}";
        }
    }

    public enum LetterGroup
    {
        Incoming,
        Outgoing,
        Internal
    }
    public enum LetterStatus
        {
            Draft,
            Approved,
            Archived,
            Void
        }
}
