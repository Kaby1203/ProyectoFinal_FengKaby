using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Data;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Controllers
{
    // Estudiantes y administradores pueden ver las carreras;
    // solo el administrador puede crear, editar o eliminar.
    [Authorize]
    public class CarrerasController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CarrerasController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var carreras = await _db.Carreras
                .Include(c => c.Cursos)
                .OrderBy(c => c.Nombre).ToListAsync();
            return View(carreras);
        }

        [Authorize(Roles = DbSeeder.RolAdmin)]
        public IActionResult Create() => View("Form", new Carrera());

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = DbSeeder.RolAdmin)]
        public async Task<IActionResult> Create(Carrera carrera)
        {
            if (!ModelState.IsValid) return View("Form", carrera);

            if (await _db.Carreras.AnyAsync(c => c.Nombre == carrera.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe una carrera con ese nombre.");
                return View("Form", carrera);
            }

            _db.Carreras.Add(carrera);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Carrera creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = DbSeeder.RolAdmin)]
        public async Task<IActionResult> Edit(int id)
        {
            var carrera = await _db.Carreras.FindAsync(id);
            if (carrera == null) return NotFound();
            return View("Form", carrera);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = DbSeeder.RolAdmin)]
        public async Task<IActionResult> Edit(int id, Carrera carrera)
        {
            if (id != carrera.Id) return NotFound();
            if (!ModelState.IsValid) return View("Form", carrera);

            if (await _db.Carreras.AnyAsync(c => c.Nombre == carrera.Nombre && c.Id != id))
            {
                ModelState.AddModelError("Nombre", "Ya existe una carrera con ese nombre.");
                return View("Form", carrera);
            }

            _db.Update(carrera);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Carrera actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = DbSeeder.RolAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            var carrera = await _db.Carreras.FindAsync(id);
            if (carrera == null) return NotFound();

            bool tieneRelaciones = await _db.Cursos.AnyAsync(c => c.CarreraId == id)
                                || await _db.Estudiantes.AnyAsync(e => e.CarreraId == id);
            if (tieneRelaciones)
            {
                TempData["Error"] = "No se puede eliminar: la carrera tiene cursos o estudiantes asociados.";
                return RedirectToAction(nameof(Index));
            }

            _db.Carreras.Remove(carrera);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Carrera eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
