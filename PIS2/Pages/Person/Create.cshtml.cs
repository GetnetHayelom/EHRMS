using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models.Foundation;
using PIS2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIS2.Pages.Person
{
    [Authorize(Roles = "HRPERSONNEL")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;
        private readonly Core _core;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CreateModel> _logger;

        private readonly string _sharePath = @"\\192.168.4.7\Attachments";

        public CreateModel(PISContext context, IWebHostEnvironment environment, Core core, ILogger<CreateModel> logger)
        {
            _context = context;
            _environment = environment;
            _core = core;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
        ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");
            return Page();
        }

        [BindProperty]
        public personModel personModel { get; set; } = default!;
        [BindProperty]
        public addressModel addressModel { get; set; } = default!;
        [BindProperty]
        public IFormFile? Photo { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("personModel.modifiedBy");
            ModelState.Remove("addressModel.modifiedBy");
            ModelState.Remove("Photo");

            personModel.modifiedBy = User.Identity.Name;
            personModel.addressID = await GetOrCreateAddress(addressModel);

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var error in kv.Value.Errors)
                    {
                        Console.WriteLine($"{kv.Key} --> {error.ErrorMessage}");
                        TempData["message"] = ("Error", $"{kv.Key} --> {error.ErrorMessage}");
                    }
                }
                
                return Page();
            }
            _context.Persons.Add(personModel);
            try
            {
                await _context.SaveChangesAsync();
            }catch(Exception ex)
            {
                TempData["message"] = ("Error", ex.Message);
            }
            

            if (Photo != null && Photo.Length > 0)
            {
                var fileExt = Path.GetExtension(Photo.FileName);
                var fileName = $"{personModel.personID}{fileExt}";
                var imagesFolder = Path.Combine(_sharePath, "Profiles");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                
                var existingFiles = Directory.GetFiles(imagesFolder, $"{personModel.personID}.*");
                foreach (var existingFile in existingFiles)
                {
                    System.IO.File.Delete(existingFile);
                    _logger.LogInformation("Deleted existing profile picture: {FileName}", existingFile);
                }
                
                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                    Console.WriteLine("################# Photo Saved");
                }
            }
            
            return RedirectToPage("./Details", new {id = personModel.personID});
        }
        public async Task<int> GetOrCreateAddress(addressModel addressModel)
        {
            var existingAddress =await _context.Addresses.FirstOrDefaultAsync(a =>
            a.addressCountry == addressModel.addressCountry &&
            a.addressRegion.ToLower() == addressModel.addressRegion.ToLower() &&
            a.addressZone.ToLower() == addressModel.addressZone.ToLower() &&
            a.addressWoreda.ToLower() == addressModel.addressWoreda.ToLower() &&
            a.addressTabya.ToLower() == addressModel.addressTabya.ToLower());
            if (existingAddress != null)
            {
                return existingAddress.addressID;
            }
            addressModel.modifiedBy = User.Identity.Name;
            addressModel.addressStatus = mainStatus.Active;

            _context.Addresses.Add(addressModel);
            await _context.SaveChangesAsync();
            return addressModel.addressID;           
        }

        public async Task<JsonResult> OnGetAddressSuggestions(string level, Country country,string region, string zone,string woreda,string term)
        {
            var result = await _core.GetAddressSuggestions(
                level, country, region, zone, woreda, term);

            return new JsonResult(result);
        }
    }
}
