using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class Aula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del aula es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        public string? Ubicacion { get; set; }
        public string? Descripcion { get; set; }

        public ICollection<Seccion> Secciones { get; set; } = new List<Seccion>();
    }
}
