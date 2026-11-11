using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Justificaciones
{
    public class DetailsModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DetailsModel(AsistenciaContext context)
        {
            _context = context;
        }

        public Justificacion Justificacion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            if (id == null) return NotFound();

            var justificacion = await _context.Justificaciones
                .Where(j => j.Asistencia!.Seccion!.ProfesorId == profesor.Id)
                .Include(j => j.Asistencia)
                    .ThenInclude(a => a!.Estudiante)
                .Include(j => j.Asistencia)
                    .ThenInclude(a => a!.Seccion)
                        .ThenInclude(seccion => seccion!.Aula)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (justificacion == null) return NotFound();

            Justificacion = justificacion;
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
