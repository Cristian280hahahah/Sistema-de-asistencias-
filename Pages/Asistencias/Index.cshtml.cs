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
    public class IndexModel : PageModel
    {
        private readonly SistemaAsistencia.Data.AsistenciaContext _context;

        public IndexModel(SistemaAsistencia.Data.AsistenciaContext context)
        {
            _context = context;
        }

        public IList<Asistencia> Asistencia { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var profesor = await ResolverProfesorAsync();
            if (profesor is null)
            {
                return Forbid();
            }

            Asistencia = await _context.Asistencias
                .Where(asistencia => asistencia.Seccion!.ProfesorId == profesor.Id)
                .Include(a => a.Estudiante)
                .Include(a => a.Seccion)
                    .ThenInclude(seccion => seccion!.Aula)
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
