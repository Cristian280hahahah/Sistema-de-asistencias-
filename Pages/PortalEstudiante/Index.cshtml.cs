using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.PortalEstudiante;

[Authorize(Roles = "Estudiante")]
public class IndexModel : PageModel
{
    private readonly AsistenciaContext _context;

    public IndexModel(AsistenciaContext context)
    {
        _context = context;
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public int SectionCount { get; private set; }
    public int AttendanceCount { get; private set; }
    public int PendingJustifications { get; private set; }
    public IList<SectionRow> Sections { get; private set; } = new List<SectionRow>();
    public IList<AttendanceRow> RecentAttendance { get; private set; } = new List<AttendanceRow>();

    public async Task OnGetAsync()
    {
        var accountId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var student = await _context.Estudiantes.AsNoTracking()
            .FirstOrDefaultAsync(item => item.CuentaUsuarioId == accountId);

        if (student is null)
        {
            return;
        }

        Name = student.Nombre;
        Code = student.Codigo;
        Sections = await _context.Secciones.AsNoTracking()
            .Where(section =>
                            section.Estudiantes.Any(item => item.Id == student.Id) ||
                            section.Asistencias.Any(attendance => attendance.EstudianteId == student.Id))
            .OrderBy(section => section.Dia)
            .ThenBy(section => section.HoraInicio)
            .Select(section => new SectionRow(
                section.Aula!.Nombre,
                section.Materia == null ? "—" : section.Materia.Nombre,
                section.Profesor == null ? "—" : section.Profesor.Nombre,
                section.Dia,
                section.HoraInicio,
                section.HoraFin))
            .ToListAsync();

        var attendanceQuery = _context.Asistencias.AsNoTracking()
            .Where(attendance => attendance.EstudianteId == student.Id);
        AttendanceCount = await attendanceQuery.CountAsync();
        PendingJustifications = await attendanceQuery.CountAsync(attendance =>
            attendance.Justificacion != null && attendance.Justificacion.Estado == EstadoJustificacion.Pendiente);

        RecentAttendance = await attendanceQuery
            .OrderByDescending(attendance => attendance.Fecha)
            .ThenByDescending(attendance => attendance.HoraLlegada)
            .Take(10)
            .Select(attendance => new AttendanceRow(
                attendance.Fecha,
                attendance.HoraLlegada,
                attendance.Estado.ToString(),
                attendance.Seccion == null ? "—" : attendance.Seccion.Aula!.Nombre,
                attendance.Justificacion == null ? null : attendance.Justificacion.Estado.ToString()))
            .ToListAsync();

        SectionCount = Sections.Count;
    }

    public sealed record SectionRow(string Name, string Subject, string Teacher, DayOfWeek Day, TimeSpan Start, TimeSpan End);
    public sealed record AttendanceRow(DateTime Date, TimeSpan Arrival, string Status, string Section, string? JustificationStatus);
}
