using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Data
{
    public static class DbSeeder
    {
        public const string RolAdmin = "Administrador";
        public const string RolEstudiante = "Estudiante";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Crea la base de datos y las tablas si no existen
            await db.Database.EnsureCreatedAsync();

            // Roles
            foreach (var rol in new[] { RolAdmin, RolEstudiante })
            {
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));
            }

            // Usuario administrador por defecto
            const string adminEmail = "admin@unimatricula.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Administrador UniMatrícula"
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, RolAdmin);
            }

            // Datos iniciales
            if (!await db.Carreras.AnyAsync())
            {
                db.Carreras.AddRange(
                    new Carrera { Nombre = "Ingeniería en Sistemas", Descripcion = "Desarrollo de software, bases de datos y redes." },
                    new Carrera { Nombre = "Administración de Empresas", Descripcion = "Gestión, finanzas y emprendimiento." },
                    new Carrera { Nombre = "Contaduría", Descripcion = "Contabilidad, auditoría e impuestos." },
                    new Carrera { Nombre = "Derecho", Descripcion = "Ciencias jurídicas y práctica legal." });
                await db.SaveChangesAsync();
            }

            if (!await db.Docentes.AnyAsync())
            {
                db.Docentes.AddRange(
                    new Docente { Nombre = "Juan", Apellidos = "Pérez", Correo = "jperez@unimatricula.com", Especialidad = "Programación" },
                    new Docente { Nombre = "Laura", Apellidos = "Gómez", Correo = "lgomez@unimatricula.com", Especialidad = "Matemática" },
                    new Docente { Nombre = "Carlos", Apellidos = "Solano", Correo = "csolano@unimatricula.com", Especialidad = "Bases de datos" });
                await db.SaveChangesAsync();
            }

            if (!await db.Cursos.AnyAsync())
            {
                var sistemas = await db.Carreras.FirstAsync(c => c.Nombre == "Ingeniería en Sistemas");
                var admon = await db.Carreras.FirstAsync(c => c.Nombre == "Administración de Empresas");
                var juan = await db.Docentes.FirstAsync(d => d.Correo == "jperez@unimatricula.com");
                var laura = await db.Docentes.FirstAsync(d => d.Correo == "lgomez@unimatricula.com");
                var carlos = await db.Docentes.FirstAsync(d => d.Correo == "csolano@unimatricula.com");

                db.Cursos.AddRange(
                    new Curso { Codigo = "SIS101", Nombre = "Programación I", Creditos = 4, CuposMaximos = 30, CarreraId = sistemas.Id, DocenteId = juan.Id },
                    new Curso { Codigo = "MAT201", Nombre = "Cálculo I", Creditos = 4, CuposMaximos = 35, CarreraId = sistemas.Id, DocenteId = laura.Id },
                    new Curso { Codigo = "SIS220", Nombre = "Bases de Datos", Creditos = 4, CuposMaximos = 25, CarreraId = sistemas.Id, DocenteId = carlos.Id },
                    new Curso { Codigo = "ADM110", Nombre = "Introducción a la Administración", Creditos = 3, CuposMaximos = 40, CarreraId = admon.Id, DocenteId = laura.Id });
                await db.SaveChangesAsync();
            }
        }
    }
}
