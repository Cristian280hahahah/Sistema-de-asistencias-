using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Secciones
{
    public class IndexModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public IndexModel(AsistenciaContext context)
        {
            _context = context;
        }

        public IList<Seccion> Secciones { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Secciones = await _context.Secciones
                .Include(s => s.Materia)
                .Include(s => s.Profesor)
                .Include(s => s.Aula)
                .ToListAsync();
        }
    }
}