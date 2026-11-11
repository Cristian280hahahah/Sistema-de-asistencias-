using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.PortalProfesor;

public class IndexModel : PageModel
{
    private readonly AsistenciaContext _context;

    public IndexModel(AsistenciaContext context) => _context = context;

    public string ProfesorNombre { get; private set; } = string.Empty;
    public int AsistenciasHoy { get; private set; }
    public int JustificacionesPendientes { get; private set; }
    public IList<ClaseRow> Clases { get; private set; } = new List<ClaseRow>();

    public async Task<IActionResult> OnGetAsync()
    {
        var cuentaId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (cuentaId is null) return Forbid();

        var profesor = await _context.Profesores.AsNoTracking()
            .Where(item => item.CuentaUsuarioId == cuentaId)
            .Select(item => new { item.Id, item.Nombre })
            .FirstOrDefaultAsync();
        if (profesor is null) return Forbid();

        ProfesorNombre = profesor.Nombre;
        Clases = await _context.Secciones.AsNoTracking()
            .Where(seccion => seccion.ProfesorId == profesor.Id)
            .OrderBy(seccion => seccion.Dia)
            .ThenBy(seccion => seccion.HoraInicio)
            .Select(seccion => new ClaseRow(
                seccion.Aula!.Nombre,
                seccion.Materia!.Nombre,
                seccion.Dia,
                seccion.HoraInicio,
                seccion.HoraFin))
            .ToListAsync();

        var seccionesDelProfesor = _context.Secciones
            .Where(seccion => seccion.ProfesorId == profesor.Id);
        var hoy = DateTime.Today;
        AsistenciasHoy = await _context.Asistencias
            .CountAsync(asistencia => asistencia.Fecha == hoy && seccionesDelProfesor.Any(seccion => seccion.Id == asistencia.SeccionId));
        JustificacionesPendientes = await _context.Justificaciones
            .CountAsync(justificacion => justificacion.Estado == EstadoJustificacion.Pendiente &&
                seccionesDelProfesor.Any(seccion => seccion.Id == justificacion.Asistencia!.SeccionId));

        return Page();
    }

    public sealed record ClaseRow(string Aula, string Materia, DayOfWeek Dia, TimeSpan HoraInicio, TimeSpan HoraFin);
}
