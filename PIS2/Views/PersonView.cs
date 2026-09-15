using PIS2.Enums;

namespace PIS2.Views
{
    public class PersonView
    {
        int personID { get; set; }
        string fullName { get; set; }
        decimal age { get; set; }
        string phoneNumber { get; set; }
        string address { get; set; }
        Gender gender { get; set; }
    }
}
