using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Curso>().HasIndex(c => c.Codigo).IsUnique();

            builder.Entity<Curso>()
                .HasOne(c => c.Carrera).WithMany(c => c.Cursos)
                .HasForeignKey(c => c.CarreraId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Curso>()
                .HasOne(c => c.Docente).WithMany(d => d.Cursos)
                .HasForeignKey(c => c.DocenteId).OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Estudiante>()
                .HasOne(e => e.Carrera).WithMany(c => c.Estudiantes)
                .HasForeignKey(e => e.CarreraId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estudiante>()
                .HasOne(e => e.User).WithMany()
                .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Matricula>()
                .HasOne(m => m.Estudiante).WithMany(e => e.Matriculas)
                .HasForeignKey(m => m.EstudianteId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Matricula>()
                .HasOne(m => m.Curso).WithMany(c => c.Matriculas)
                .HasForeignKey(m => m.CursoId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
