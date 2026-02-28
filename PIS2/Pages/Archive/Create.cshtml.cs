using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Pages.Archive
{
    [Authorize(Roles ="MIE\\PMS_HRCLERK")]
    public class CreateModel : PageModel
    {
        private readonly PISContext _context;

        public CreateModel(PISContext context)
        {
            _context = context;
        }

        [BindProperty]
        public archiveModel Archive { get; set; } = new();

        public letterModel Letter { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Letter = await _context.Letters
                .FirstOrDefaultAsync(l => l.letterID == id);

            if (Letter == null)
            { return NotFound();}

            // Prevent double archive
            bool alreadyArchived = await _context.Archives
                .AnyAsync(a => a.letterID == id);

            if (alreadyArchived)
            { return RedirectToPage("/Shared/AccessDenied"); }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var letter = await _context.Letters
                .FirstOrDefaultAsync(l => l.letterID == Archive.letterID);

            if (letter == null)
                return NotFound();
            ModelState.Clear();
            ModelState.Remove("Archive.modifiedBy");
            ModelState.Remove("Archive.archivedDate");
            ModelState.Remove("Archive.modifiedDate");
            // Set system fields
            Archive.archivedDate = DateTime.Now;
            Archive.modifiedBy = User.Identity?.Name;
            Archive.modifiedDate = DateTime.Now;

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
                Letter = letter;
                return Page();
            }

            

            _context.Archives.Add(Archive);
            letter.letterNumber =await GenerateLetterNumber(letter);

            // Update letter status
            letter.letterStatus = LetterStatus.Archived;
            letter.modifiedBy = User.Identity?.Name;
            letter.modifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            //TempData["Success"] =("Error",$"Letter ID {Archive.letterID} archived successfully.") ;

            return RedirectToPage("/Letter/Details", new { id = Archive.letterID });
        }

        private async Task<string> GenerateLetterNumber(letterModel letter)
        {
            int year = DateTime.Now.Year;
            var type = await _context.LetterTypes.AsNoTracking().FirstOrDefaultAsync(t => t.letterTypeID == letter.letterTypeID);

            var seq = await _context.LetterSequences
                .FirstOrDefaultAsync(x => x.letterTypeCode == type.letterTypeCode && x.Year == year);

            if (seq == null)
            {
                seq = new LetterSequence
                {
                    letterTypePrefix = type.letterTypePrefix,
                    letterTypeCode = type.letterTypeCode,
                    letterGroup = letter.letterGroup,
                    Year = year,
                    lastNumber = 0
                };
                _context.LetterSequences.Add(seq);
            }

            seq.lastNumber++;

            await _context.SaveChangesAsync();

            string prefix = letter.letterGroup switch
            {
                LetterGroup.Incoming => "01",
                LetterGroup.Outgoing => "02",
                LetterGroup.Internal => "03",
                _ => "04"
            };

            return $"{prefix}/{type.letterTypeCode}/{seq.lastNumber:D4}/{year}";
        }

    }
}