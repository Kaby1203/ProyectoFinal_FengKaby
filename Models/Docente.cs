using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_FengKaby.Models
{
    public class Docente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(80)]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [StringLength(120)]
        public string Correo { get; set; }

        [StringLength(100)]
        public string Especialidad { get; set; }

        [Display(Name = "Docente")]
        public string NombreCompleto => $"{Nombre} {Apellidos}";

        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
