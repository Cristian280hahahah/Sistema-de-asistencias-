using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public enum EstadoJustificacion
    {
        Pendiente,
        Aprobado,
        Rechazado
    }

    public class Justificacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        public EstadoJustificacion Estado { get; set; } = EstadoJustificacion.Pendiente;

        // Foreign Key 1 a 1 con Asistencia
        [Required]
        public int AsistenciaId { get; set; }
        public Asistencia? Asistencia { get; set; }
    }
}