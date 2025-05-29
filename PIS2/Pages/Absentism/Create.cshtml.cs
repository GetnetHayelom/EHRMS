using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PIS2.Models;
using static System.Formats.Asn1.AsnWriter;

namespace PIS2.Pages.Absentism
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }


        [BindProperty]
        public List<personModel>? People { get; set; } = default!;
        [BindProperty]
        public List<employmentModel>? Employments { get; set; } = default!;
        [BindProperty]
        public employmentModel? Employment { get; set; } = default!;
        [BindProperty]
        public leaveModel leaveModel { get; set; } = default!;
        public personModel Person { get; set; }
        public IActionResult OnGet()
        {
            People = _context.Persons.ToList();
            Employments = _context.Employments.ToList();
            Person = new personModel();
            Employment = new employmentModel();
            ViewData["employmentID"] = new SelectList(_context.Employments, "employmentID", "givenID");
            var leaveTypes = _context.LeaveTypes.Where(lt => lt.leaveGroup == leaveGroup.Absentism && lt.leaveTypeStatus == mainStatus.Active).ToList();
            ViewData["leaveTypeID"] = new SelectList(leaveTypes, "leaveTypeID", "leaveTypeName");
            return Page();
        }

       
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Leaves.Add(leaveModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        [BindProperty]
        public string searchID { get; set; } = default!;
        [BindProperty]
        public string Department { get; set; } = default!;
        public async Task<IActionResult> OnPostSearchID()
        {
            Console.WriteLine($"Search ID: {searchID}");


            if (!string.IsNullOrEmpty(searchID))
            {


                Person = await _context.Persons.Where(p => p.Employments.Any(e => e.givenID == searchID))
                    .FirstOrDefaultAsync();

                if (Person == null)
                {
                    TempData["SuccessMessage"] = $"No employment found with employment ID {searchID}";
                }
                else
                {
                    TempData["PersonID"] = Person.personID;
                    Employment = await _context.Employments
                        .Include(e => e.Leaves)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.jobModel)
                        .Include(e => e.JobPlacements).ThenInclude(j => j.departmentModel)
                        .FirstOrDefaultAsync(e => e.personID == Person.personID && e.employmentStatus == mainStatus.Active);
                    if (Employment == null)
                    {
                        TempData["SuccessMessage"] = $"No active employment found with employment ID {searchID}";
                        return NotFound();
                        
                    }

                }

            }
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();

            return Page();
        }
        [BindProperty]
        public string searchName { get; set; } = default!;
        public async Task<IActionResult> OnPostSearchName()
        {

            if (!searchName.IsNullOrEmpty())
            {

                Person = await _context.Persons
                    .FirstOrDefaultAsync(p => (p.personFirstName + " " + p.personFatherName + " " + p.personLastName).Contains(searchName));

                if (Person == null)
                {
                    TempData["SuccessMessage"] = $"No person found with Name {searchName}";
                }
                else
                {                   
                    TempData["PersonID"] = Person.personID;

                    TempData["PersonID"] = Person.personID;
                    Employment = await _context.Employments
                        .Include(e => e.Leaves)?
                        .Include(e => e.JobPlacements)?.ThenInclude(j => j.jobModel)?
                        .Include(e => e.JobPlacements)?.ThenInclude(j => j.departmentModel)?
                        .FirstOrDefaultAsync(e => e.personID == Person.personID && e.employmentStatus == mainStatus.Active);
                    if (Employment == null)
                    {
                        TempData["SuccessMessage"] = $"No active employment found with employment ID {searchID}";
                        return NotFound();
                    }

                }
               
            }
            People = await _context.Persons.ToListAsync();
            Employments = await _context.Employments.ToListAsync();

            return Page();
        }
    }
}
