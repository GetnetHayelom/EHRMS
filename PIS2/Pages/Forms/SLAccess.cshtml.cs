using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System.Collections.Generic;
namespace PIS2.Pages.Forms
{
    public class SLAccessModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        public SLAccessModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        // =======================================================
        // 1. USER AND BUSINESS UNIT INFORMATION
        // =======================================================

        [BindProperty]
        public DateTime RequestDate { get; set; }

        [BindProperty]
        public string Department { get; set; }

        [BindProperty]
        public string Company { get; set; }
        [BindProperty]
        public string EmployeeName { get; set; }

        [BindProperty]
        public string EmployeeID { get; set; }

        [BindProperty]
        public string JobTitle { get; set; }

        // =======================================================
        // 2. GENERAL ACCESS REQUIREMENTS
        // =======================================================

        // Windows Access
        [BindProperty]
        public string WindowsRequired { get; set; } // Yes/No

        [BindProperty]
        public string WindowsLoginName { get; set; }

        // Email Account Access
        [BindProperty]
        public string EmailRequired { get; set; } // Yes/No

        [BindProperty]
        public string EmailAccount { get; set; }

        [BindProperty]
        public string EmailGroup { get; set; }

        // Internet Access Time
        [BindProperty]
        public string InternetTime { get; set; } // Full Time/Half Day/2 Hours

        // =======================================================
        // 3. SL (SYSTEMLINK) ACCESS PRIVILEGES
        // The lists are used to bind to multiple selected checkboxes
        // =======================================================

        [BindProperty]
        public string SLRequired { get; set; } // Yes/No

        [BindProperty]
        public List<string> SLGroup { get; set; } = new List<string>(); // e.g., RQALL, AMDTC, AP, XH

        [BindProperty]
        public List<string> SLCustomization { get; set; } = new List<string>(); // e.g., DENTRY, AMDTC, Store

        // =======================================================
        // 4. USER AND MANAGER DECLARATIONS
        // =======================================================

        // Department Manager
        [BindProperty]
        public string ManagerName { get; set; }

        [BindProperty]
        public string ManagerSign { get; set; } // Placeholder for signature/digital sign

        // Requesting User
        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string UserSign { get; set; } // Placeholder for signature/digital sign

        // =======================================================
        // 5. ICT APPROVALS
        // =======================================================

        // Database Administrator (Prepared By)
        [BindProperty]
        public string DBAPreparedBy { get; set; }

        [BindProperty]
        public string DBASign { get; set; }

        // ICT/Division Manager (Checked By)
        [BindProperty]
        public string DivisionManagerCheckedBy { get; set; }

        [BindProperty]
        public string DivisionManagerSign { get; set; }

        // ICT Dept Manager (Approved By)
        [BindProperty]
        public string ICTDeptManagerApprovedBy { get; set; }

        [BindProperty]
        public string ICTDeptManagerSign { get; set; }

        // =======================================================
        // 6. TERMINATION SECTION
        // =======================================================

        [BindProperty]
        public DateTime? TerminationDate { get; set; } // Nullable date

        [BindProperty]
        public string TerminationReason { get; set; }

        // =======================================================
        // PAGE HANDLERS
        // =======================================================

        public void OnGet()
        {
            // Logic to run when the page is loaded (e.g., initial data setup)
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Handle validation errors, return to the form
                return Page();
            }

            // 1. Process the data (e.g., convert to a model object, save to database, or send an email)

            // Example: Display selected SL Groups
            // var selectedGroups = string.Join(", ", SLGroup);

            // 2. Redirect the user to a confirmation page
            return RedirectToPage("AccessRequestConfirmation");
        }
        
        
    }
}