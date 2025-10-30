using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Pages.Forms
{
    public class SickReportModel : PageModel
    {
        

        
        [BindProperty]
        public string EmployeeName { get; set; } 

        [BindProperty]
        public string Sex { get; set; } 
        [BindProperty]
        public int? Age { get; set; } 

        [BindProperty]
        public string Address { get; set; } 

        
        [BindProperty]
        public string Department { get; set; } 

        [BindProperty]
        public string JobTitle { get; set; } 
        [BindProperty]
        [DataType(DataType.Time)]
        public DateTime? TimeSendToClinic { get; set; } 
        
        [BindProperty]
        public string InChargeDivisionName { get; set; } 

        [BindProperty]
        public DateTime? ReportDate { get; set; } 
        public string ClinicFeedBackTitle => "Clinic Feed Back";

        [BindProperty]
        public string Diagnosis { get; set; } 

        [BindProperty]
        [DataType(DataType.Time)]
        public DateTime? ReturnedTimeFromClinic { get; set; }
        [BindProperty]
        public string NameInChargeOfClinic { get; set; }

        [BindProperty]
        [DataType(DataType.Date)]
        public DateTime? ClinicDate { get; set; } 
        public void OnGet()
        {
            ReportDate = DateTime.Today;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            return Page();
        }
    }
}
