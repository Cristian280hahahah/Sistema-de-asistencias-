using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class Estudiante
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "El código debe contener exactamente 8 números.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
        public string Email { get; set; } = string.Empty;

        public string? CuentaUsuarioId { get; set; }

        // Relación muchos a muchos con Secciones
        public ICollection<Seccion> Secciones { get; set; } = new List<Seccion>();
    }
}