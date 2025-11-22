using CareSync.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Register HTTP Context Accessor for accessing session in handlers
builder.Services.AddHttpContextAccessor();

// Register Authorization Handler
builder.Services.AddTransient<AuthorizationMessageHandler>();

// Register HttpClient with Authorization Handler
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5157/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();

// Register API Services
builder.Services.AddScoped<CareSync.Services.AdminApiService>();
builder.Services.AddScoped<CareSync.Services.PatientApiService>();

// Add services to the container.
builder.Services.AddRazorPages();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
