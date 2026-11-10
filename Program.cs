using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Data;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
        var builder = WebApplication.CreateBuilder(args);

        // Base de datos (SQL Server LocalDB)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ASP.NET Core Identity con roles
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccesoDenegado";
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Crear la base de datos, roles, usuario administrador y datos iniciales
        using (var scope = app.Services.CreateScope())
        {
            await DbSeeder.SeedAsync(scope.ServiceProvider);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
        }
    }
}
