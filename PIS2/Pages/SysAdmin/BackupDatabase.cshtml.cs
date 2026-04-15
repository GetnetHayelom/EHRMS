using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using System.IO;


public class BackupDatabaseModel : PageModel
{
    private readonly string _logPath = Path.Combine(Directory.GetCurrentDirectory(), "//192.168.4.7/Attachments");
    private readonly IConfiguration _config;

    public BackupDatabaseModel(IConfiguration config)
    {
        _config = config;
    }
    public List<string> BackFiles { get; set; } = new();
    [BindProperty]
    public string BackupPath { get; set; }

    [BindProperty]
    public string ExcelPath { get; set; }

    public string Message { get; set; }

    public void OnGet()
    {
        BackupPath = $"C:\\Backups\\PIS2_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
        ExcelPath = $"C:\\Backups\\PIS2_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        if (!Directory.Exists(_logPath))
            return;

        BackFiles = Directory.GetFiles(_logPath)
            .OrderByDescending(f => f)
            .Select(Path.GetFileName)
            .ToList();
    }

    public void OnPost()
    {
        string action = Request.Form["action"];
        string connStr = _config.GetConnectionString("Default");
        string dbName = new SqlConnectionStringBuilder(connStr).InitialCatalog;

        try
        {
            using var conn = new SqlConnection(connStr);
            conn.Open();

            if (action == "BackupSql")
            {
                using var cmd = new SqlCommand($"BACKUP DATABASE [{dbName}] TO DISK = @path WITH FORMAT", conn);
                cmd.Parameters.AddWithValue("@path", BackupPath);
                cmd.ExecuteNonQuery();
                Message = $"✅ Database backed up to: <strong>{BackupPath}</strong>";
            }
            else if (action == "ExportExcel")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var excel = new ExcelPackage();

                DataTable tables = conn.GetSchema("Tables");

                foreach (DataRow row in tables.Rows)
                {
                    string schema = row["TABLE_SCHEMA"].ToString();
                    string tableName = row["TABLE_NAME"].ToString();
                    string query = $"SELECT * FROM [{schema}].[{tableName}]";

                    using var cmd = new SqlCommand(query, conn);
                    using var reader = cmd.ExecuteReader();
                    var sheet = excel.Workbook.Worksheets.Add(tableName);
                    sheet.Cells["A1"].LoadFromDataReader(reader, true);
                }

                FileInfo fi = new FileInfo(ExcelPath);
                excel.SaveAs(fi);
                Message = $"✅ Excel export completed to: <strong>{ExcelPath}</strong>";
            }
        }
        catch (Exception ex)
        {
            Message = $"❌ Error: {ex.Message}";
        }
    }
}
