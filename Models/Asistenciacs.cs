using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public enum EstadoAsistencia
    {
        Presente,
        Tardanza,
        Ausente,
        Justificado
    }

    public class Asistencia
    {
        internal TimeSpan HoraRegistro;

        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraLlegada { get; set; }

        public EstadoAsistencia Estado { get; set; }

        // Foreign Keys
        [Required]
        public int EstudianteId { get; set; }
        public Estudiante? Estudiante { get; set; }

        [Required]
        public int SeccionId { get; set; }
        public Seccion? Seccion { get; set; }

        public Justificacion? Justificacion { get; set; }
    }
}