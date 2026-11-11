using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Asistencias
{
    public class EditModel : PageModel
    {
        private readonly SistemaAsistencia.Data.AsistenciaContext _context;

        public EditModel(SistemaAsistencia.Data.AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Asistencia Asistencia { get; set; } = default!;

        public string MinimumDate { get; private set; } = string.Empty;
        public string MaximumDate { get; private set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(item => item.Id == id && item.Seccion!.ProfesorId == profesor.Id);
            if (asistencia == null)
            {
                return NotFound();
            }
            Asistencia = asistencia;
            ConfigurarRangoFecha(DateTime.Today);
            CargarListas(profesor.Id);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            var asistenciaExistente = await _context.Asistencias
                .FirstOrDefaultAsync(item => item.Id == Asistencia.Id && item.Seccion!.ProfesorId == profesor.Id);
            if (asistenciaExistente is null)
            {
                return NotFound();
            }

            var seccion = await _context.Secciones
                .FirstOrDefaultAsync(item => item.Id == Asistencia.SeccionId && item.ProfesorId == profesor.Id);
            if (seccion is null)
            {
                ModelState.AddModelError("Asistencia.SeccionId", "La sección seleccionada no existe o no pertenece a tu perfil.");
                ConfigurarRangoFecha(DateTime.Today);
                CargarListas(profesor.Id);
                return Page();
            }

            var today = DateTime.Today;
            ConfigurarRangoFecha(today);

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

            asistenciaExistente.Fecha = Asistencia.Fecha;
            asistenciaExistente.HoraLlegada = Asistencia.HoraLlegada;
            asistenciaExistente.EstudianteId = Asistencia.EstudianteId;
            asistenciaExistente.SeccionId = Asistencia.SeccionId;
            asistenciaExistente.Estado = Asistencia.Estado;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AsistenciaExistsAsync(Asistencia.Id, profesor.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

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

        private Task<bool> AsistenciaExistsAsync(int id, int profesorId)
        {
            return _context.Asistencias.AnyAsync(asistencia =>
                asistencia.Id == id && asistencia.Seccion!.ProfesorId == profesorId);
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
