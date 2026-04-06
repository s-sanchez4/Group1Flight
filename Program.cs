using Microsoft.EntityFrameworkCore;
using Group1Flight.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddHttpContextAccessor();


builder.Services.AddDbContext<FlightContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FlightContext") ?? "Data Source=Flight.sqlite"));

builder.Services.AddControllersWithViews();

var app = builder.Build();


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