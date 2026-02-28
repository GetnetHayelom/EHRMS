using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Drawing;
using System;
using System.Net.Mail;
using System.Runtime.Intrinsics.X86;

namespace PIS2.Controllers
{
    [ApiController]
    [Route("api/attachments")]
    public class AttachmentsController : ControllerBase
    {
        private readonly Models.PISContext _db;
        private readonly string _sharePath = @"\\192.168.4.7\Attachments"; // network path

        public AttachmentsController(Models.PISContext db)
        {
            _db = db;
        }

        // GET attachments
        [HttpGet("{table}/{recordId}")]
        public async Task<IActionResult> Get(string table, int recordId)
        {
            var files = await _db.Attachments
                .Where(a => a.TableName == table && a.RecordId == recordId)
                .OrderByDescending(a => a.UploadedDate)
                .ToListAsync();

            return Ok(files);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10_485_760)] // Hard limit at the server level (e.g., 10MB)
        public async Task<IActionResult> Upload(
    [FromForm] IFormFile file,
    [FromForm] string tableName,
    [FromForm] int recordId,
    [FromForm] string fileTitle)
        {
            // 1. Initial Validation
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            // Validate inputs to prevent DB errors or script injection
            if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(fileTitle))
                return BadRequest("Table name and File title are required.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowed = { ".pdf", ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(extension))
                return BadRequest("Unsupported file type.");

            // 2. Transaction Management
            // We use a transaction so if the physical save fails, the DB record doesn't stay behind (Orphaned Records)
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 3. Create DB Record
                var attachment = new Models.AttachmentModel
                {
                    TableName = tableName,
                    RecordId = recordId,
                    FileName = fileTitle,
                    FilePath = "PENDING", // Placeholder
                    FileType = extension,
                    FileSize = file.Length,
                    UploadedBy = User.Identity?.Name ?? "System",
                    UploadedDate = DateTime.UtcNow // Always track time in production
                };

                _db.Attachments.Add(attachment);
                await _db.SaveChangesAsync();

                // 4. Physical Save
                // Sanitize filename to prevent path traversal attacks
                var storedFileName = $"{attachment.AttachmentID}{extension}";
                var fullPath = Path.Combine(_sharePath, storedFileName);

                // Ensure directory exists
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                // 5. Update Path and Commit
                attachment.FilePath = fullPath;
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { id = attachment.AttachmentID, name = fileTitle });
            }
            catch (IOException ioEx)
            {
                // Log: "Disk full or network share unreachable"
                await transaction.RollbackAsync();
                return StatusCode(503, "Storage service unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                // Log: ex.Message
                await transaction.RollbackAsync();
                return StatusCode(500, "An internal error occurred during upload.");
            }
        }


        [Authorize(Roles = "MIE\\PMS_HRCLERK, MIE\\PMS_HRMANAGER")]
        [HttpGet("view/{id}")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Client)] // Cache for 24 hours
        public async Task<IActionResult> View(int id)
        {
            // 1. Fetch metadata
            var att = await _db.Attachments.AsNoTracking().FirstOrDefaultAsync(x => x.AttachmentID == id);

            if (att == null)
                return NotFound("Record not found in database.");

            // 2. Resolve Path (Ensure it matches your upload logic)
            var storedFileName = $"{att.AttachmentID}{att.FileType}";
            var fullPath = Path.Combine(_sharePath, storedFileName);

            // 3. Check physical existence
            if (!System.IO.File.Exists(fullPath))
            {
                // Log: $"File missing on disk: {fullPath} for AttachmentID: {id}"
                return NotFound("The physical file is missing from storage.");
            }

            try
            {
                // 4. Determine Content Type
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fullPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                // 5. STREAM the file instead of reading all bytes
                // 'FileStream' is better for memory because it chunks the data
                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

                // Use this to OPEN in browser (Inline)
                return File(fileStream, contentType);
                // This overload handles the disposal of the stream automatically
                // Use this to FORCE DOWNLOAD(Attachment)
                //return File(fileStream, contentType, att.FileName ?? "document" + att.FileType);
            }
            catch (IOException ex)
            {
                // Log: ex (e.g., Network share timeout)
                return StatusCode(503, "Error accessing storage.");
            }
        }

    }

}
