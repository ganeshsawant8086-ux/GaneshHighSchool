using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Ganesh1.Models;   // ✅ DbContext namespace

namespace Ganesh1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Add services to the container
            builder.Services.AddControllersWithViews();

            // ✅ Register DbContext with SQL Server (no BuildServiceProvider needed)
            var connectionString = builder.Configuration.GetConnectionString("StudentDBConnection")
                ?? throw new InvalidOperationException("Connection string 'StudentDBConnection' is missing from configuration.");

            builder.Services.AddDbContext<StudentDbContext>(options =>
                options.UseSqlServer(connectionString));

            var app = builder.Build();

            // ✅ Auto-initialize & seed database if needed
            DbInitializer.Initialize(app.Services);

            // ✅ Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // ✅ Default route: Home/Index
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
