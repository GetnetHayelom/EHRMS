using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PIS2.Data;
using PIS2.Models;
using System.IO;

namespace PIS2.Controllers
{
    [ApiController]
    [Route("api/attachments")]
    public class AttachmentsController : ControllerBase
    {
        private readonly PISContext _db;
        private readonly ILogger<AttachmentsController> _logger;
        private readonly string _sharePath = @"\\192.168.4.7\Attachments";

        public AttachmentsController(
            PISContext db,
            ILogger<AttachmentsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET attachments
        [HttpGet("{table}/{recordId}")]
        public async Task<IActionResult> Get(string table, int recordId)
        {
            _logger.LogInformation("Fetching attachments for Table:{Table} RecordId:{RecordId}", table, recordId);

            var files = await _db.Attachments
                .Where(a => a.TableName == table && a.RecordId == recordId)
                .OrderByDescending(a => a.UploadedDate)
                .ToListAsync();

            _logger.LogInformation("{Count} attachments returned for Table:{Table} RecordId:{RecordId}",
                files.Count, table, recordId);

            return Ok(files);
        }

        //UPLOAD ATTACHMENTS
        [HttpPost("upload")]
        [RequestSizeLimit(10_485_760)]
        public async Task<IActionResult> Upload(
            [FromForm] IFormFile file,
            [FromForm] string tableName,
            [FromForm] int recordId,
            [FromForm] string fileTitle)
        {
            _logger.LogInformation("Upload attempt started by {User} for Table:{Table} RecordId:{RecordId}",
                User.Identity?.Name, tableName, recordId);

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload failed: No file provided.");
                return BadRequest("No file provided.");
            }

            if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(fileTitle))
            {
                _logger.LogWarning("Upload validation failed: Missing table name or file title.");
                return BadRequest("Table name and File title are required.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowed = { ".pdf", ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(extension))
            {
                _logger.LogWarning("Upload rejected: Unsupported file type {Extension}", extension);
                return BadRequest("Unsupported file type.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var attachment = new Models.AttachmentModel
                {
                    TableName = tableName,
                    RecordId = recordId,
                    FileName = fileTitle,
                    FilePath = "PENDING",
                    FileType = extension,
                    FileSize = file.Length,
                    UploadedBy = User.Identity?.Name,
                    UploadedDate = DateTime.UtcNow
                };

                _db.Attachments.Add(attachment);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Attachment record created with ID:{AttachmentID}", attachment.AttachmentID);

                var storedFileName = $"{attachment.AttachmentID}{extension}";
                var fullPath = Path.Combine(_sharePath, storedFileName);

                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    _logger.LogInformation("Created attachment directory {Directory}", directory);
                }

                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                attachment.FilePath = fullPath;
                await _db.SaveChangesAsync();
                
                _logger.LogInformation(
                    "File uploaded successfully. AttachmentID:{AttachmentID} Size:{Size} UploadedBy:{User}",
                    attachment.AttachmentID,
                    file.Length.Bytes(),
                    User.Identity?.Name);

                await transaction.CommitAsync();

                return Ok(new { id = attachment.AttachmentID, name = fileTitle });
            }
            catch (IOException ioEx)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ioEx,
                    "Storage error during file upload. Table:{Table} RecordId:{RecordId}",
                    tableName, recordId);

                return StatusCode(503, "Storage service unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex,
                    "Unexpected error during file upload for Table:{Table} RecordId:{RecordId} EX:{Exception}",
                    tableName, recordId,ex.Message);

                return StatusCode(500, $"An internal error occurred during upload. EX_ {ex.Message}");
            }
        }

        
        //VIEW ATTACHMENTS
        [Authorize(Roles = "HRCLERK, HRMANAGER")]
        [HttpGet("view/{id}")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Client)]        
        public async Task<IActionResult> View(int id)
        {
            _logger.LogInformation("Attachment view request for ID:{AttachmentID} by {User}",
                id, User.Identity?.Name);

            var att = await _db.Attachments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AttachmentID == id);

            if (att == null)
            {
                _logger.LogWarning("Attachment {AttachmentID} not found in database.", id);
                return NotFound("Record not found in database.");
            }

            var storedFileName = $"{att.AttachmentID}{att.FileType}";
            var fullPath = Path.Combine(_sharePath, storedFileName);

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogError("Physical file missing. AttachmentID:{AttachmentID} Path:{Path}",
                    id, fullPath);

                return NotFound("The physical file is missing from storage.");
            }

            try
            {
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(fullPath, out var contentType))
                    contentType = "application/octet-stream";

                var fileStream = new FileStream(
                    fullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    useAsync: true);

                _logger.LogInformation("Streaming attachment {AttachmentID} to user {User}",
                    id, User.Identity?.Name);

                return File(fileStream, contentType);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Error accessing storage while reading attachment {AttachmentID}",
                    id);

                return StatusCode(503, "Error accessing storage.");
            }
        }

        
        //UPLOAD PROFILE PICTURE
        [Authorize(Roles = "HRCLERK, HRMANAGER")]
        [HttpPost("uploadprofile")]
        [RequestSizeLimit(10_485_760)]
        public async Task<IActionResult> UploadProfile(
            [FromForm] IFormFile file,
            [FromForm] int personId)
        {
            _logger.LogInformation($"Person profile picture upload attempt started by {User} for  PersonId:{personId}",
                User.Identity?.Name, personId);

            if (file == null || file.Length == 0)
            {
                _logger.LogError("Upload failed: No file provided.");
                return BadRequest("No file provided.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowed = { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(extension))
            {
                _logger.LogError("Upload rejected: Unsupported file type {Extension}", extension);
                return BadRequest("Unsupported file type.");
            }
            
            try
            {
                var profilePath =Path.Combine(_sharePath, "Profile");
                if (!Directory.Exists(profilePath))
                {
                    Directory.CreateDirectory(profilePath);
                    _logger.LogInformation("Created profile directory {Directory}", profilePath);
                }

                var existingFiles = Directory.GetFiles(profilePath, $"{personId}.*");
                foreach (var existingFile in existingFiles)
                {
                    System.IO.File.Delete(existingFile);
                    _logger.LogWarning("Deleted existing profile picture: {FileName}", existingFile);
                }

                _logger.LogWarning("Profile record accepted with ID:{PersonID}", personId);

                var storedFileName = $"{personId}{extension}";
                var fullPath = Path.Combine(profilePath, storedFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogWarning(
                    "File uploaded successfully. PersonID:{AttachmentID} Size:{Size} UploadedBy:{User}",
                    personId,
                    file.Length.Bytes(),
                    User.Identity?.Name);

                var Audit = new AuditLog 
                {
                    TableName = "Persons",
                     RecordID = personId,
                     ColumnName = "Profile Picture",
                     OldValue = "Photo Uploaded",
                     ModifiedBy = User.Identity.Name,
                     ModifiedDate = DateTime.Now
                };

                _db.AuditLogs.Add(Audit);

                await _db.SaveChangesAsync();


                return new JsonResult(new { success = true, message="Profile picture change successfully!"});
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx,
                    "Storage error during file upload. Table:{Table} RecordId:{RecordId}",
                    personId, User.Identity.Name);

                return StatusCode(503, "Storage service unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error during file upload for Table:{Table} RecordId:{RecordId}",
                    personId, User.Identity.Name);

                return StatusCode(500, "An internal error occurred during upload.");
            }
        }


        //VIEW PROFILE
        [HttpGet("viewprofile/{id}")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> ViewProfile(int id)
        {
            var storedFileName = id;
            var fullPath = Path.Combine(_sharePath + "\\Profile");

            var file = Directory.GetFiles(fullPath, id + ".*").FirstOrDefault();

            if (file == null)
            {
                _logger.LogError("Physical file missing. PersonID:{PersonID}", id);
                return NotFound("The physical file is missing from storage.");
            }
            

            try
            {
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(file, out var contentType))
                    contentType = "application/octet-stream";

                var fileStream = new FileStream(
                    file,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    useAsync: true);

                _logger.LogInformation("Streaming profile {PersonID} to user {User}",
                    id, User.Identity?.Name);

                return File(fileStream, contentType);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex,
                    "Error accessing storage while reading profile picture {PersonID}",
                    id);

                return StatusCode(503, "Error accessing storage.");
            }
        }
    }
}