using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class Materia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código de materia es obligatorio.")]
        [StringLength(8, ErrorMessage = "El código no puede superar 8 caracteres.")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "El código solo puede contener letras y números, sin símbolos ni espacios.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de la materia es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Seccion> Secciones { get; set; } = new List<Seccion>();
    }
}