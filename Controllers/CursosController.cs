using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Data;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Controllers
{
    [Authorize(Roles = DbSeeder.RolAdmin)]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CursosController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var cursos = await _db.Cursos
                .Include(c => c.Carrera).Include(c => c.Docente).Include(c => c.Matriculas)
                .OrderBy(c => c.Codigo).ToListAsync();
            return View(cursos);
        }

        public async Task<IActionResult> Create()
        {
            await CargarListas();
            return View("Form", new Curso { Creditos = 3, CuposMaximos = 30 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (await _db.Cursos.AnyAsync(c => c.Codigo == curso.Codigo))
                ModelState.AddModelError("Codigo", "Ya existe un curso con ese código.");

            if (!ModelState.IsValid)
            {
                await CargarListas(curso);
                return View("Form", curso);
            }

            _db.Cursos.Add(curso);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Curso registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            await CargarListas(curso);
            return View("Form", curso);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (await _db.Cursos.AnyAsync(c => c.Codigo == curso.Codigo && c.Id != id))
                ModelState.AddModelError("Codigo", "Ya existe un curso con ese código.");

            // Control de cupos: no se puede bajar el cupo por debajo de los ya matriculados
            int inscritos = await _db.Matriculas.CountAsync(m => m.CursoId == id && m.Estado == "Activa");
            if (curso.CuposMaximos < inscritos)
                ModelState.AddModelError("CuposMaximos",
                    $"El curso ya tiene {inscritos} estudiantes matriculados; el cupo no puede ser menor.");

            if (!ModelState.IsValid)
            {
                await CargarListas(curso);
                return View("Form", curso);
            }

            _db.Update(curso);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Curso actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            if (await _db.Matriculas.AnyAsync(m => m.CursoId == id))
            {
                TempData["Error"] = "No se puede eliminar: el curso tiene matrículas registradas.";
                return RedirectToAction(nameof(Index));
            }

            _db.Cursos.Remove(curso);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Curso eliminado.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListas(Curso curso = null)
        {
            var carreras = await _db.Carreras.OrderBy(c => c.Nombre).ToListAsync();
            var docentes = await _db.Docentes.OrderBy(d => d.Apellidos).ToListAsync();
            ViewBag.Carreras = new SelectList(carreras, "Id", "Nombre", curso?.CarreraId);
            ViewBag.Docentes = new SelectList(docentes, "Id", "NombreCompleto", curso?.DocenteId);
        }
    }
}
