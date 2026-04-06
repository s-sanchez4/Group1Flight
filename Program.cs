using Microsoft.EntityFrameworkCore;
using Group1Flight.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Session Services
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// NEW: Requirement #5 - Required for the CartWrapper to access Cookies
builder.Services.AddHttpContextAccessor();

// 2. Add Database Context
builder.Services.AddDbContext<FlightContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FlightContext") ?? "Data Source=Flight.sqlite"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. MIDDLEWARE ORDER (CRITICAL)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SESSION MUST BE HERE: After Routing, but BEFORE Authorization
app.UseSession(); 

app.UseAuthorization();

// 4. ROUTING
app.MapStaticAssets(); 

// Area Route (Airlines)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default Route (Public Site)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Flights}/{action=Index}/{id?}") 
    .WithStaticAssets();

app.Run();