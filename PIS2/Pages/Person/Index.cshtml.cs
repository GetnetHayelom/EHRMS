using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Person
{
    public class IndexModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public IndexModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }

        public IList<personModel> personModel { get;set; } = default!;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1; 
        public int PageSize { get; set; } = 100;
        public int Employed { get; set; }
        public int xEmployed { get; set; }
        public async Task OnGetAsync(int id = 1 )
        {

            CurrentPage = id > 0? id:1;
            var totalPersons = await _context.Persons.CountAsync();
    
            TotalPages = (int)Math.Ceiling(totalPersons / (double)PageSize);

            //personModel = await _context.Persons
            //    .Include(p => p.addressModel).OrderBy(p => p.personFirstName).ThenBy(p => p.personFatherName).ThenBy(p => p.personLastName)
            //    .Skip((CurrentPage - 1) * PageSize)
            //    .Take(PageSize)
            //    .ToListAsync();
            personModel = await _context.Persons
                .Include(p => p.addressModel).OrderBy(p => p.personFirstName).ThenBy(p => p.personFatherName).ThenBy(p => p.personLastName)
                .ToListAsync();

            Employed = personModel.Count(p => _context.Employments.Where(e => e.employmentStatus== mainStatus.Active).Any(e => e.personID == p.personID));
            xEmployed = personModel.Count(p => _context.Employments.Where(e => e.employmentStatus == mainStatus.Inactive).Any(e => e.personID == p.personID));
        }
    }
}
