using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using PIS2.Middleware;
using PIS2.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// Add services to the container
// -------------------------------

// Enable Windows Authentication (Negotiate)
//builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//   .AddNegotiate();
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
//builder.Services.AddAuthorization();


// Global authorization policy — all requests require authorization by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

//Add Payroll Service
builder.Services.AddScoped<PayrollService>();

// Add Razor Pages and Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Register your application services
builder.Services.AddScoped<Core>();

// Configure your database context
builder.Services.AddDbContext<PISContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


// -------------------------------
// Build the application
// -------------------------------
var app = builder.Build();


// -------------------------------
// Configure the HTTP request pipeline
// -------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/403");

    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------------------
// Custom middleware to handle 403 (Access Denied)
// ---------------------------------------------
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
    {
        context.Response.Redirect("/Shared/AccessDenied");
    }
});

// Map Razor Pages and API Controllers
app.MapRazorPages();
app.MapControllers();

app.Run();
