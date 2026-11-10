using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_FengKaby.Data;
using ProyectoFinal_FengKaby.Models;

namespace ProyectoFinal_FengKaby.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        // ---------- LOGIN ----------
        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Correo);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user, model.Password, model.Recordarme, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(model);
        }

        // ---------- REGISTRO (estudiantes) ----------
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            await CargarCarreras();
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCarreras(model.CarreraId);
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Correo,
                Email = model.Correo,
                NombreCompleto = model.NombreCompleto
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, TraducirError(error));
                await CargarCarreras(model.CarreraId);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, DbSeeder.RolEstudiante);

            var estudiante = new Estudiante
            {
                UserId = user.Id,
                NombreCompleto = model.NombreCompleto,
                CarreraId = model.CarreraId.Value
            };
            _db.Estudiantes.Add(estudiante);
            await _db.SaveChangesAsync();

            // Número de expediente académico asignado por el sistema
            estudiante.Expediente = $"UM-{DateTime.Now.Year}-{estudiante.Id:D4}";
            await _db.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado() => View();

        private async Task CargarCarreras(int? seleccionada = null)
        {
            var carreras = await _db.Carreras.OrderBy(c => c.Nombre).ToListAsync();
            ViewBag.Carreras = new SelectList(carreras, "Id", "Nombre", seleccionada);
        }

        private static string TraducirError(IdentityError e) => e.Code switch
        {
            "DuplicateUserName" or "DuplicateEmail" => "Ya existe una cuenta con ese correo.",
            "PasswordTooShort" => "La contraseña debe tener al menos 6 caracteres.",
            "PasswordRequiresDigit" => "La contraseña debe incluir al menos un número.",
            "PasswordRequiresLower" => "La contraseña debe incluir al menos una letra minúscula.",
            _ => e.Description
        };
    }
}
