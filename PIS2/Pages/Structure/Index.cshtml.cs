using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Organization;
using PIS2.Pages.Management;
using PIS2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Structure
{
    
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;
        public IndexModel(PISContext context) {_context = context;} 

        public ICollection<StructureView> Structures { get; set; } = new List<StructureView>();
        [BindProperty]
        public int? CompanyID { get; set; } = 0;
        public int? DepartmentID { get; set; } = 0;
        public List<companyModel> Companies { get; set; }
        
        public mainStatus? Status { get; set; }
        public async Task OnGetAsync(int? CompanyID, int? DepartmentID, mainStatus? Status)
        {
            Companies = _context.Companies.OrderBy(c => c.companyName).ToList();
            var structure = _context.StructureView.AsQueryable();

            this.CompanyID = CompanyID;
            this.DepartmentID = DepartmentID;
            this.Status = Status;

            if (CompanyID != null)
            {
                structure = structure
                    .Where(s => s.CompanyID == CompanyID);
            }
            if (DepartmentID != null)
            {
                
                structure =structure
                    .Where(s => s.DepartmentID == DepartmentID);
            }
            
            
             if (Status != null)
            {
                structure = structure
                    .Where(s => s.StructureStatus == (int) Status);
            }

            var filteredStruct = structure.ToList();

            Structures = filteredStruct;
                
        }
    }
}
