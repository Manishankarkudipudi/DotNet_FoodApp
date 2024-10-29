using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Stripe;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ProjectContext>(options =>
    options.UseSqlServer(connectionString));builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ProjectContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
options.IdleTimeout = TimeSpan.FromMinutes(30));


builder.Services.AddAuthentication();





var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication and Authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Use session middleware
app.UseSession();

// Map default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map admin route
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "admin/{action=Index}/{id?}",
        defaults: new { controller = "Admin" });
});

// Map admin route
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "user",
        pattern: "user/{action=Index}/{id?}",
        defaults: new { controller = "Users" });
});

// Map Razor Pages for Identity (Login, Register, etc.)
app.MapRazorPages();

app.Run();
