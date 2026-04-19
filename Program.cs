using Microsoft.EntityFrameworkCore;
using Group1Flight.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES (The "Add" Section) ---
builder.Services.AddControllersWithViews();

// Database setup
builder.Services.AddDbContext<FlightContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FlightContext") ?? "Data Source=Flight.sqlite"));

// Essential for your Session and CartWrapper
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache(); 

// Session Configuration (Increased to 30 mins as per your preference)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Path = "/";
});

// --- 2. BUILD (Only once!) ---
var app = builder.Build();

// --- 3. MIDDLEWARE (The "Use" Section - ORDER MATTERS) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // Routing comes first

// SESSION MUST BE HERE: After Routing, BEFORE Authorization
app.UseSession(); 

app.UseAuthorization();

// --- 4. ROUTING (The "Map" Section) ---
app.MapStaticAssets(); 

// Area Route (Airlines/Admin)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default Route (Public Site)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}") 
    .WithStaticAssets();

app.Run();