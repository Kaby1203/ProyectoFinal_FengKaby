using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_FengKaby.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre del curso es obligatorio.")]
        [StringLength(120)]
        public string Nombre { get; set; }

        [Range(1, 10, ErrorMessage = "Los créditos deben estar entre 1 y 10.")]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }

        [Range(1, 200, ErrorMessage = "Los cupos deben estar entre 1 y 200.")]
        [Display(Name = "Cupos máximos")]
        public int CuposMaximos { get; set; }

        [Required(ErrorMessage = "Seleccione una carrera.")]
        [Display(Name = "Carrera")]
        public int? CarreraId { get; set; }
        public Carrera Carrera { get; set; }

        [Display(Name = "Docente")]
        public int? DocenteId { get; set; }
        public Docente Docente { get; set; }

        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
