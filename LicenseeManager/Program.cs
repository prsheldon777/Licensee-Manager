using Licensee_Manager.Models;
using LicenseeManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer; // Add this using directive

/// <summary>
/// Program entry for the Licensee Manager web application.
/// </summary>
/// <remarks>
/// Configures services (MVC and EF Core DbContext), builds the <see cref="WebApplication"/>, 
/// sets up the HTTP request pipeline and runs the application.
/// This class uses the explicit <c>Main</c> entrypoint to allow XML documentation.
/// </remarks>
public static class Program
{
    /// <summary>
    /// Application entry point that configures and runs the web host.
    /// </summary>
    /// <param name="args">Command-line arguments forwarded to <see cref="WebApplication.CreateBuilder(string[])"/>.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // Production-style error handling
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
