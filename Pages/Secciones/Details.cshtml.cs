using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Secciones
{
    public class DetailsModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DetailsModel(AsistenciaContext context)
        {
            _context = context;
        }

        public Seccion Seccion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var seccion = await _context.Secciones
                .Include(s => s.Materia)
                .Include(s => s.Profesor)
                .Include(s => s.Aula)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (seccion == null) return NotFound();

            Seccion = seccion;
            return Page();
        }
    }
}