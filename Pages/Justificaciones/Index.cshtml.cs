using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Justificaciones
{
    public class IndexModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public IndexModel(AsistenciaContext context)
        {
            _context = context;
        }

        public IList<Justificacion> Justificaciones { get; set; } = new List<Justificacion>();
        public int Total { get; private set; }
        public int Pendientes { get; private set; }
        public int Aprobadas { get; private set; }
        public string? Estado { get; set; }

        public async Task<IActionResult> OnGetAsync(string? estado)
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            Estado = estado;
            IQueryable<Justificacion> query = _context.Justificaciones
                .Where(j => j.Asistencia!.Seccion!.ProfesorId == profesor.Id)
                .Include(j => j.Asistencia)
                    .ThenInclude(a => a!.Estudiante)
                .Include(j => j.Asistencia)
                    .ThenInclude(a => a!.Seccion)
                        .ThenInclude(seccion => seccion!.Aula);

            Total = await query.CountAsync();
            Pendientes = await query.CountAsync(j => j.Estado == EstadoJustificacion.Pendiente);
            Aprobadas = await query.CountAsync(j => j.Estado == EstadoJustificacion.Aprobado);

            if (Enum.TryParse<EstadoJustificacion>(estado, true, out var estadoSeleccionado))
            {
                query = query.Where(j => j.Estado == estadoSeleccionado);
            }

            Justificaciones = await query
                .OrderByDescending(j => j.Asistencia!.Fecha)
                .ToListAsync();

            return Page();
        }

        private Task<Profesor?> ResolverProfesorAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Task.FromResult<Profesor?>(null);
            }

            return _context.Profesores
                .FirstOrDefaultAsync(profesor => profesor.CuentaUsuarioId == userId);
        }
    }
}
