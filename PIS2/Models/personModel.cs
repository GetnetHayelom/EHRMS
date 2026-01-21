using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIS2.Models
{
    public class personModel
    {
        [Key]
        public int personID { get; set; }
        [StringLength(100,ErrorMessage = "First name cannot exceed 100 characters")]
        public string personFirstName { get; set; }
        [StringLength(100, ErrorMessage = "Fathers name cannot exceed 100 characters")]
        public string personFatherName { get; set; } 
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string? personLastName { get; set; }
        public DateTime personDoB { get; set; }
        public Gender personGender { get; set; }
        [StringLength(50, ErrorMessage = "Input too long")]
        public String? personIDType { get; set; }//kebelle ID, Passport, Driving license etc
        [StringLength(50, ErrorMessage = "Input too long")]
        public String? personIDNumber { get; set; }// Number of the provided identification card
        [RegularExpression(@"^\+?[0-9]\d{1,14}$", ErrorMessage = "Please enter a valid phone number.")]
        public String? personPhoneNumber { get; set; }
        public String? personRecordNumber {get; set; }
        public int? addressID { get; set; }
        public mainStatus personStatus { get; set; }
        public virtual addressModel? addressModel { get; set; }
        [EmailAddress(ErrorMessage ="Invalid emailaddress")]
        public string? personEmailAddress { get; set; }
        public string personFullName => $"{personFirstName} {personFatherName} {personLastName}";
        public int personsAge => DateTime.Now.Year - personDoB.Year;
        //Navigation Properties
        public virtual ICollection<bankInfoModel>? Banks { get; set; }
        public virtual ICollection<employmentModel>? Employments { get; set; }
        public virtual ICollection<personEducationLevelModel>? PersonEducationLevels { get; set; }
        public virtual ICollection<personHistoryModel>? PersonHistories { get; set; }
        public virtual ICollection<familyModel>? Families1 { get; set; } = new List<familyModel>();
        public virtual ICollection<familyModel>? Families2 { get; set; } = new List<familyModel>();
        public virtual ICollection<experienceModel>? Experiences { get; set; }
        public virtual ICollection<ApplicantModel>? Applicants {get; set;}
        public virtual ICollection<trainingSessionModel> TrainingSessions { get; set; }
        public virtual userModel? userModel { get; set; }
        public string modifiedBy { get; set; }
        public personModel()
        {

        }
        public personModel(string firstName, string fatherName, string endName, DateTime DoB, int address, String phone, Gender gender) 
        {
            personFirstName= firstName;
            personFatherName= fatherName;
            personLastName= endName;
            personDoB = DoB;
            personGender = gender;
            personPhoneNumber = phone;
            addressID = address;
        }
      
        public int personAge
        {
            get
            {
                int age;
                age = DateTime.Now.Year - personDoB.Year;
                if (personDoB.AddYears(age) > DateTime.Today)
                {
                    age--;
                }
                return age;
            }
        }
        public decimal getMonthlyEarning(int personID)
        {
            decimal monthlyEarning;
            decimal salary=0;
            decimal allowance=0;
            decimal overtime=0;
            monthlyEarning=salary + allowance + overtime;
            return monthlyEarning;
        }

        public void getExprience(int personID)
        {
            //List<employmentModel> employmentModels = new List<employmentModel>().Where(e => e.personID == personID);

        }
       
    }
    public class personHistoryModel
    {
        [Key]
        public int personHistoryID { get; set; }
        [Required]
        public int personID { get; set; }
        public virtual personModel personModel { get; set; }
        public string personFirstName { get; set; }
        public string personFatherName { get; set; }
        public string? personLastName { get; set; }
        public DateTime personDoB { get; set; }
        public Gender personGender { get; set; }
        public String? personIDType { get; set; }//kebelle ID, Passport, Driving license etc
        public String? personIDNumber { get; set; }// Number of the provided identification card
        public String? personPhoneNumber { get; set; }
        public String? personRecordNumber { get; set; }
        public int? addressID { get; set; }
        public virtual addressModel? addressModel { get; set; } 
        public string? personEmailAddress { get; set; }
        public string personFullName => $"{personFirstName} {personFatherName} {personLastName}";
        public int personsAge => DateTime.Now.Year - personDoB.Year;
        public virtual userModel? userModel { get; set; }
        public mainStatus personStatus { get; set; }
        public string modifiedBy { get; set; }
        public string modifiedDate { get; set; }
        public personHistoryModel() { }
    }
}
