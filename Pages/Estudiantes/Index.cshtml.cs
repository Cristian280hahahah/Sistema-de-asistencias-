using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Estudiantes
{
    public class IndexModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public IndexModel(AsistenciaContext context)
        {
            _context = context;
        }

        public IList<Estudiante> Estudiantes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Estudiantes = await _context.Estudiantes.ToListAsync();
        }
    }
}