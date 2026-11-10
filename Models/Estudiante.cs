using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_FengKaby.Models
{
    public class Estudiante
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required, StringLength(150)]
        public string NombreCompleto { get; set; }

        [StringLength(20)]
        public string Expediente { get; set; }

        public int CarreraId { get; set; }
        public Carrera Carrera { get; set; }

        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
