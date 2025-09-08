using Microsoft.Data.SqlClient;
namespace PIS2.Middleware
{
    public class DbHealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public DbHealthCheckMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(); // Try to connect
            }
            catch
            {
                // If DB is down → return custom error response
                context.Response.StatusCode = 503; // Service Unavailable
                context.Response.ContentType = "text/html";

                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"Database server is unavailable.\"}");
                }
                else
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<h1>!!!!!!!!!!Database server is unavailable. Please contact system administrator.!!!!!!!!!!!!</h1>");
                }

                return; // Do not call next middleware
            }

            // If DB is available → continue pipeline
            await _next(context);
        }
    }
}
