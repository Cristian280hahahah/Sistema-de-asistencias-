using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Justificaciones
{
    public class CreateModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public CreateModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int AsistenciaId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            await CargarInasistenciasAsync(profesor.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.Id == AsistenciaId
                    && a.Seccion!.ProfesorId == profesor.Id
                    && (a.Estado == EstadoAsistencia.Ausente || a.Estado == EstadoAsistencia.Tardanza));

            if (asistencia is null)
            {
                ModelState.AddModelError(nameof(AsistenciaId), "La asistencia seleccionada no existe o no pertenece a tus secciones.");
            }
            else if (await _context.Justificaciones.AnyAsync(j => j.AsistenciaId == asistencia.Id))
            {
                ModelState.AddModelError(nameof(AsistenciaId), "Esta asistencia ya tiene una justificación registrada.");
            }

            if (!ModelState.IsValid)
            {
                await CargarInasistenciasAsync(profesor.Id);
                return Page();
            }

            _context.Justificaciones.Add(new Justificacion
            {
                AsistenciaId = asistencia!.Id,
                Motivo = Motivo.Trim(),
                Estado = EstadoJustificacion.Pendiente
            });
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task CargarInasistenciasAsync(int profesorId)
        {
            var asistencias = await _context.Asistencias
                .Where(a => a.Seccion!.ProfesorId == profesorId
                    && (a.Estado == EstadoAsistencia.Ausente || a.Estado == EstadoAsistencia.Tardanza)
                    && !_context.Justificaciones.Any(j => j.AsistenciaId == a.Id))
                .Select(a => new
                {
                    a.Id,
                    Descripcion = $"{a.Estudiante!.Nombre} - {a.Seccion!.Aula!.Nombre} ({a.Fecha:dd/MM/yyyy} - {a.Estado})"
                })
                .ToListAsync();

            ViewData["AsistenciaId"] = new SelectList(asistencias, "Id", "Descripcion");
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
