using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;

namespace PIS2.Pages.Person
{
    [Authorize(Roles ="HRCLERK, HRMANAGER, MANAGER")]
    public class IndexModel : PageModel
    {
        private readonly PISContext _context;

        public IndexModel(PISContext context)
        {
            _context = context;
        }

        public IList<personModel> personModel { get;set; } = default!;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1; 
        public int PageSize { get; set; } = 100;
        public int Employed { get; set; }
        public int xEmployed { get; set; }
        public async Task OnGetAsync(int id = 1)
        {
            CurrentPage = id > 0 ? id : 1;

            personModel = await _context.Persons
                .Include(p => p.addressModel)
                .OrderBy(p => p.personFirstName)
                .ThenBy(p => p.personFatherName)
                .ThenBy(p => p.personLastName)
                .AsNoTracking()
                .ToListAsync();

            var employmentStats = await _context.Employments
                .GroupBy(e => e.employmentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            Employed = employmentStats.FirstOrDefault(x => x.Status == mainStatus.Active)?.Count ?? 0;
            xEmployed = employmentStats.FirstOrDefault(x => x.Status == mainStatus.Inactive)?.Count ?? 0;

            TotalPages = (int)Math.Ceiling(personModel.Count / (double)PageSize);
        }

    }
}
