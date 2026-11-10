using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_FengKaby.Models
{
    public class Carrera
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la carrera es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre de la carrera")]
        public string Nombre { get; set; }

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
    }
}
