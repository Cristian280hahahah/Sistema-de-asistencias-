using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class Seccion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El día de la semana es obligatorio.")]
        public DayOfWeek Dia { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraFin { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un aula.")]
        public int AulaId { get; set; }
        public Aula? Aula { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "Debe asignar una materia.")]
        public int MateriaId { get; set; }
        public Materia? Materia { get; set; }

        [Required(ErrorMessage = "Debe asignar un profesor.")]
        public int ProfesorId { get; set; }
        public Profesor? Profesor { get; set; }

        public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}