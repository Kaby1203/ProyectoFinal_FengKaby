using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Data;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Controllers
{
    [Authorize(Roles = DbSeeder.RolAdmin)]
    public class DocentesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DocentesController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var docentes = await _db.Docentes.Include(d => d.Cursos)
                .OrderBy(d => d.Apellidos).ToListAsync();
            return View(docentes);
        }

        public IActionResult Create() => View("Form", new Docente());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Docente docente)
        {
            if (!ModelState.IsValid) return View("Form", docente);

            if (await _db.Docentes.AnyAsync(d => d.Correo == docente.Correo))
            {
                ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
                return View("Form", docente);
            }

            _db.Docentes.Add(docente);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Docente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var docente = await _db.Docentes.FindAsync(id);
            if (docente == null) return NotFound();
            return View("Form", docente);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Docente docente)
        {
            if (id != docente.Id) return NotFound();
            if (!ModelState.IsValid) return View("Form", docente);

            if (await _db.Docentes.AnyAsync(d => d.Correo == docente.Correo && d.Id != id))
            {
                ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
                return View("Form", docente);
            }

            _db.Update(docente);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Docente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var docente = await _db.Docentes.FindAsync(id);
            if (docente == null) return NotFound();

            // Los cursos del docente quedan sin docente asignado (SetNull)
            _db.Docentes.Remove(docente);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Docente eliminado. Sus cursos quedaron sin docente asignado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
