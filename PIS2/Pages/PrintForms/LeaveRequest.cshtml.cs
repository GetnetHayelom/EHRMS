using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS2.Pages.PrintForms
{
    public class LeaveRequestModel : PageModel
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }
        public double LeaveDays { get; set; }
        public string Reason { get; set; }

        public void OnGet()
        {
            // Example data for testing
            Name = "John Doe";
            Position = "Software Engineer";
            LeaveStartDate = DateTime.Now.AddDays(2);
            LeaveEndDate = DateTime.Now.AddDays(5);
            LeaveDays = 3;
            Reason = "Family emergency";
        }
    }
}
