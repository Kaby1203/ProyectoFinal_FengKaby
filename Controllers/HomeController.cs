using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Data;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Dashboard: cambia según el rol (estudiante o administrador)
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole(DbSeeder.RolAdmin))
            {
                ViewBag.TotalEstudiantes = await _db.Estudiantes.CountAsync();
                ViewBag.TotalCursos = await _db.Cursos.CountAsync();
                ViewBag.TotalDocentes = await _db.Docentes.CountAsync();
                ViewBag.TotalCarreras = await _db.Carreras.CountAsync();
                ViewBag.TotalMatriculas = await _db.Matriculas.CountAsync(m => m.Estado == "Activa");
                return View("DashboardAdmin");
            }

            var userId = _userManager.GetUserId(User);
            var estudiante = await _db.Estudiantes.Include(e => e.Carrera)
                .FirstOrDefaultAsync(e => e.UserId == userId);
            if (estudiante == null) return RedirectToAction("Login", "Account");

            ViewBag.CursosMatriculados = await _db.Matriculas
                .CountAsync(m => m.EstudianteId == estudiante.Id && m.Estado == "Activa");
            ViewBag.Creditos = await _db.Matriculas
                .Where(m => m.EstudianteId == estudiante.Id && m.Estado == "Activa")
                .SumAsync(m => (int?)m.Curso.Creditos) ?? 0;
            return View("DashboardEstudiante", estudiante);
        }

        [AllowAnonymous]
        public IActionResult Error() => View();
    }
}
