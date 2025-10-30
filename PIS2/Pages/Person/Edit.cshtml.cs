using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Person
{
    [Authorize(Roles = "MIE\\PMS_HRCLERK")]
    public class EditModel : PageModel
    {
        private readonly PISContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(PISContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public personModel personModel { get; set; } = default!;
        [BindProperty]
        public addressModel addressModel { get; set; } = default!;
        [BindProperty]
        public IFormFile Photo { get; set; } = default!;
        public bool PhotoExists { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons.FirstOrDefaultAsync(m => m.personID == id);
            if (person == null) return NotFound();

            personModel = person;
            addressModel = await _context.Addresses.FirstOrDefaultAsync(a => a.addressID == person.addressID)
                           ?? new addressModel();

            var photoPath = Path.Combine(_environment.WebRootPath, "images", $"{person.personID}.jpg");
            PhotoExists = System.IO.File.Exists(photoPath);

            ViewData["addressID"] = new SelectList(_context.Addresses, "addressID", "addressFormatted");


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            personModel.modifiedBy = User.Identity?.Name ?? "system";
            personModel.addressID = await GetOrCreateAddress(addressModel);

            ModelState.Clear(); // Ensure clean state for re-validation
            if (!TryValidateModel(personModel))
            {
                return Page();
            }

            _context.Attach(personModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!personModelExists(personModel.personID))
                    return NotFound();
                throw;
            }

            // ✅ Handle photo upload
            if (Photo != null && Photo.Length > 0)
            {
                var fileExt = Path.GetExtension(Photo.FileName);
                var baseFileName = $"{personModel.personID}";
                var imagesFolder = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var newFilePath = Path.Combine(imagesFolder, baseFileName + fileExt);

                // ✅ If a main image already exists, rename it with incremental suffix (_0, _1, etc.)
                if (System.IO.File.Exists(newFilePath))
                {
                    int suffix = 0;
                    string oldFilePath;
                    do
                    {
                        oldFilePath = Path.Combine(imagesFolder, $"{baseFileName}_{suffix}{fileExt}");
                        suffix++;
                    } while (System.IO.File.Exists(oldFilePath));

                    // Rename existing original file
                    System.IO.File.Move(newFilePath, oldFilePath);
                }

                // ✅ Save new uploaded photo as personID.ext
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }
            }

            return RedirectToPage("./Details", new { id = personModel.personID });
        }


        private bool personModelExists(int id)
        {
            return _context.Persons.Any(e => e.personID == id);
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
                return existingAddress.addressID;

            addressModel.modifiedBy = User.Identity?.Name ?? "system";
            addressModel.addressStatus = mainStatus.Active;

            _context.Addresses.Add(addressModel);
            await _context.SaveChangesAsync();
            return addressModel.addressID;
        }
    }
}
