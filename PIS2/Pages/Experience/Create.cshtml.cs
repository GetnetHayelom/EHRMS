using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PIS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIS2.Pages.Exprience
{

    [Authorize(Roles = "MIE\\PMS_HRMANAGER,MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PIS2.Models.PISContext _context;

        public CreateModel(PIS2.Models.PISContext context)
        {
            _context = context;
        }
        [BindProperty]
        public List<experienceModel> Experiences { get; set; } = default!;
        [BindProperty]
        public List<IFormFile>? UploadedFiles { get; set; } = new List<IFormFile>();


        public IActionResult OnGet(int? id)
        {
            Experiences = new List<experienceModel>();

            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
            {
                return RedirectToPage("/Shared/AccessDenied");
            }
            if (id != null)
            {
                experienceModel = new experienceModel();
                experienceModel.personID = id ?? 0;
                Experiences = _context.Experiences.Where(e => e.personID == id).ToList();
            }
            ViewData["personID"] = new SelectList(_context.Persons.ToList(), "personID", "personFullName");
            return Page();
        }

        [BindProperty]
        public experienceModel experienceModel { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("MIE\\PMS_HRCLERK"))
                return RedirectToPage("/Shared/AccessDenied");

            if (!ModelState.IsValid)
                return Page();

            // Save Experience
            _context.Experiences.Add(experienceModel);
            await _context.SaveChangesAsync();

            // Save uploaded files
            if (UploadedFiles != null && UploadedFiles.Any())
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "experience", experienceModel.experienceID.ToString());
                Directory.CreateDirectory(uploadsFolder);

                foreach (var file in UploadedFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    // Optional: store attachment record in DB
                    _context.Attachements.Add(new Attachements
                    {
                        fileName = file.FileName,
                        filePath = $"/uploads/experience/{experienceModel.experienceID}/{fileName}",
                        fileType = file.ContentType,
                        fileSize = file.Length,
                        uploadedBy = User.Identity.Name,
                        uploadedDate = DateTime.Now
                    });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Experience/Index", new { id = experienceModel.personID });
        }

    }
}
