using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class Profesor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
        public string Email { get; set; } = string.Empty;

        public string? CuentaUsuarioId { get; set; }

        public ICollection<Seccion> Secciones { get; set; } = new List<Seccion>();
    }
}