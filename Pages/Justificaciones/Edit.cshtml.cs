using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Justificaciones
{
    public class EditModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public EditModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public EstadoJustificacion Estado { get; set; }

        public Justificacion Justificacion { get; private set; } = default!;
        public string DetalleAsistencia { get; private set; } = string.Empty;

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

            PrepararVista(justificacion);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            var justificacion = await ObtenerJustificacionAsync(Id, profesor.Id);
            if (justificacion is null) return NotFound();

            if (!Enum.IsDefined(typeof(EstadoJustificacion), Estado))
            {
                ModelState.AddModelError(nameof(Estado), "El estado seleccionado no es válido.");
            }

            if (!ModelState.IsValid)
            {
                PrepararVista(justificacion);
                return Page();
            }

            justificacion.Estado = Estado;
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

        private void PrepararVista(Justificacion justificacion)
        {
            Justificacion = justificacion;
            DetalleAsistencia = $"{justificacion.Asistencia?.Estudiante?.Nombre} - {justificacion.Asistencia?.Seccion?.Aula?.Nombre} ({justificacion.Asistencia?.Fecha:dd/MM/yyyy})";
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
