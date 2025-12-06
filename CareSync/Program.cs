using CareSync.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Polly;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.Services;

var builder = WebApplication.CreateBuilder(args);

// Register HTTP Context Accessor for accessing session in handlers
builder.Services.AddHttpContextAccessor();

// Add Authentication with Cookie scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = "CareSync.Auth";
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("DoctorAssistantOnly", policy => policy.RequireRole("DoctorAssistant"));
    options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
    options.AddPolicy("Lab", policy => policy.RequireRole("Lab"));
    options.AddPolicy("LabAssistant", policy => policy.RequireRole("LabAssistant"));
});

// Register Authorization Handler
builder.Services.AddScoped<AuthorizationMessageHandler>();

// Register HttpClient with Authorization Handler and a Polly retry policy
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7138/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthorizationMessageHandler>()
.AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            // Minimal logging hook - resolves at runtime from DI's logger if needed.
            // Keep this small to avoid depending on logger here.
        }));

// Register API Services
builder.Services.AddScoped<AdminApiService>();
builder.Services.AddScoped<PatientApiService>();
builder.Services.AddScoped<UserManagementApiService>();
builder.Services.AddScoped<DoctorApiService>();
builder.Services.AddScoped<IAppointmentService, AppointmentApiService>();

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Configure authorization for admin pages
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Doctor", "DoctorOnly");
    options.Conventions.AuthorizeFolder("/Patient", "PatientOnly");
    options.Conventions.AllowAnonymousToFolder("/Auth");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Privacy");
});

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/auth/login", "");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
