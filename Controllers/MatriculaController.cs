using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Data;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Controllers
{
    [Authorize(Roles = DbSeeder.RolEstudiante)]
    public class MatriculaController : Controller
    {
        public const string PeriodoActual = "2026-III";

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MatriculaController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private async Task<Estudiante> ObtenerEstudiante()
        {
            var userId = _userManager.GetUserId(User);
            return await _db.Estudiantes.Include(e => e.Carrera)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        // Oferta académica: cursos de la carrera del estudiante
        public async Task<IActionResult> Index()
        {
            var estudiante = await ObtenerEstudiante();
            if (estudiante == null) return NotFound();

            var cursos = await _db.Cursos
                .Include(c => c.Docente)
                .Where(c => c.CarreraId == estudiante.CarreraId)
                .OrderBy(c => c.Codigo).ToListAsync();

            var matriculasActivas = await _db.Matriculas
                .Where(m => m.Estado == "Activa" && m.Periodo == PeriodoActual)
                .ToListAsync();

            var lista = cursos.Select(c => new CursoMatriculaViewModel
            {
                Curso = c,
                Inscritos = matriculasActivas.Count(m => m.CursoId == c.Id),
                YaMatriculado = matriculasActivas.Any(m => m.CursoId == c.Id && m.EstudianteId == estudiante.Id)
            }).ToList();

            ViewBag.Carrera = estudiante.Carrera.Nombre;
            return View(lista);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribir(int cursoId)
        {
            var estudiante = await ObtenerEstudiante();
            var curso = await _db.Cursos.FindAsync(cursoId);
            if (estudiante == null || curso == null) return NotFound();

            // Validaciones de matrícula
            if (curso.CarreraId != estudiante.CarreraId)
            {
                TempData["Error"] = "Solo puede matricular cursos de su carrera.";
                return RedirectToAction(nameof(Index));
            }

            bool yaMatriculado = await _db.Matriculas.AnyAsync(m => m.EstudianteId == estudiante.Id
                && m.CursoId == cursoId && m.Periodo == PeriodoActual && m.Estado == "Activa");
            if (yaMatriculado)
            {
                TempData["Error"] = "Ya está matriculado en este curso.";
                return RedirectToAction(nameof(Index));
            }

            int inscritos = await _db.Matriculas.CountAsync(m => m.CursoId == cursoId
                && m.Periodo == PeriodoActual && m.Estado == "Activa");
            if (inscritos >= curso.CuposMaximos)
            {
                TempData["Error"] = "El curso no tiene cupos disponibles.";
                return RedirectToAction(nameof(Index));
            }

            _db.Matriculas.Add(new Matricula
            {
                EstudianteId = estudiante.Id,
                CursoId = cursoId,
                Periodo = PeriodoActual,
                Fecha = DateTime.Now,
                Estado = "Activa"
            });
            await _db.SaveChangesAsync();

            TempData["Ok"] = $"Se matriculó correctamente en {curso.Nombre}.";
            return RedirectToAction(nameof(MisCursos));
        }

        public async Task<IActionResult> MisCursos()
        {
            var estudiante = await ObtenerEstudiante();
            if (estudiante == null) return NotFound();

            var matriculas = await _db.Matriculas
                .Include(m => m.Curso).ThenInclude(c => c.Docente)
                .Where(m => m.EstudianteId == estudiante.Id && m.Estado == "Activa")
                .OrderBy(m => m.Curso.Codigo).ToListAsync();
            return View(matriculas);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var estudiante = await ObtenerEstudiante();
            var matricula = await _db.Matriculas
                .FirstOrDefaultAsync(m => m.Id == id && m.EstudianteId == estudiante.Id);
            if (matricula == null) return NotFound();

            matricula.Estado = "Cancelada";
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Matrícula cancelada.";
            return RedirectToAction(nameof(MisCursos));
        }
    }
}
