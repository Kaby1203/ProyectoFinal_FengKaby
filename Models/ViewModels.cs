using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_FengKaby.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese su correo.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [Display(Name = "Correo institucional")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Ingrese su contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ingrese su nombre completo.")]
        [StringLength(150)]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "Ingrese su correo.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [Display(Name = "Correo institucional")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Seleccione su carrera.")]
        [Display(Name = "Carrera")]
        public int? CarreraId { get; set; }

        [Required(ErrorMessage = "Ingrese una contraseña.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirme su contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; }
    }

    public class CursoMatriculaViewModel
    {
        public Curso Curso { get; set; }
        public int Inscritos { get; set; }
        public bool YaMatriculado { get; set; }
        public int CuposDisponibles => Curso.CuposMaximos - Inscritos;
    }
}
