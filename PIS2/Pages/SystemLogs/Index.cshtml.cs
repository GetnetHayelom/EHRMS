using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS2.Pages.SystemLogs
{
   
    public class IndexModel : PageModel
    {
        private readonly string _logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

        public List<string> LogFiles { get; set; } = new();
        public string LogContent { get; set; }

        public void OnGet(string file)
        {
            if (!Directory.Exists(_logPath))
                return;

            LogFiles = Directory.GetFiles(_logPath)
                .OrderByDescending(f => f)
                .Select(Path.GetFileName)
                .ToList();

            if (!string.IsNullOrEmpty(file))
            {
                var safeFile = Path.GetFileName(file);
                var path = Path.Combine(_logPath, safeFile);

                if (System.IO.File.Exists(path))
                {
                    //LogContent = System.IO.File.ReadAllText(path);
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream))
                    {
                        LogContent = reader.ReadToEnd();
                    }
                }
            }
        }
    }
}