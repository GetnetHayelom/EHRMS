using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PIS2.Controllers
{
    [Authorize(Roles = "SYSADMIN")]
    [Route("admin/systemlogs")]
    public class SystemLogsController : Controller
    {
        private readonly string _logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

        public IActionResult Index()
        {
            var files = Directory.GetFiles(_logPath)
                .OrderByDescending(f => f)
                .Select(Path.GetFileName);

            return View(files);
        }

        public IActionResult View(string file)
        {
            var path = Path.Combine(_logPath, file);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var text = System.IO.File.ReadAllText(path);

            return Content(text, "text/plain");
        }
    }
}
