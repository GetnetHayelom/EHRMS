using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PIS2.Pages.Forms
{
    public class PISAccessModel : PageModel
    {
        // Employee and Business Information (Source 3)
        [BindProperty]
       
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [BindProperty]
        public string BusinessUnit { get; set; }

        [BindProperty]
        
        public string EmployeeID { get; set; }

        [BindProperty]
        
        public string EmployeeName { get; set; }

        [BindProperty]
        public string JobTitle { get; set; }

        [BindProperty]
        public string WorkLocation { get; set; }

        // PIS Access Request (Source 4)
        [BindProperty]
        
        public string PISRequired { get; set; } // Yes/No

        [BindProperty]
        public string PISUserID { get; set; }

        // Access Groups Selection (Source 5-8)
        [BindProperty]
        public List<string> AccessGroup { get; set; } = new List<string>(); // Captures multiple selected Group IDs

        // Biometric System Request (Source 8-9)
        [BindProperty]
        
        public string BiometricRequired { get; set; } // Yes/No

        [BindProperty]
        public string BiometricUserID { get; set; }

        // Sign-Off Information (Source 10-11)
        [BindProperty]
        public string DeptManagerName { get; set; }

        [BindProperty]
        public string DeptManagerSignature { get; set; }

        [BindProperty]
        public string RequestingUserName { get; set; }

        [BindProperty]
        public string RequestingUserSignature { get; set; }

        public void OnGet()
        {
            // Initialize Date on form load
            if (Date == DateTime.MinValue)
            {
                Date = DateTime.Today;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // If validation fails, return the user to the same page with error messages.
                return Page();
            }

            // --- Successful Form Submission Logic (e.g., Save to DB, Send Notification, etc.) ---

            // For now, just return to the page
            return Page();
        }
    }
}
