using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PIS2.Data;
using PIS2.Middleware;
using PIS2.Models.Foundation;
using PIS2.Services;
using PIS2.Services.Finance;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// Add services to the container
// -------------------------------

// Enable Windows Authentication (Negotiate)
//builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//   .AddNegotiate();
//builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
//builder.Services.AddAuthorization();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()               // Capture everything Debug and above
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // optional
    .Enrich.FromLogContext()
    .WriteTo.File(
        "logs/system-.log",
        rollingInterval: RollingInterval.Day,   
        retainedFileCountLimit: 30,
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

// Global authorization policy � all requests require authorization by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add Payroll Service
builder.Services.AddScoped<PayrollService>();

// Add Razor Pages and Controllers
builder.Services.AddRazorPages(options =>
{
    // Force every page in the app to require authentication unless marked [AllowAnonymous]
    options.Conventions.AuthorizeFolder("/");
});

builder.Services.AddControllers();

// Add core application services
builder.Services.AddScoped<Core>();
builder.Services.AddScoped<LeaveService>();
builder.Services.AddScoped<HRDashboardService>();
builder.Services.AddScoped<Global_S>();
builder.Services.AddScoped<Global_C>();
builder.Services.AddScoped<AccessService>();
builder.Services.AddScoped<LanguageContext>();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddScoped<LocalizationHelper>();
builder.Services.AddScoped<JournalService>();

// Configure database context
builder.Services.AddDbContext<PISContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();


// Add Identity with Roles
builder.Services.AddIdentity<userModel, IdentityRole<int>>(options =>
{
    // Disable email requirement
    options.User.RequireUniqueEmail = false;

    // Allow alphanumeric characters, underscores, dashes, etc. for usernames
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";

    // Configure password rules
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<PISContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "PIS2.Identity";

    options.LoginPath = "/Account/Login";

    options.LogoutPath = "/Account/Logout";

    options.AccessDeniedPath = "/Account/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    options.SlidingExpiration = true;
});
// -------------------------------
// Build the application
// -------------------------------
var app = builder.Build();

// ==========================================
// SEEDING EXECUTION BLOCK
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Executes the static method from Seeder.cs
        await Seeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
// ==========================================


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

app.UseMiddleware<PasswordChanger>();

app.UseAuthorization();
app.UseMiddleware<LocalizationMiddleware>();
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
