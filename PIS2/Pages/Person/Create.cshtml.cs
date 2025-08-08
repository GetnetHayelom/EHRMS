using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using PIS2.Models;

namespace PIS2.Pages.Person
{
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(PIS2.Models.PISContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        public IFormFile Photo { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            personModel.modifiedBy = User.Identity.Name;
            personModel.addressID = await GetOrCreateAddress(addressModel);
            ModelState.Clear();
            Console.WriteLine("################# modelstate cleared");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("################# modelstate is Invalid");
                return Page();
            }

            
            Console.WriteLine("################# Address ID is" + personModel.addressID);
            _context.Persons.Add(personModel);
            await _context.SaveChangesAsync();

            //var personHistory = new personHistoryModel
            //{
            //    personID = personModel.personID,
            //    personFirstName = personModel.personFirstName,
            //    personFatherName = personModel.personFatherName,
            //    personLastName = personModel.personLastName,
            //    personGender = personModel.personGender,
            //    personDoB = personModel.personDoB,
            //    personEmailAddress = personModel.personEmailAddress,
            //    personIDNumber = personModel.personIDNumber,
            //    personIDType = personModel.personIDType,
            //    personPhoneNumber = personModel.personPhoneNumber,
            //    personRecordNumber = personModel.personRecordNumber,
            //    modifiedBy = personModel.modifiedBy,
            //    modifiedDate = DateTime.Now.ToString()
            //};
            //_context.PersonHistories.Add(personHistory);
            //await _context.SaveChangesAsync();

            Console.WriteLine("################# Pesron Saved" + personModel.personID);
            if (Photo != null && Photo.Length > 0)
            {
                var fileExt = Path.GetExtension(Photo.FileName);
                var fileName = $"{personModel.personID}{fileExt}";
                var imagesFolder = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                    Console.WriteLine("################# Photo Saved");
                }
            }
            else
            {
                ModelState.AddModelError("Photo","Please upload a photo");
                Console.WriteLine("###Photo Null##########################");
                return Page();
            }
            return RedirectToPage("./Details", new {id = personModel.personID});
        }
        public async Task<int> GetOrCreateAddress(addressModel addressModel)
        {
            
            var existingAddress = _context.Addresses.FirstOrDefault(a =>
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
    }
}
