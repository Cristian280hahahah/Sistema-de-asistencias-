using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Justificaciones
{
    public class DeleteModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DeleteModel(AsistenciaContext context)
        {
            _context = context;
        }

        public Justificacion Justificacion { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            if (id == null) return NotFound();

            var justificacion = await ObtenerJustificacionAsync(id.Value, profesor.Id);
            if (justificacion is null) return NotFound();

            Justificacion = justificacion;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            if (id == null) return NotFound();

            var justificacion = await ObtenerJustificacionAsync(id.Value, profesor.Id);
            if (justificacion is null) return NotFound();

            _context.Justificaciones.Remove(justificacion);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private Task<Justificacion?> ObtenerJustificacionAsync(int id, int profesorId)
        {
            return _context.Justificaciones
                .Where(j => j.Id == id && j.Asistencia!.Seccion!.ProfesorId == profesorId)
                .Include(j => j.Asistencia)
                    .ThenInclude(a => a!.Estudiante)
                .Include(j => j.Asistencia)
                    .ThenInclude(a => a!.Seccion)
                        .ThenInclude(seccion => seccion!.Aula)
                .FirstOrDefaultAsync();
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
