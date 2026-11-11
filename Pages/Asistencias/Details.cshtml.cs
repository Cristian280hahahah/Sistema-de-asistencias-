using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Asistencias
{
    public class DetailsModel : PageModel
    {
        private readonly SistemaAsistencia.Data.AsistenciaContext _context;

        public DetailsModel(SistemaAsistencia.Data.AsistenciaContext context)
        {
            _context = context;
        }

        public Asistencia Asistencia { get; set; } = default!;

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
                .Include(item => item.Seccion)
                    .ThenInclude(seccion => seccion!.Aula)
                .Include(item => item.Estudiante)
                .FirstOrDefaultAsync(m => m.Id == id && m.Seccion!.ProfesorId == profesor.Id);

            if (asistencia is not null)
            {
                Asistencia = asistencia;

                return Page();
            }

            return NotFound();
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
