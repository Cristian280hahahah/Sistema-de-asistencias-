using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Asistencias
{
    public class CreateModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public CreateModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Asistencia Asistencia { get; set; } = default!;

        public string MinimumDate { get; private set; } = string.Empty;
        public string MaximumDate { get; private set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            var today = DateTime.Today;
            Asistencia = new Asistencia { Fecha = today };
            ConfigurarRangoFecha(today);
            CargarListas(profesor.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            var today = DateTime.Today;
            ConfigurarRangoFecha(today);

            var seccion = await _context.Secciones
                .FirstOrDefaultAsync(item => item.Id == Asistencia.SeccionId && item.ProfesorId == profesor.Id);
            if (seccion is null)
            {
                ModelState.AddModelError("Asistencia.SeccionId", "La sección seleccionada no existe o no pertenece a tu perfil.");
                CargarListas(profesor.Id);
                return Page();
            }

            if (!ModelState.IsValid)
            {
                CargarListas(profesor.Id);
                return Page();
            }

            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            if (Asistencia.Fecha.Date < firstDayOfMonth || Asistencia.Fecha.Date > today)
            {
                ModelState.AddModelError("Asistencia.Fecha", "La fecha debe estar dentro del mes actual y no puede ser posterior a hoy.");
                CargarListas(profesor.Id);
                return Page();
            }

            DateTime fechaUnica = Asistencia.Fecha.Date;

            // Validar si ya registró asistencia hoy para esta sección
            bool yaMarco = await _context.Asistencias.AnyAsync(a => 
                a.EstudianteId == Asistencia.EstudianteId && 
                a.SeccionId == Asistencia.SeccionId && 
                a.Fecha == fechaUnica);

            if (yaMarco)
            {
                ModelState.AddModelError(string.Empty, "El estudiante ya tiene asistencia registrada para este día en la sección seleccionada.");
                CargarListas(profesor.Id);
                return Page();
            }

            var estudiante = await _context.Estudiantes
                .Include(item => item.Secciones)
                .FirstOrDefaultAsync(item => item.Id == Asistencia.EstudianteId);
            if (estudiante == null)
            {
                ModelState.AddModelError(string.Empty, "El estudiante seleccionado no existe.");
                CargarListas(profesor.Id);
                return Page();
            }

            if (!estudiante.Secciones.Any(item => item.Id == seccion.Id))
            {
                estudiante.Secciones.Add(seccion);
            }

            // Lógica de cálculo de estado
            TimeSpan horaEntrada = Asistencia.HoraLlegada;
            TimeSpan toleranciaTardanza = seccion.HoraInicio.Add(TimeSpan.FromMinutes(15));
            TimeSpan limiteAusencia = seccion.HoraInicio.Add(TimeSpan.FromMinutes(30));

            if (horaEntrada > limiteAusencia)
            {
                Asistencia.Estado = EstadoAsistencia.Ausente;
            }
            else if (horaEntrada > toleranciaTardanza)
            {
                Asistencia.Estado = EstadoAsistencia.Tardanza;
            }
            else
            {
                Asistencia.Estado = EstadoAsistencia.Presente;
            }

            Asistencia.Fecha = fechaUnica;

            _context.Asistencias.Add(Asistencia);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void ConfigurarRangoFecha(DateTime today)
        {
            MinimumDate = new DateTime(today.Year, today.Month, 1).ToString("yyyy-MM-dd");
            MaximumDate = today.ToString("yyyy-MM-dd");
        }

        private void CargarListas(int profesorId)
        {
            var estudiantes = _context.Estudiantes
                .OrderBy(e => e.Nombre)
                .Select(e => new { e.Id, Nombre = e.Codigo + " — " + e.Nombre })
                .ToList();
            var secciones = _context.Secciones
                .Where(s => s.ProfesorId == profesorId)
                .Include(s => s.Materia)
                .Include(s => s.Aula)
                .OrderBy(s => s.Aula!.Nombre)
                .Select(s => new { s.Id, Nombre = s.Aula!.Nombre + " — " + s.Materia!.Nombre })
                .ToList();

            ViewData["EstudianteId"] = new SelectList(estudiantes, "Id", "Nombre");
            ViewData["SeccionId"] = new SelectList(secciones, "Id", "Nombre");
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